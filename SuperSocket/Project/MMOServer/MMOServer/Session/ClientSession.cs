using Google.Protobuf;
using Google.Protobuf.Protocol;
using MMOServer.Game;
using SuperSocket.SocketBase;
using System;
using System.Net;

namespace MMOServer.Session
{
    public class ClientSession : AppSession<ClientSession, EFBinaryRequestInfo>
    {

        // 세션에 해당하는 플레이어를 알면 관리하기 쉬우니
        public Player MyPlayer { get; set; }
        public int SessionId { get; set; }

        public void OnConnected()
        {
            MyPlayer = PlayerManager.Instance.Add();

            {
                MyPlayer.Info.Name = $"Player_{MyPlayer.Info.PlayerId}";
                MyPlayer.Info.PosInfo.State = CreatureState.Idle;
                MyPlayer.Info.PosInfo.MoveDir = MoveDir.None;
                MyPlayer.Info.PosInfo.PosX = 0;
                MyPlayer.Info.PosInfo.PosY = 0;
                MyPlayer.Session = this;
            }
            RoomManager.Instance.Find(1).EnterGame(MyPlayer);
        }

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }
    }
}
