using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace MMOServer.Packet
{
    public class PacketProcessor
    {
        bool IsThreadRunning = false;
        System.Threading.Thread ProcessThread = null;
        BufferBlock<ServerPacketData> MsgBuffer = new BufferBlock<ServerPacketData>();
        Dictionary<int, Action<ServerPacketData>> PacketHandlerMap = new Dictionary<int, Action<ServerPacketData>>();

        PacketHandlerCommon CommonPacketHandler = new PacketHandlerCommon();

        public static MainServer MainServer = null;

        public void CreateAndStart(MainServer mainServer)
        {
            IsThreadRunning = true;
            MainServer = mainServer;
            RegistPacketHandler(mainServer);
            ProcessThread = new System.Threading.Thread(this.Process);
            ProcessThread.Start();
        }


        public void InsertPacket(ServerPacketData data)
        {
            MsgBuffer.Post(data);
        }

        void RegistPacketHandler(MainServer serverNetwork)
        {
            CommonPacketHandler.Init(serverNetwork);
            CommonPacketHandler.RegistPacketHandler(PacketHandlerMap);
        }

        void Process()
        {
            while (IsThreadRunning)
            {
                try
                {
                    var packet = MsgBuffer.Receive();
                    if (PacketHandlerMap.ContainsKey(packet.PacketID))
                    {
                        PacketHandlerMap[packet.PacketID](packet);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("세션 번호 {0}, PacketID {1}, 받은 데이터 크기: {2}", packet.SessionID, packet.PacketID, packet.BodyData.Length);
                    }
                }
                catch (Exception ex)
                {
                    IsThreadRunning.IfTrue(() => MainServer.MainLogger.Error(ex.ToString()));
                }

            }
        }
    }
}
