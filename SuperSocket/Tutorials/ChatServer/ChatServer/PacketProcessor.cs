using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace ChatServer
{
    class PacketProcessor
    {
        bool IsThreadRunning = false;

        // receive쪽에서 처리하지 않아도 Post에서 블럭킹 되지 않는다.
        // BufferBlock<T>(DataflowBlockOptions) 에서 DataflowBlockOptions의 BoundedCapacity로 버퍼 가능 수 지정. BoundedCapacity 보다 크게 쌓이면 블럭킹 된다
        BufferBlock<ServerPacketData> MsgBuffer = new BufferBlock<ServerPacketData>();

        Tuple<int, int> RoomNumberRange = new Tuple<int, int>(-1, -1);
        List<Room> RoomList = new List<Room>();
        UserManager UserMgr = new UserManager();

        Dictionary<int, Action<ServerPacketData>> PacketHandlerMap = new Dictionary<int, Action<ServerPacketData>>();
        System.Threading.Thread ProcessThread = null;

        PKHCommon CommonPacketHandler = new PKHCommon();
        PKHRoom RoomPacketHandler = new PKHRoom();



        public void InsertPacket(ServerPacketData data)
        {
            MsgBuffer.Post(data);
        }

        public void Destory()
        {
            IsThreadRunning = false;
            MsgBuffer.Complete();
        }

        public void CreateAndStart(List<Room> roomList, MainServer mainServer)
        {
            var maxUserCount = MainServer.ServerOption.RoomMaxCount * MainServer.ServerOption.RoomMaxUserCount;
            UserMgr.Init(maxUserCount);

            RoomList = roomList;
            var minRoomNum = RoomList[0].Number;
            var maxRoomNum = RoomList[0].Number + RoomList.Count() - 1;
            RoomNumberRange = new Tuple<int, int>(minRoomNum, maxRoomNum);

            RegistPacketHandler(mainServer);

            IsThreadRunning = true;
            ProcessThread = new System.Threading.Thread(this.Process);
            ProcessThread.Start();

        }

        void RegistPacketHandler(MainServer serverNetwork)
        {
            CommonPacketHandler.Init(serverNetwork, UserMgr);
            CommonPacketHandler.RegistPacketHandler(PacketHandlerMap);

            RoomPacketHandler.Init(serverNetwork, UserMgr);
            RoomPacketHandler.SetRoomList(RoomList);
            RoomPacketHandler.RegistPacketHandler(PacketHandlerMap);
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
