using CSBaseLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer
{
    public class ServerPacketData
    {
        public Int16 PacketSize;
        public string SessionID;
        public Int16 PacketID;
        public SByte Type;
        public byte[] BodyData;

        public static ServerPacketData MakeNTFInConnectOrDisConnectClientPacket(bool isConnect, string sessionID)
        {
            var packet = new ServerPacketData();
            if (isConnect)
            {
                packet.PacketID = (Int32)PACKETID.NTF_IN_CONNECT_CLIENT;
            }
            else
            {
                packet.PacketID = (Int32)PACKETID.NTF_IN_DISCONNECT_CLIENT;
            }
            packet.SessionID = sessionID;
            return packet;
        }
    }
}
