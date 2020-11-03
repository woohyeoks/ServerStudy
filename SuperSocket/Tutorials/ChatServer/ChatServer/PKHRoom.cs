using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer
{
    public class PKHRoom : PKHandler
    {
        List<Room> RoomList = null;
        int StartRoomNumber;

        public void SetRoomList(List<Room> roomList)
        {
            RoomList = roomList;
            StartRoomNumber = roomList[0].Number;
        }

        public void RegistPacketHandler(Dictionary<int, Action<ServerPacketData>> packetHandlerMap)
        {
        }
    }
}
