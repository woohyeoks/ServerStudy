using ServerLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Game
{
    public class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();

        object _lock = new object();

        Dictionary<int, Player> m_players = new Dictionary<int, Player>();

        int _playerId = 1;

        public Player Add()
        {
            Player player = new Player();
            lock (_lock)
            {
                player.Info.PlayerId = _playerId;
                m_players.Add(_playerId, player);
                _playerId++;
            }
            return player;
        }

    }
}
