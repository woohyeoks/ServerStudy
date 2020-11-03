using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class MySocket
{
    Socket Sock;
    byte[] RecvBuf = new byte[1024 * 4];
    public byte[] Buf = new byte[1024 * 4];
    string LastAddr;
    int LastPort;
    public int BufPos = 0;


    Action FuncConnect;
    Action<byte[], int> FuncRecv;
    Action<string> FuncClosed;



    public void TryConnect(string ip, int port, Action funcConn, Action<byte[], int> funcRecv, Action<string> funcClosed)
    {
        LastAddr = ip;
        LastPort = port;

        FuncConnect = funcConn;
        FuncRecv = funcRecv;
        FuncClosed = funcClosed;


        Sock = new Socket(
          AddressFamily.InterNetwork,
          SocketType.Stream,
          ProtocolType.Tcp
          );
        Sock.NoDelay = true;

        Dns.BeginResolve(ip, OnDnsResolveCallback, this);
    }

    private void OnDnsResolveCallback(IAsyncResult ar)
    {
        try
        {
            var host = Dns.EndResolve(ar);
            if (host.AddressList.Length < 1)
            {
                throw new Exception("AddressList is empty");
            }

            IPAddress ipv4_addr = null;
            IPAddress ipv6_addr = null;
            foreach (var addr in host.AddressList)
            {
                if (ipv4_addr == null)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipv4_addr = addr;
                    }
                }

                if (ipv6_addr == null)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        ipv6_addr = addr;
                    }
                }
            }

            var addr_0 = ipv4_addr;
            if (addr_0 == null)
            {
                addr_0 = ipv6_addr;
            }

            if (addr_0 == null)
            {
                throw new Exception("addr_0 is null");
            }

            if (Sock.AddressFamily != addr_0.AddressFamily)
            {
                Sock.Close();
                Sock = new Socket(addr_0.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }

            var lep = new IPEndPoint(addr_0, LastPort);
            Sock.BeginConnect(
                lep,
                new AsyncCallback(OnConnectCallback),
                this);
        }
        catch (Exception e)
        {
            if (Sock != null)
            {
                var s = Sock;
                Sock = null;
                s.Close();
                FuncClosed("OnDnsResolveCallback ex: " + e.Message);
            }
        }
    }

    private void OnConnectCallback(IAsyncResult ar)
    {
        try
        {
            Sock.EndConnect(ar);

            BufPos = 0;

            FuncConnect();

            TryReceive();
        }
        catch (Exception e)
        {
            if (Sock != null)
            {
                var s = Sock;
                Sock = null;
                s.Close();
                FuncClosed("OnConnectCallback ex: " + e.Message);
            }
        }
    }

    private void TryReceive()
    {
        try
        {
            Sock.BeginReceive(
                RecvBuf,
                0,
                RecvBuf.Length,
                0,
                new AsyncCallback(OnReceiveCallback),
                this
            );
        }
        catch (Exception e)
        {
            if (Sock != null)
            {
                var s = Sock;
                Sock = null;
                s.Close();
                FuncClosed("TryReceive ex: " + e.Message);
            }
        }
    }


    private void OnReceiveCallback(IAsyncResult ar)
    {
        bool needClose = false;
        try
        {
            int size = Sock.EndReceive(ar);
            if (size <= 0)
            {
                throw new Exception("recv zero length");
            }

            var bin = RecvBuf;
            FuncRecv(bin, size);

            TryReceive();
        }
        catch (Exception e)
        {
            if (Sock != null)
            {
                var s = Sock;
                Sock = null;
                s.Close();
                FuncClosed("TryReceive ex: " + e.Message);
            }
        }
    }

    public void TryClose()
    {
        try
        {
            if (Sock != null)
            {
                var s = Sock;
                Sock = null;
                s.Close();
                FuncClosed("TryClose ok");
            }
        }
        catch (Exception e)
        {
            Sock = null;
            FuncClosed("TryClose ex: " + e.Message);
        }

        FuncConnect = delegate { };
        FuncRecv = delegate { };
        FuncClosed = delegate { };
    }


    public void SendPacket(string api, params object[] args)
    {
        var sb = new StringBuilder();
        sb.Append(api);
        foreach (var a in args)
        {
            sb.Append(',');
            sb.Append(a);
        }
        SendText(sb.ToString());
    }

    public void SendText(string msg)
    {
        byte[] bin = new byte[msg.Length + 2];
        int i = 0;
        for (; i < msg.Length; i++)
        {
            bin[i] = (byte)msg[i];
        }
        bin[i++] = (byte)'\r';
        bin[i++] = (byte)'\n';
        Send(bin);
    }

    void Send(byte[] bin)
    {
        try
        {
            Sock.BeginSend(
                bin, 0,
                bin.Length, 0,
                new AsyncCallback(OnSendCallback),
                this);
        }
        catch (Exception e)
        {
            if (Sock != null)
            {
                var s = Sock;
                Sock = null;
                s.Close();
                FuncClosed("Send ex: " + e.Message);
            }
        }
    }


    private void OnSendCallback(IAsyncResult ar)
    {
        try
        {
            Sock.EndSend(ar);
        }
        catch (Exception e)
        {
            if (Sock != null)
            {
                var s = Sock;
                Sock = null;
                s.Close();
                FuncClosed("OnSendCallback ex: " + e.Message);
            }
        }
    }


}




