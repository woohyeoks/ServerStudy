using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLib
{
    public class PacketDef
    {
        public const Int16 PACKET_HEADER_SIZE = 4;
        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;

        public const int INVALID_ROOM_NUMBER = -1;
    }


    public class PacketToBytes
    {
        public static byte[] Make(PACKETID packetID, byte[] bodyData)
        {
            byte type = 0;
            var pktID = (Int16)packetID;
            Int16 bodyDataSize = 0;
            if (bodyData != null)
            {
                bodyDataSize = (Int16)bodyData.Length;
            }
            var packetSize = (Int16)(bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

            var dataSource = new byte[packetSize];
            Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(pktID), 0, dataSource, 2, 2);

            if (bodyData != null)
            {
                Buffer.BlockCopy(bodyData, 0, dataSource, 4, bodyDataSize);
            }

            return dataSource;
        }

        public static Tuple<int, byte[]> ClientReceiveData(int recvLength, byte[] recvData)
        {
            var packetSize = BitConverter.ToInt16(recvData, 0);
            var packetID = BitConverter.ToInt16(recvData, 2);
            var bodySize = packetSize - PacketDef.PACKET_HEADER_SIZE;

            var packetBody = new byte[bodySize];
            Buffer.BlockCopy(recvData, PacketDef.PACKET_HEADER_SIZE, packetBody, 0, bodySize);

            return new Tuple<int, byte[]>(packetID, packetBody);
        }
    }

    /*public enum CreatureState
    {
        IDLE = 0,
        MOVING = 1,
        SKILL = 2,
        DEAD = 3,
    }

    public enum MoveDir
    {
        NONE = 0,
        UP = 1,
        DOWN = 2,
        LEFT = 3,
        RIGHT = 4
    }

    [MessagePackObject]
    public struct PlayerInfo
    {
        [Key(0)]
        public Int32 playerId;
        [Key(1)]
        public string name { get; set; }
        [Key(2)]
        public PositionInfo posInfo { get; set; }
    }

    [MessagePackObject]
    public struct PositionInfo
    {
        [Key(0)]
        public CreatureState state;
        [Key(1)]
        public MoveDir moveDir;
        [Key(2)]
        public Int32 posX;
        [Key(3)]
        public Int32 posY;
    }

    [MessagePackObject]
    public class PKTNtfGameEnter
    {
        [Key(0)]
        public PlayerInfo Player;
    }

    [MessagePackObject]
    public class PKTNtfRoomPlayerList
    {
        [Key(0)]
        public List<PlayerInfo> players = new List<PlayerInfo>();
    }*/



}
