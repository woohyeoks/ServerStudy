using Google.Protobuf.Protocol;
using MessagePack;
using MMOServer.Packet;
using ServerLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Game
{
    public class Room
    {
        public int Index { get; private set; }
        public int Number { get; private set; }
        int MaxUserCount = 0;
        Dictionary<int, Player> m_playerList = new Dictionary<int, Player>();
        Map m_map = new Map();


        public void Init(int index, int number, int maxUserCount, int mapId)
        {
            Index = index;
            Number = number;
            MaxUserCount = maxUserCount;
           // m_map.LoadMap(mapId);
        }

        public void EnterGame(Player player, string netSessionID)
        {
            if (player == null)
                return;
            MainServer.MainLogger.Debug($"룸 입장 PID { player.Info.ObjectId }");

            player.Set(player.Info.ObjectId, netSessionID, this);
            m_playerList.Add(player.Info.ObjectId, player);

            // 본인한테 정보 전송
            {
                S_EnterGame enterPacket = new S_EnterGame();
                enterPacket.Player = player.Info;
                PacketProcessor.MainServer.SendData(player.NetSessionID, enterPacket);

                // 본인에게 다른 사람 정보 전달.
                var spawnPacket = new S_Spawn();
                foreach (Player p in m_playerList.Values)
                {
                    if (player != p)
                        spawnPacket.Objects.Add(p.Info);
                }
                PacketProcessor.MainServer.SendData(player.NetSessionID, spawnPacket);
                Console.WriteLine($"PKTNtfGameEnter 본인에게 전송 {m_playerList.Count}");
            }

            // 타인에게 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(player.Info);
                foreach (Player p in m_playerList.Values)
                {
                    if (player != p)
                        PacketProcessor.MainServer.SendData(p.NetSessionID, spawnPacket);

                }
                Console.WriteLine($"PKTNtfGameEnter 타인에게 전송 {m_playerList.Count}");
            }
        }

        public bool GetUser(int playerId)
        {
            Player player = null;
            return m_playerList.TryGetValue(playerId, out player);
        }


    }
}
