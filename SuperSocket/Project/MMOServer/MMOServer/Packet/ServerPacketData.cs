using ServerLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Packet
{
    public class ServerPacketData
    {
        public Int16 PacketSize;
        public string SessionID;
        public Int16 PacketID;
        public byte[] BodyData;


        public void Assign(string sessionID, Int16 packetID, byte[] packetBodyData)
        {
            SessionID = sessionID;
            PacketID = packetID;

            if (packetBodyData.Length > 0)
            {
                BodyData = packetBodyData;
            }
        }


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
