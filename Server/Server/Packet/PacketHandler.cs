using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

// 네트워크 패킷이 조립되는 이 공간은 레드존임을 기억하자.
class PacketHandler
{
    public static void C_SpawnHandler(PacketSession session, IMessage packet)
    {
        C_Spawn pkt = packet as C_Spawn;
        
        ClientSession Session = session as ClientSession;

        // Objectcode에 따라 스폰 지점 결정
        switch (pkt.ObjectInfo.ObjectCode)
        {
            case (int)ObjectCode.Player:
                break;
            case (int)ObjectCode.Monster:
                break;
        }
    }

    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        // 클라에서 받은 CellPos를 통해 서버의 맵에 업데이트.
        C_Move pkt = packet as C_Move;

        // 원자성이 보장되는 영역인 Room에서 해야 됨
        Room pktRoom = RoomManager.Instance.Find(pkt.ObjectInfo.RoomId);
        if (pktRoom != null)
        {
            pktRoom.Push(pktRoom.C_Move, pkt);
        }
    }
}
