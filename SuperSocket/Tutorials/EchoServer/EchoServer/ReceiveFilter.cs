﻿using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;
using SuperSocket.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace EchoServer
{
    public class EFBinaryRequestInfo : BinaryRequestInfo
    {
        // 패킷 헤더용 변수
        public Int16 TotalSize { get; private set; }
        public Int16 PacketID { get; private set; }
        public SByte Value1 { get; private set; }

        public const int HEADER_SIZE = 5;

        public EFBinaryRequestInfo(Int16 totalSize, Int16 packetID, SByte value1, byte[] body)
            : base(null, body)
        {
            this.TotalSize = totalSize;
            this.PacketID = packetID;
            this.Value1 = value1;
        }
    }

    public class ReceiveFilter : FixedHeaderReceiveFilter<EFBinaryRequestInfo>
    {
        public ReceiveFilter() : base(EFBinaryRequestInfo.HEADER_SIZE)
        {

        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(header, offset, 2);
            var packetTotalSize = BitConverter.ToInt16(header, offset);
            return packetTotalSize - EFBinaryRequestInfo.HEADER_SIZE;
        }

        protected override EFBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(header.Array, 0, EFBinaryRequestInfo.HEADER_SIZE);

            return new EFBinaryRequestInfo(BitConverter.ToInt16(header.Array, 0),
                                           BitConverter.ToInt16(header.Array, 0 + 2),
                                           (SByte)header.Array[4],
                                           bodyBuffer.CloneRange(offset, length));
        }
    }
}
