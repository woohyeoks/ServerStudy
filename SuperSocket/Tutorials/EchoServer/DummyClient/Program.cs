using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using System.Threading;

namespace DummyClient
{
    class Program
    {
        public enum PACKETID : int
        {
            REQ_ECHO = 101,
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Client!");

            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 8888);

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);

            int header_size = 5;
            byte[] buff = new byte[1024];
            while (true)
            {
                // 패킷 만들기
                List<byte> dataSource = new List<byte>();
                byte[] msg = Encoding.UTF8.GetBytes("abcde");
                var packet_size = msg.Length + header_size;

                dataSource.AddRange(BitConverter.GetBytes((Int16)packet_size));
                dataSource.AddRange(BitConverter.GetBytes((Int16)PACKETID.REQ_ECHO));
                dataSource.AddRange(new byte[1]);
                dataSource.AddRange(msg);
                socket.Send(new ArraySegment<byte>(dataSource.ToArray(), 0, dataSource.Count));
                Thread.Sleep(1000);
                int recvBytes = socket.Receive(buff);
                if (recvBytes > 0)
                {
                    int size = BitConverter.ToInt16(new ArraySegment<byte>(buff, 0, 2));
                    int id = BitConverter.ToInt16(new ArraySegment<byte>(buff, 2, 2));
                    var recvMsg = Encoding.UTF8.GetString(new ArraySegment<byte>(buff, header_size,  size - header_size));

                    Console.WriteLine($"[FromServer] id : {id} msg : {recvMsg}");
                }
            }


        }
    }
}
