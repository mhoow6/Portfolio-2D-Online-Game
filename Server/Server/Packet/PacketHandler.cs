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

    public static void C_AttackHandler(PacketSession session, IMessage packet)
    {
        // 클라에서 받은 CellPos를 통해 적이 공격을 받을 수 있는지 없는지 판단할 것임.
        C_Attack pkt = packet as C_Attack;

        // 원자성이 보장되는 영역인 Room에서 해야 됨
        Room pktRoom = RoomManager.Instance.Find(pkt.AttackerInfo.RoomId);
        if (pktRoom != null)
        {
            pktRoom.Push(pktRoom.C_Attack, pkt);
        }
    }

    public static void C_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        C_LeaveGame pkt = packet as C_LeaveGame;

        Player obj = PlayerManager.Instance.Find(pkt.ObjectId);

        // 원자성이 보장되는 영역인 Room에서 해야 됨
        Room pktRoom = RoomManager.Instance.Find(obj.objectInfo.RoomId);
        if (pktRoom != null)
        {
            pktRoom.Push(pktRoom.C_Leave_Game, pkt.ObjectId);
        }
    }

    public static void C_SyncHandler(PacketSession session, IMessage packet)
    {
        C_Sync pkt = packet as C_Sync;

        Player obj = PlayerManager.Instance.Find(pkt.ObjectInfo.ObjectId);

        // 원자성이 보장되는 영역인 Room에서 해야 됨
        Room pktRoom = RoomManager.Instance.Find(obj.objectInfo.RoomId);
        if (pktRoom != null)
        {
            pktRoom.Push(pktRoom.C_Sync, pkt);
        }
    }
}
