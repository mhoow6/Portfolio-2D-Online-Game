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
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
        
        // 게임에 입장했으면 내 자신 소환
        BaseObject gameObject = Manager.Spawner.SpawnObject((ObjectCode)enterGamePacket.Player.ObjectCode);
        gameObject.id = enterGamePacket.Player.ObjectId;
        gameObject.CellPos = new Vector3Int(enterGamePacket.Player.Position.X, enterGamePacket.Player.Position.Y, 0);
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(gameObject.CellPos) + new Vector3(0.5f, 0.5f);
        gameObject.transform.position = pos;
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn pkt = packet as S_Spawn;

        // 서버에서 추가적으로 스폰된 애들을 소환시킨다.
        foreach (var obj in pkt.Objects)
        {
            BaseObject gameObject = Manager.Spawner.SpawnObject((ObjectCode)obj.ObjectCode);
            gameObject.id = obj.ObjectId;
            gameObject.CellPos = new Vector3Int(obj.Position.X, obj.Position.Y, 0);

            Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(gameObject.CellPos) + new Vector3(0.5f, 0.5f);
            gameObject.transform.position = pos;
        }
    }
}
