using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;

namespace Server
{
    public class Room : JobSerializer
    {
        public int roomId;
        public Map Map { get; private set; } = new Map();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public void Make(int mapId)
        {
            Map.LoadMap((MapId)mapId);
        }

        public void Update()
        {
            Map.RespawnUpdate();

            foreach (Projectile proj in _projectiles.Values)
            {
                proj.V_UpdateObject();
            }

            // 패킷으로 인해 오브젝트가 변동되는 일들을 처리한다.
            Flush();
        }

        public void C_EnterGame(BaseObject gameObject)
        {
            if (gameObject == null)
                return;

            // 로그
            Console.WriteLine($"Object({gameObject.objectInfo.ObjectId}) entered Room({gameObject.objectInfo.RoomId})");

            ObjectCode code = ObjectManager.GetObjectCodeById(gameObject.objectInfo.ObjectId);
            ObjectType type = ObjectFactory.GetObjectType(code);

            switch (type)
            {
                case ObjectType.OtPlayer:
                    {
                        Player player = gameObject as Player;
                        _players.Add(gameObject.objectInfo.ObjectId, player);

                        // 1. 클라이언트에게 내가 들어왔다고 알림
                        S_EnterGame enterPacket = new S_EnterGame();
                        enterPacket.Player = player.objectInfo;

                        // 맵에 업데이트
                        Map.UpdatePosition(enterPacket.Player.Position, player);

                        player.session.Send(enterPacket);

                        /**************************************************************/

                        // 2. 현재 방에 있는 플레이어들의 정보를 내가 알아야 한다
                        S_Spawn spawnPacket = new S_Spawn();
                        foreach (Player p in _players.Values)
                        {
                            if (player != p)
                            {
                                spawnPacket.Objects.Add(p.objectInfo);
                            }
                        }
                        player.session.Send(spawnPacket);
                    }
                    break;
                case ObjectType.OtMonster:
                    break;
                default:
                    break;
            }

            // 4. 방에 있는 존재들에게 내가 스폰됬다고 알린다
            {
                S_Spawn spawnPacket = new S_Spawn();

                spawnPacket.Objects.Add(gameObject.objectInfo);
                foreach (Player p in _players.Values)
                {
                    // 나를 제외한 존재들에게만..
                    if (p.objectInfo.ObjectId != gameObject.objectInfo.ObjectId) 
                    {
                        p.session.Send(spawnPacket);
                    }
                }
            }
            
        }

        public void C_Move(C_Move packet)
        {
            // 패킷을 보낸 플레이어의 방 검색 후 작업
            Room pktRoom = RoomManager.Instance.Find(packet.ObjectInfo.RoomId);
            if (pktRoom != null)
            {
                // 해당 플레이어 찾고 나서 작업
                Player player = null;
                if (_players.TryGetValue(packet.ObjectInfo.ObjectId, out player) == true)
                {
                    // 맵에 있던 기존 오브젝트는 null로 만들어야 함
                    Vector2 currentPos = player.objectInfo.Position;
                    pktRoom.Map.RemoveCreature(currentPos);

                    // 플레이어 정보 업데이트
                    player.objectInfo = packet.ObjectInfo;

                    // 맵에 위치 업데이트
                    pktRoom.Map.UpdatePosition(packet.ObjectInfo.Position, player);

                    // 방에 있는 사람들에게 알림
                    S_Move response = new S_Move();
                    response.Objects.Add(new ObjectInfo(player.objectInfo));
                    foreach (Player p in _players.Values)
                    {
                        // 나를 제외한 존재들에게만..
                        if (p.objectInfo.ObjectId != player.objectInfo.ObjectId)
                        {
                            p.session.Send(response);
                        }
                    }
                }

                // 로그
                Console.WriteLine($"Object({player.objectInfo.ObjectId}) move To ({player.objectInfo.Position.X}, {player.objectInfo.Position.Y})");
            }
        }

        public void C_Attack(C_Attack packet)
        {
            // 패킷을 보낸 플레이어의 방 검색 후 작업
            Room pktRoom = RoomManager.Instance.Find(packet.AttackerInfo.RoomId);
            if (pktRoom != null)
            {
                // TODO: 플레이어뿐만 아니라 임의의 오브젝트에 대해서 공격판정을 가능케 하기
                Player attacker = null;
                if (_players.TryGetValue(packet.AttackerInfo.ObjectId, out attacker) == true)
                {
                    // 공격자 정보 업데이트
                    attacker.objectInfo = packet.AttackerInfo;

                    // 검증: 공격자 앞에 뭔가 있는가?
                    if (Map.IsCreatureAt(attacker.GetFrontCellPos()))
                    {
                        // 공격자 앞의 타겟
                        Creature target = Map.CreatureAt(attacker.GetFrontCellPos());

                        // 체력깎기
                        Console.WriteLine($"Object({target.objectInfo.ObjectId}) got Damaged({attacker.Damage}) by Object({attacker.objectInfo.ObjectId})");
                        target.objectInfo.Stat.Hp -= attacker.Damage; // TODO 데미지 공식
                        if (target.objectInfo.Stat.Hp <= 0)
                        {
                            // 타겟이 죽었다고 사람들에게 알리기
                            S_Dead deadPkt = new S_Dead();
                            deadPkt.ObjectId = target.objectInfo.ObjectId;
                            BroadCast(deadPkt);

                            // 죽은 플레이어는 방에는 존재하지만 맵에는 존재하지 않아야 한다.
                            Map.RemoveCreature(target.objectInfo.Position);

                            // 죽은 플레이어는 리스폰 큐에 들어간다
                            Map.AddRespawn(target);

                            // 공격자의 애니메이션 동기화
                            S_Sync response = new S_Sync();
                            response.ObjectInfo = attacker.objectInfo;
                            foreach (Player p in _players.Values)
                            {
                                // 나를 제외한 존재들에게만..
                                if (p.objectInfo.ObjectId != attacker.objectInfo.ObjectId)
                                {
                                    p.session.Send(response);
                                }
                            }
                            return;
                        }
                        else
                        {
                            // 타겟이 맞았다고 사람들에게 알리기
                            S_Attack response = new S_Attack();
                            response.TargetInfo = target.objectInfo;
                            response.AttackerInfo = attacker.objectInfo;
                            response.Damage = attacker.Damage;
                            BroadCast(response);
                        }
                    }
                    else
                    {
                        // 아무도 없는 데 때린거면 그냥 동기화만 해줌
                        S_Sync response = new S_Sync();
                        response.ObjectInfo = attacker.objectInfo;
                        foreach (Player p in _players.Values)
                        {
                            // 나를 제외한 존재들에게만..
                            if (p.objectInfo.ObjectId != attacker.objectInfo.ObjectId)
                            {
                                p.session.Send(response);
                            }
                        }
                    }
                }
            }
        }

        public void C_LeaveGame(int objectId)
        {
            ObjectCode code = ObjectManager.GetObjectCodeById(objectId);
            ObjectType type = ObjectFactory.GetObjectType(code);

            switch (type)
            {
                case ObjectType.OtPlayer:
                    {
                        // 방에서 삭제
                        _players.Remove(objectId);

                        // 오브젝트 관리자에서 삭제
                        ObjectManager.Instance.Remove(objectId);

                        // 모두에게 알림
                        S_LeaveGame pkt = new S_LeaveGame();
                        pkt.ObjectId = objectId;
                        BroadCast(pkt);
                    }
                    break;
                case ObjectType.OtMonster:
                    break;
                case ObjectType.OtProjectile:
                    {
                        // 방에서 삭제
                        _projectiles.Remove(objectId);

                        // 오브젝트 관리자에서 삭제
                        ObjectManager.Instance.Remove(objectId);

                        // 모두에게 알림
                        S_LeaveGame pkt = new S_LeaveGame();
                        pkt.ObjectId = objectId;
                        BroadCast(pkt);
                    }
                    break;
                case ObjectType.OtNone:
                    break;
            }
        }

        public void C_Sync(C_Sync packet)
        {
            // 패킷을 보낸 플레이어의 방 검색 후 작업
            Room pktRoom = RoomManager.Instance.Find(packet.ObjectInfo.RoomId);
            if (pktRoom != null)
            {
                ObjectCode code = ObjectManager.GetObjectCodeById(packet.ObjectInfo.ObjectId);
                ObjectType type = ObjectFactory.GetObjectType(code);

                switch (type)
                {
                    case ObjectType.OtPlayer:
                        {
                            // 해당 플레이어 찾고 나서 작업
                            Player player = null;
                            if (_players.TryGetValue(packet.ObjectInfo.ObjectId, out player) == true)
                            {
                                // 플레이어 정보 업데이트
                                player.objectInfo = packet.ObjectInfo;

                                // 방에 있는 사람들에게 알림
                                S_Sync response = new S_Sync();
                                response.ObjectInfo = player.objectInfo;
                                foreach (Player p in _players.Values)
                                {
                                    // 나를 제외한 존재들에게만..
                                    if (p.objectInfo.ObjectId != player.objectInfo.ObjectId)
                                    {
                                        p.session.Send(response);
                                    }
                                }

                                // 로그
                                Console.WriteLine($"Player({player.objectInfo.ObjectId}) wants to sync.");
                            }
                        }
                        break;
                    case ObjectType.OtMonster:
                        break;
                    case ObjectType.OtProjectile:
                        break;
                    case ObjectType.OtNone:
                        break;
                    default:
                        break;
                }
            }
        }

        public void C_Spawn(C_Spawn packet)
        {
            // 패킷을 보낸 플레이어의 방 검색 후 작업
            Player spawner = null;
            if (_players.TryGetValue(packet.SpawnInfo.SpawnerId, out spawner) == true)
            {
                Room pktRoom = RoomManager.Instance.Find(packet.SpawnInfo.RoomId);
                if (pktRoom != null)
                {
                    ObjectCode code = (ObjectCode)packet.SpawnInfo.ObjectCode;
                    ObjectType type = ObjectFactory.GetObjectType(code);

                    switch (type)
                    {
                        // 플레이어를 소환하는 경우는 리스폰 요청
                        case ObjectType.OtPlayer:
                            {
                                // 스폰 위치 정하기
                                spawner.objectInfo.Position = DataManager.Instance.SpawnData.GetRandomPosition(Map.Id);

                                // 맵에 업데이트
                                Map.UpdatePosition(spawner.objectInfo.Position, spawner);

                                // 방 안의 모든 플레이어들에게 알리기
                                S_Spawn spawnPacket = new S_Spawn();
                                spawnPacket.Objects.Add(spawner.objectInfo);
                                BroadCast(spawnPacket);
                            }
                            break;

                        case ObjectType.OtProjectile:
                            {
                                Projectile proj = ProjectileFactory.GetProjectile(code, spawner);

                                // 서버에서 피격판정을 계산하기 위해 방에 추가
                                _projectiles.Add(proj.objectInfo.ObjectId, proj);

                                // 투사체가 소환되었다고 패킷 보내기
                                S_Spawn spawnPkt = new S_Spawn();
                                spawnPkt.Objects.Add(proj.objectInfo);
                                BroadCast(spawnPkt);
                            }
                            break;
                    }
                }
            }
        }

        public void BroadCast(IMessage packet)
        {
            foreach (Player player in _players.Values)
            {
                player.session.Send(packet);
            }
        }
    }
}
