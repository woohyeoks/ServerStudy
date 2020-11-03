using Google.Protobuf;
using Google.Protobuf.Protocol;
using MMOServer.Game;
using MMOServer.Packet;
using MMOServer.Session;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using static MMOServer.EFBinaryRequestInfo;

namespace MMOServer
{
    public class MainServer : AppServer<ClientSession, EFBinaryRequestInfo>
    {
        public static ServerOption ServerOption;
        public static SuperSocket.SocketBase.Logging.ILog MainLogger;
        SuperSocket.SocketBase.Config.IServerConfig m_Config;

        PacketProcessor MainPacketProcessor = new PacketProcessor();

        public MainServer()
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, EFBinaryRequestInfo>())
        {
            NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
            SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<ClientSession, EFBinaryRequestInfo>(OnPacketReceived);
        }
        public void InitConfig(ServerOption option)
        {
            ServerOption = option;
            m_Config = new SuperSocket.SocketBase.Config.ServerConfig
            {
                Name = option.Name,
                Ip = "Any",
                Port = option.Port,
                Mode = SocketMode.Tcp,
                MaxConnectionNumber = option.MaxConnectionNumber,
                MaxRequestLength = option.MaxRequestLength,
                ReceiveBufferSize = option.ReceiveBufferSize,
                SendBufferSize = option.SendBufferSize
            };
        }

        public void CreateStartServer()
        {
            try
            {
                bool bResult = Setup(new SuperSocket.SocketBase.Config.RootConfig(), m_Config, logFactory: new SuperSocket.SocketBase.Logging.NLogLogFactory());
                if (bResult == false)
                {
                    Console.WriteLine("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                    return;
                }
                else
                {
                    MainLogger = base.Logger;
                    MainLogger.Info("서버 초기화 성공");
                }
                CreateComponent();
                Start();
                MainLogger.Info("서버 생성 성공");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 서버 생성 실패: {ex.ToString()}");
            }
        }

        public void CreateComponent()
        {
            RoomManager.Instance.CreateRooms();
            MainPacketProcessor = new PacketProcessor();
            MainPacketProcessor.CreateAndStart(this);
            MainLogger.Info("CreateComponent - Success");
        }

        public void StopServer()
        {
            Stop();

            //MainPacketProcessor.Destory();
        }

        // 주의 => 멀티스레드 환경에서 동작한다 
        void OnConnected(ClientSession session)
        {
            // 옵션의 최대 연결 수를 넘으면 SuperSocket이 바로 접속을 짤라버린다. 즉 이 OnConnected 함수가 호출되지 않는다.
            MainLogger.Info(string.Format("OnConnected 세션 번호 {0} 접속", session.SessionID));

            // 세션 추가
            SessionManager.Instance.AddSession(session);
            session.OnConnected();
            
           /* var packet = ServerPacketData.MakeNTFInConnectOrDisConnectClientPacket(true, session.SessionID);
            Distribute(packet);*/
        }

        void OnClosed(ClientSession session, CloseReason reason)
        {
            MainLogger.Info(string.Format("세션 번호 {0} 접속해제: {1}", session.SessionID, reason.ToString()));
            var packet = ServerPacketData.MakeNTFInConnectOrDisConnectClientPacket(false, session.SessionID);
            Distribute(packet);
        }

        void OnPacketReceived(ClientSession session, EFBinaryRequestInfo reqInfo)
        {
            MainLogger.Debug(string.Format("세션 번호 {0} 받은 데이터 크기: {1}, ThreadId: {2}", session.SessionID, reqInfo.Body.Length, System.Threading.Thread.CurrentThread.ManagedThreadId));

            session.OnRecvPacket(reqInfo.Body);

           /* var packet = new ServerPacketData();
            packet.SessionID = session.SessionID;
            packet.PacketSize = reqInfo.Size;
            packet.PacketID = reqInfo.PacketID;
            packet.BodyData = reqInfo.Body;

            Distribute(packet);*/

        }

        public bool SendData(string sessionID, IMessage packet)
        {


            var session = GetSessionByID(sessionID);
            try
            {
                if (session == null)
                {
                    return false;
                }
                string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
                MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

                ushort size = (ushort)packet.CalculateSize();
                byte[] sendBuffer = new byte[size + 4];
                Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
                Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
                Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

                session.Send(new ArraySegment<byte>(sendBuffer));
            }
            catch (Exception ex)
            {
                // TimeoutException 예외가 발생할 수 있다
                MainServer.MainLogger.Error($"{ex.ToString()},  {ex.StackTrace}");
                session.SendEndWhenSendingTimeOut();
                session.Close();
            }
            return true;
        }


        public void Distribute(ServerPacketData requestPacket)
        {
            MainPacketProcessor.InsertPacket(requestPacket);
        }




    }

}

