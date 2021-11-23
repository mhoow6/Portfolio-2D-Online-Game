using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

namespace Server
{
    public class Room : JobSerializer
    {
        public int roomId;
        public int PlayerCount { get => _players.Count; }

        public Map Map { get; private set; } = new Map();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        Dictionary<int, Aoni> _aonis = new Dictionary<int, Aoni>();

        public void Make(int mapId)
        {
            Map.LoadMap((MapId)mapId);
            _aonis = Map.LoadAoni(Map.Id, this);
        }

        public void Update()
        {
            Map.RespawnUpdate();

            foreach (Aoni aoni in _aonis.Values)
            {
                aoni.V_UpdateObject();
            }

            foreach (Projectile proj in _projectiles.Values)
            {
                proj.V_UpdateObject();
            }

            // 패킷으로 인해 오브젝트가 변동되는 일들을 처리한다.
            Flush();
        }

        public Player FindNearestPlayer(int searchRange, Vector2 pivotPos)
        {
            Player nearest = null;
            float minDistance = float.MaxValue;

            foreach (Player p in _players.Values)
            {
                Vector2 dis = Vector2Helper.Minus(p.objectInfo.Position, pivotPos);
                float distance = Vector2Helper.PowMagnitude(dis);

                if (distance < MathF.Pow(searchRange, 2)) // 제곱근 연산 방지
                {
                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        nearest = p;
                    }
                }
                else
                {
                    continue;
                }
            }

            return nearest;
        }

        #region 패킷
        public void C_EnterGame(C_EnterGame packet, PacketSession session)
        {
            PlayerInfo playerStat = DataManager.Instance.GetPlayerData();
            ClientSession client = session as ClientSession;

            // 이 클라이언트는 더 이상 로비에 있지 않다.
            SessionManager.Instance.OutLobby(client);

            // 오브젝트 기본 정보 초기화
            Player player = ObjectManager.Instance.Add<Player>((ObjectCode)playerStat.code);
            {
                player.session = client;

                // 랜덤 스폰 장소
                player.objectInfo.Position = DataManager.Instance.SpawnData.GetRandomPosition((MapId)packet.RoomInfo.MapId);

                // 방 번호
                Room room = this;
                player.room = room;
                player.objectInfo.RoomId = packet.RoomInfo.RoomId;

                // 기본 상태, 이동방향
                player.objectInfo.MoveDir = MoveDir.Up;
                player.objectInfo.State = State.Idle;

                // 게임에서 쓰일 스텟정보 초기화
                player.objectInfo.Stat = new StatInfo() { Hp = playerStat.hp, Movespeed = playerStat.movespeed, WeaponId = playerStat.weaponId, OriginHp = playerStat.hp };

                // 방의 players에 추가
                _players.Add(player.objectInfo.ObjectId, player);
            }  

            /*******************************************************************/

            // 자기 자신 클라이언트에게 내가 들어왔다고 알림
            {
                S_EnterGame enterPacket = new S_EnterGame();
                enterPacket.Player = player.objectInfo;

                // 맵에 업데이트
                Map.UpdatePosition(enterPacket.Player.Position, player);

                player.session.Send(enterPacket);
            }            

            /*******************************************************************/

            // 현재 방에 있는 정보를 내가 알아야 한다
            {
                S_Spawn spawnPacket = new S_Spawn();
                
                // 플레이어들
                foreach (Player p in _players.Values)
                {
                    if (player != p)
                    {
                        spawnPacket.Objects.Add(p.objectInfo);
                    }
                }

                // 몬스터들
                foreach (Aoni aoni in _aonis.Values)
                {
                    spawnPacket.Objects.Add(aoni.objectInfo);
                }

                player.session.Send(spawnPacket);
            }

            /********************************************************************/

            // 방에 있는 존재들에게 내가 스폰됬다고 알린다
            {
                S_Spawn spawnPacket = new S_Spawn();

                spawnPacket.Objects.Add(player.objectInfo);
                foreach (Player p in _players.Values)
                {
                    // 나를 제외한 존재들에게만..
                    if (p.objectInfo.ObjectId != player.objectInfo.ObjectId)
                    {
                        p.session.Send(spawnPacket);
                    }
                }
            }

            Console.WriteLine($"Object({player.objectInfo.ObjectId}) entered Room({player.objectInfo.RoomId})");
        }

        public void C_LeaveGame(int objectId)
        {
            ObjectCode code = ObjectManager.GetObjectCodeById(objectId);
            ObjectType type = ObjectFactory.GetObjectType(code);

            switch (type)
            {
                case ObjectType.OtPlayer:
                    {
                        // 맵에서 삭제
                        Player leaver = null;
                        if (_players.TryGetValue(objectId, out leaver))
                        {
                            Map.RemoveCreature(leaver.objectInfo.Position);
                        }

                        // 방에서 삭제
                        _players.Remove(objectId);

                        // 오브젝트 관리자에서 삭제
                        ObjectManager.Instance.Remove(objectId);

                        // 모두에게 알림
                        S_LeaveGame pkt = new S_LeaveGame();
                        pkt.ObjectId = objectId;
                        BroadCast(pkt);

                        // 방에 아무도 없다면 방 폭파
                        if (_players.Count == 0)
                        {
                            RoomManager.Instance.Remove(roomId);
                            Console.WriteLine($"Room:{roomId} Removed.");
                        }
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

                    // 공격자 앞의 타겟
                    Creature target = null;

                    // 검증: 공격자 앞에 뭔가 있는가?
                    if (Map.IsCreatureAt(attacker.GetFrontCellPos()))
                    {
                        target = Map.CreatureAt(attacker.GetFrontCellPos());

                        // 데미지 판정
                        AttackHelper.Attack(attacker, target, () =>
                        {
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
                        });
                        return;
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

        public void C_CreateRoom(PacketSession session)
        {
            ClientSession client = session as ClientSession;
            S_CreateRoom response = new S_CreateRoom() { RoomInfo = new RoomInfo() };
            response.RoomInfo.MapId = (int)Map.Id;
            response.RoomInfo.RoomId = roomId;
            client.Send(response);
        }

        public void BroadCast(IMessage packet)
        {
            foreach (Player player in _players.Values)
            {
                player.session.Send(packet);
            }
        }
        #endregion
    }
}
