using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public enum MySockEventType
    {
        Connected,
        Disconnected,
        Recv,
    }

    public class MySockEvent
    {
        public MySockEventType Type;
        public string recvLine;
    }

    MySocket sock = new MySocket();
    List<MySockEvent> sockEvents = new List<MySockEvent>();
    //public Button buttonConnect;

    public void Init()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);


    }
}
