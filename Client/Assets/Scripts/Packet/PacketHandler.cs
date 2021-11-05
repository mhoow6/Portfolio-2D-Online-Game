using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 네트워크 패킷이 조립되는 이 공간은 레드존임을 기억하자.
class PacketHandler
{
    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn pkt = packet as S_Spawn;

        BaseObject obj = Manager.Spawner.SpawnObject((ObjectCode)pkt.ObjectCode);
        obj.id = pkt.ObjectId;
        obj.CellPos = new Vector3Int(pkt.Position.X, pkt.Position.Y, 0);

        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(obj.CellPos) + new Vector3(0.5f, 0.5f);
        obj.transform.position = pos;
    }
}
