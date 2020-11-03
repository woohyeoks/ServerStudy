﻿using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace EchoServer
{
    class MainServer : AppServer<NetworkSession, EFBinaryRequestInfo>
    {
        public static SuperSocket.SocketBase.Logging.ILog MainLogger;

        Dictionary<int, Action<NetworkSession, EFBinaryRequestInfo>> HandlerMap = new Dictionary<int, Action<NetworkSession, EFBinaryRequestInfo>>();
        IServerConfig m_Config;
        CommonHandler CommonHan = new CommonHandler();

        public MainServer()
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, EFBinaryRequestInfo>())
        {
            NewSessionConnected += new SessionHandler<NetworkSession>(OnConnected);
            SessionClosed += new SessionHandler<NetworkSession, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<NetworkSession, EFBinaryRequestInfo>(RequestReceived);
        }

        public void InitConfig()
        {
            m_Config = new ServerConfig
            {
                Port = 8888,
                Ip = "Any",
                MaxConnectionNumber = 100,
                Mode = SocketMode.Tcp,
                Name = "EchoServer"
            };
        }


        public void CreateServer()
        {
            try
            {
                bool bResult = Setup(new RootConfig(), m_Config, logFactory: new ConsoleLogFactory());

                if (bResult == false)
                {
                    Console.WriteLine("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                    return;
                }

                MainLogger = base.Logger;

                RegistHandler();

                MainLogger.Info("서버 생성 성공");
            }
            catch (Exception ex)
            {
                MainLogger.Error($"서버 생성 실패: {ex.ToString()}");
            }
        }


        void OnConnected(NetworkSession session)
        {
            MainLogger.Info($"세션 번호 {session.SessionID} 접속 start, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        void OnClosed(NetworkSession session, CloseReason reason)
        {
            MainLogger.Info($"세션 번호 {session.SessionID},  접속해제: {reason.ToString()}");
        }

        void RequestReceived(NetworkSession session, EFBinaryRequestInfo reqInfo)
        {
            MainLogger.Debug($"세션 번호 {session.SessionID},  받은 데이터 크기: {reqInfo.Body.Length}, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            var PacketID = reqInfo.PacketID;

            if (HandlerMap.ContainsKey(PacketID))
            {
                HandlerMap[PacketID](session, reqInfo);
            }
            else
            {
                MainLogger.Info($"세션 번호 {session.SessionID} 받은 데이터 크기: {reqInfo.Body.Length}");
            }
        }


        void RegistHandler()
        {
            HandlerMap.Add((int)PACKETID.REQ_ECHO, CommonHan.RequestEcho);

            MainLogger.Info("핸들러 등록 완료");
        }
    }

    public class NetworkSession : AppSession<NetworkSession, EFBinaryRequestInfo>
    {

    }
}
