using Google.Protobuf.Protocol;
using ServerLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Game
{
    public class Player
    {
        public ObjectInfo Info { get; set; }
        public GameObjectTpye ObjectType { get; protected set; } = GameObjectTpye.None;
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();

        public string UserID { get; private set; }
        public string NetSessionID { get; private set; }
        // 현재 속한 룸정보
        public Room MyRoom { get; private set; }

        public void Set(int playerID, string netSessionID, Room room)
        {
           // PlayerID = playerID;
            NetSessionID = netSessionID;
            MyRoom = room;
        }

    }
}
