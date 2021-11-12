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
            // [TODO] 방 안에 있는 모든 게임 오브젝트들을 갱신
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

            switch (code)
            {
                case (ObjectCode.Player):
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

                        /**************************************************************/
                    }
                    break;
                case (ObjectCode.Monster):
                    break;
                case (ObjectCode.Arrow):
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
                Console.WriteLine($"Object({player.objectInfo.ObjectId}) Move To ({player.objectInfo.Position.X}, {player.objectInfo.Position.Y})");
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
                        Console.WriteLine($"Object({target.objectInfo.ObjectId}) got Damaged by Object({attacker.objectInfo.ObjectId})");
                        target.objectInfo.Hp -= 10; // TODO
                        if (target.objectInfo.Hp <= 0)
                        {
                            // 타겟이 죽었다고 사람들에게 알리기
                            S_Dead deadPkt = new S_Dead();
                            deadPkt.ObjectId = target.objectInfo.ObjectId;
                            BroadCast(deadPkt);
                            return;
                        }
                        else
                        {
                            // 타겟이 맞았다고 사람들에게 알리기
                            S_Attack response = new S_Attack();
                            response.TargetInfo = target.objectInfo;
                            response.AttackerInfo = attacker.objectInfo;
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

            // [TODO] 방에 있는 오브젝트의 종류에 따라 할 일을 다르게 처리해야 함.
            switch (code)
            {
                case (ObjectCode.Player):
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
                case (ObjectCode.Monster):
                    break;
                case (ObjectCode.Arrow):
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
            }
        }

        public void C_Sync(C_Sync packet)
        {
            // 패킷을 보낸 플레이어의 방 검색 후 작업
            Room pktRoom = RoomManager.Instance.Find(packet.ObjectInfo.RoomId);
            if (pktRoom != null)
            {
                // 해당 플레이어 찾고 나서 작업
                Player player = null;
                if (_players.TryGetValue(packet.ObjectInfo.ObjectId, out player) == true)
                {
                    // 플레이어 정보 업데이트
                    player.objectInfo = packet.ObjectInfo;

                    // 맵에 위치 업데이트
                    pktRoom.Map.UpdatePosition(packet.ObjectInfo.Position, player);

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
                }
            }
        }

        public void C_Spawn(C_Spawn packet)
        {
            // 패킷을 보낸 플레이어의 방 검색 후 작업
            Player p = null;
            if (_players.TryGetValue(packet.SpawnInfo.SpawnerId, out p) == true)
            {
                Room pktRoom = RoomManager.Instance.Find(packet.SpawnInfo.RoomId);
                if (pktRoom != null)
                {
                    // TODO: Code로 일일이 따지는 것이 아닌 나중에 오브젝트 종류별로 기본적인 처리를 나누고, 세부적인 것은 팩토리에서 처리 
                    switch (packet.SpawnInfo.ObjectCode)
                    {
                        case (int)ObjectCode.Arrow:
                            {
                                // arrow 소환 (TODO: 플레이어의 상태에 따라 소환가능한지 검증)
                                Arrow arrow = ObjectManager.Instance.Add<Arrow>(ObjectCode.Arrow);
                                arrow.V_SetOwner(p);

                                // 서버에서 피격판정을 계산하기 위해 방에 추가
                                _projectiles.Add(arrow.objectInfo.ObjectId, arrow);

                                // 맵에 업데이트
                                Map.UpdatePosition(arrow.objectInfo.Position, arrow);
                                
                                // 화살이 소환되었다고 패킷 보내기
                                S_Spawn spawnPkt = new S_Spawn();
                                spawnPkt.Objects.Add(arrow.objectInfo);
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
