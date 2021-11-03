using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerCore;
using System.Net;
using Google.Protobuf;
using System;

public class NetworkManager : IDisposable
{
    ServerSession _session = new ServerSession();

    public void Dispose()
    {
        _session.Disconnect();
    }

    public void Init()
    {
        string hostName = Dns.GetHostName();
        IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
        IPAddress hostIp = hostEntry.AddressList[0];
        IPEndPoint _host = new IPEndPoint(hostIp, 7080);

        Connector connector = new Connector();
        connector.Connect(_host, () => { return _session; }, 1);
    }

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }

    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }
}
