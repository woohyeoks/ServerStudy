using ServerLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Game
{
    public class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();

        object m_lock = new object();
        Dictionary<int, Player> m_players = new Dictionary<int, Player>();

        int m_playerId = 1;

        public Player Add()
        {
            Player player = new Player();
            lock (m_lock)
            {
                player.Info.ObjectId = m_playerId;
                m_players.Add(m_playerId, player);
                m_playerId++;
            }
            return player;
        }

    }
}
