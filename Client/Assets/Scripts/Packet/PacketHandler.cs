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
        BaseObject gameObject = Manager.Spawner.SpawnObject(enterGamePacket.Player);

        // 로컬 플레이어 지정
        Manager.ObjectManager.AddMe(gameObject);

        // 위치 초기화
        gameObject.CellPos = new Vector3Int(enterGamePacket.Player.Position.X, enterGamePacket.Player.Position.Y, 0);
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(gameObject.CellPos) + new Vector3(0.5f, 0.5f);
        gameObject.transform.position = pos;
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame pkt = packet as S_LeaveGame;

        BaseObject leaver = Manager.ObjectManager.Find(pkt.ObjectId);
        if (leaver != null)
        {
            leaver.V_Clear();
        }
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn pkt = packet as S_Spawn;

        // 서버에서 추가적으로 스폰된 애들을 소환시킨다.
        foreach (var pk in pkt.Objects)
        {
            // 오브젝트 소환
            BaseObject gameObject = Manager.Spawner.SpawnObject(new ObjectInfo(pk));

            // 오브젝트 관리자에 등록
            Manager.ObjectManager.Add(gameObject);

            // 위치 초기화
            gameObject.CellPos = new Vector3Int(pk.Position.X, pk.Position.Y, 0);
            Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(gameObject.CellPos) + new Vector3(0.5f, 0.5f);
            gameObject.transform.position = pos;

            // 플레이어 상태, 이동방향 정보 동기화
            gameObject.MoveDir = pk.MoveDir;
            gameObject.State = pk.State;
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        // 서버에서 누가누가 움직였는지에 대한 정보가 올 것임. 그것을 토대로 움직일 것
        S_Move pkt = packet as S_Move;

        foreach (var pk in pkt.Objects)
        {
            // 오브젝트가 게임 안에 있는 지 검사 후, 셀 좌표 이동
            BaseObject obj = Manager.ObjectManager.Find(pk.ObjectId);
            if (obj != null)
            {
                obj.CellPos = new Vector3Int(pk.Position.X, pk.Position.Y, 0);

                // 플레이어 상태, 이동방향 정보 동기화
                obj.MoveDir = pk.MoveDir;
                obj.State = pk.State;
            }
        }
    }

    public static void S_AttackHandler(PacketSession session, IMessage packet)
    {
        // 서버에서 어떤 애가 맞았는지에 대한 정보가 올 것임. objectId를 이용하여 관리자 클래스에서 찾아서 Hp 감소
        S_Attack pkt = packet as S_Attack;

        // 공격자 애니메이션 변동
        BaseObject attacker = Manager.ObjectManager.Find(pkt.AttackerInfo.ObjectId);
        if (attacker != null)
        {
            attacker.ObjectInfo = pkt.AttackerInfo;

            attacker.Weapon = (ObjectCode)pkt.AttackerInfo.Stat.WeaponId;
            attacker.State = pkt.AttackerInfo.State;
        }

        // 타겟 HP 감소
        BaseObject target = Manager.ObjectManager.Find(pkt.TargetInfo.ObjectId);
        if (target != null)
        {
            target.Hp = pkt.TargetInfo.Stat.Hp;
            Debug.Log($"Object({target.ObjectInfo.ObjectId}) got Damaged({pkt.Damage}) by Object({attacker.ObjectInfo.ObjectId}).\n" +
                $"Current Object({target.ObjectInfo.ObjectId}) Health:{target.Hp}");
        }
    }

    public static void S_DeadHandler(PacketSession session, IMessage packet)
    {
        S_Dead pkt = packet as S_Dead;

        BaseObject deadObj = Manager.ObjectManager.Find(pkt.ObjectId);
        if (deadObj != null)
        {
            deadObj.V_Dead();
        }
    }

    public static void S_SyncHandler(PacketSession session, IMessage packet)
    {
        // 서버에서 누가 자기를 싱크 맞춰달라고 요청이 옴
        S_Sync pkt = packet as S_Sync;

        // 오브젝트가 게임 안에 있는 지 검사 후
        BaseObject obj = Manager.ObjectManager.Find(pkt.ObjectInfo.ObjectId);
        if (obj != null)
        {
            obj.ObjectInfo = pkt.ObjectInfo;

            obj.Weapon = (ObjectCode)pkt.ObjectInfo.Stat.WeaponId;
            obj.State = pkt.ObjectInfo.State;
        }
    }

    public static void S_CreateRoomHandler(PacketSession session, IMessage packet)
    {
        // 자신이 요청한 방을 서버에 만든 뒤에 이어서 클라이언트에서도 만든다.
        S_CreateRoom pkt = packet as S_CreateRoom;

        // 맵 로드
        Manager.Map.LoadMap((MapId)pkt.RoomInfo.MapId, pkt.RoomInfo.RoomId);
    }

    public static void S_ShowRoomHandler(PacketSession session, IMessage packet)
    {
        LobbyUI lobby = UIManager.Instance.FindPopup(PopUI.Lobby) as LobbyUI;
        if (lobby != null)
        {
            lobby.ShowRoom(packet as S_ShowRoom);
        }
    }
}
