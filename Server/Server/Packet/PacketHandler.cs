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

        // 원자성이 보장되는 영역인 Room에서 해야 됨
        Room pktRoom = RoomManager.Instance.Find(pkt.SpawnInfo.RoomId);
        if (pktRoom != null)
        {
            pktRoom.Push(pktRoom.C_Spawn, pkt);
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

    //C_EnterGameHandler
    public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        C_EnterGame pkt = packet as C_EnterGame;

        Room pktRoom = RoomManager.Instance.Find(pkt.RoomInfo.RoomId);
        if (pktRoom != null)
        {
            pktRoom.Push(pktRoom.C_EnterGame, pkt, session);
        }
        
    }

    public static void C_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        C_LeaveGame pkt = packet as C_LeaveGame;

        Player obj = ObjectManager.Instance.Find(pkt.ObjectId) as Player;

        // 원자성이 보장되는 영역인 Room에서 해야 됨
        Room pktRoom = RoomManager.Instance.Find(obj.objectInfo.RoomId);
        if (pktRoom != null)
        {
            pktRoom.Push(pktRoom.C_LeaveGame, pkt.ObjectId);
        }
    }

    public static void C_SyncHandler(PacketSession session, IMessage packet)
    {
        C_Sync pkt = packet as C_Sync;

        Player obj = ObjectManager.Instance.Find(pkt.ObjectInfo.ObjectId) as Player;

        // 원자성이 보장되는 영역인 Room에서 해야 됨
        Room pktRoom = RoomManager.Instance.Find(obj.objectInfo.RoomId);
        if (pktRoom != null)
        {
            pktRoom.Push(pktRoom.C_Sync, pkt);
        }
    }

    public static void C_CreateRoomHandler(PacketSession session, IMessage packet)
    {
        C_CreateRoom pkt = packet as C_CreateRoom;

        // RoomManager 안에서 락을 걸고 방을 만들기 때문에 100% 다른 방이 나옴
        Room room = RoomManager.Instance.Add(pkt.MapId); 
        if (room != null)
        {
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine($"Room:{room.roomId} initalize Map {room.Map.Id}");
            Program.TickRoom(room, 50); // 50ms마다 실행

            room.Push(room.C_CreateRoom, session);
        }
    }
}
