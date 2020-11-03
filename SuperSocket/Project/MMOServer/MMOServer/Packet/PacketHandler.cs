using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Packet
{
    public class PacketHandler
    {
        protected MainServer ServerNetwork;

        public void Init(MainServer serverNetwork)
        {
            ServerNetwork = serverNetwork;
        }
    }
}
