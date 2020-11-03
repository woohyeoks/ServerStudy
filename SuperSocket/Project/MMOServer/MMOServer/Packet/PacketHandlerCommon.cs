using Google.Protobuf.Protocol;
using MMOServer.Game;
using ServerLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOServer.Packet
{
    public class PacketHandlerCommon : PacketHandler
    {
        // 별도의 싱글 스레드에서 호출된다.
        public void RegistPacketHandler(Dictionary<int, Action<ServerPacketData>> packetHandlerMap)
        {
            packetHandlerMap.Add((int)PACKETID.NTF_IN_CONNECT_CLIENT, NotifyInConnectClient);
        }

       
        public void NotifyInConnectClient(ServerPacketData requestData)
        {
            var sessionID = requestData.SessionID;
            
            var MyPlayer = PlayerManager.Instance.Add();

            {
                MyPlayer.Info.Name = $"Player_{MyPlayer.Info.ObjectId}";
                MyPlayer.Info.PosInfo.State = CreatureState.Idle;
                MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
                MyPlayer.Info.PosInfo.PosX = 0;
                MyPlayer.Info.PosInfo.PosY = 0;
            }

            RoomManager.Instance.Find(1).EnterGame(MyPlayer, requestData.SessionID);

            MainServer.MainLogger.Debug("연결 성공 플레이어 생성");
        }

        public void RequestLogin(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            MainServer.MainLogger.Debug("로그인 요청 받음");
        }
    }
}
