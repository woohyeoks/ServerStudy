using CSBaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatServer
{
    public class PKHCommon : PKHandler
    {
        public void RegistPacketHandler(Dictionary<int, Action<ServerPacketData>> packetHandlerMap)
        {
            packetHandlerMap.Add((int)PACKETID.NTF_IN_CONNECT_CLIENT, NotifyInConnectClient);
            packetHandlerMap.Add((int)PACKETID.NTF_IN_DISCONNECT_CLIENT, NotifyInDisConnectClient);
            packetHandlerMap.Add((int)PACKETID.REQ_LOGIN, RequestLogin);
        }

        public void NotifyInConnectClient(ServerPacketData requestData)
        {
            MainServer.MainLogger.Debug($"Current Connected Session Count: {ServerNetwork.SessionCount}");
        }

        public void NotifyInDisConnectClient(ServerPacketData requestData)
        {
            MainServer.MainLogger.Debug($"Current DisConnected Session Count: {ServerNetwork.SessionCount}");

        }

        public void RequestLogin(ServerPacketData packetData)
        {
            MainServer.MainLogger.Debug("로그인 요청 받음");
        }
    }
}
