﻿using Google.Protobuf.Protocol;
using ServerLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Game
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo() { PosInfo = new PositionInfo() };
        public GameRoom Room { get; set; }
        public ClientSession Session { get; set; }
    }
}
