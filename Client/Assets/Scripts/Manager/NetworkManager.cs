using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerCore;
using System.Net;
using Google.Protobuf;
using System;
using Google.Protobuf.Protocol;

public class NetworkManager
{
    ServerSession _session = new ServerSession();

    public void Init()
    {
        string hostName = Dns.GetHostName();
        IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
        IPAddress hostIp = hostEntry.AddressList[0];
        IPEndPoint _host = new IPEndPoint(hostIp, 7080);

        Connector connector = new Connector();
        connector.Connect(_host, () => { return _session; }, 1);
    }

    public void SendMovePacket(ObjectInfo objInfo)
    {
        C_Move pkt = new C_Move();
        pkt.ObjectInfo = objInfo;
        _session.Send(pkt);
    }

    public void SendAttackPacket(ObjectInfo objInfo)
    {
        C_Attack pkt = new C_Attack();
        pkt.AttackerInfo = objInfo;
        _session.Send(pkt);
    }

    public void SendLeaveGamePacket(ObjectInfo objInfo)
    {
        C_LeaveGame pkt = new C_LeaveGame();
        pkt.ObjectId = objInfo.ObjectId;
        _session.Send(pkt);
    }

    public void SendSyncPacket(ObjectInfo objInfo)
    {
        C_Sync pkt = new C_Sync();
        pkt.ObjectInfo = objInfo;
        _session.Send(pkt);
    }

    public void SendSpawnPacket(SpawnInfo spawnInfo)
    {
        C_Spawn pkt = new C_Spawn();
        pkt.SpawnInfo = spawnInfo;
        _session.Send(pkt);
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
