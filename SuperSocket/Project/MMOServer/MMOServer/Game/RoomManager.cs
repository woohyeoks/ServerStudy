using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Game
{
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();
        Dictionary<int, Room> m_roomList = new Dictionary<int, Room>();

        public void CreateRooms()
        {
            var maxRoomCount = MainServer.ServerOption.RoomMaxCount;
            var startNumber = MainServer.ServerOption.RoomStartNumber;
            var maxUserCount = MainServer.ServerOption.RoomMaxUserCount;

            for (int i = 0; i < maxRoomCount; ++i)
            {
                var roomNumber = (startNumber + i);
                var room = new Room();
                room.Init(i, roomNumber, maxUserCount, mapId : 1);
                m_roomList.Add(roomNumber, room);
            }
        }

        public Room Find(int roomNum)
        {

            Room room = null;
            if (m_roomList.TryGetValue(roomNum, out room))
                return room;
            return null;

        }



    }
}
