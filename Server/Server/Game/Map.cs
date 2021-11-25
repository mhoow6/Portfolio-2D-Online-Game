using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Server
{
    public class RespawnCreature : IComparable<RespawnCreature>
    {
        public Creature creature;
        public int respawnTime;

        public int CompareTo(RespawnCreature creature)
        {
            if (this.respawnTime == creature.respawnTime)
                return 0;

            // <0 lhs < rhs     0 lhs = rhs     1 lhs > rhs
            return this.respawnTime < creature.respawnTime ? -1 : 1;
        }
    }

    public class Map
    {
        public MapId Id { get; private set; }
        bool[,] _collision;
        Creature[,] _objects;
        public PriorityQueue<RespawnCreature> Respawns { get; private set; }
        = new PriorityQueue<RespawnCreature>();

        public int XLength
        {
            get
            {
                return Math.Abs(MinX) + Math.Abs(MaxX);
            }
        }

        public int YLength
        {
            get
            {
                return Math.Abs(MinY) + Math.Abs(MaxY);
            }
        }


        public int MinX { get; private set; }
        public int MaxX { get; private set; }
        public int MinY { get; private set; }
        public int MaxY { get; private set; }


        public void LoadMap(MapId mapId)
        {
            string fileName = string.Empty;
            Id = mapId;

            // 맵 마다 해야할 일을 정하자.
            switch (mapId)
            {
                case MapId.Dungeon:
                    fileName = ResourcePath.DungeonCollision;
                    break;
            }

            if (fileName != string.Empty)
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    MinX = int.Parse(sr.ReadLine());
                    MaxX = int.Parse(sr.ReadLine());
                    MinY = int.Parse(sr.ReadLine());
                    MaxY = int.Parse(sr.ReadLine());

                    int xCount = MaxX - MinX + 1;
                    int yCount = MaxY - MinY + 1;
                    _collision = new bool[yCount, xCount];
                    _objects = new Creature[yCount, xCount];

                    // collision: 왼쪽 아래에서 오른쪽 위로 순회
                    for (int y = 0; y < yCount; y++)
                    {
                        string line = sr.ReadLine();
                        for (int x = 0; x < xCount; x++)
                        {
                            _collision[y, x] = (line[x] == '1' ? true : false);
                        }
                    }
                }
            }
            
        }

        public Dictionary<int, Aoni> LoadAoni(MapId mapId, Room aoniRoom)
        {
            Dictionary<int, Aoni> ret = new Dictionary<int, Aoni>();
            switch (mapId)
            {
                case MapId.Dungeon:
                    // TEMP: 아오오니 1명만 배치
                    var temp = DataManager.Instance.SpawnData.DungeonSpawnData.DungeonAoniSpawnPosition;
                    temp.RemoveAt(1);

                    foreach (var spawnpos in temp)
                    {
                        // 아오오니 생성
                        Aoni aoni = ObjectManager.Instance.Add<Aoni>(ObjectCode.Aoni);
                        aoni.room = aoniRoom;

                        // 아오오니 스텟
                        aoni.objectInfo.Stat = MonsterFactory.GetAoniInfo();
                        aoni.objectInfo.MoveDir = MoveDir.Down;
                        aoni.objectInfo.State = State.Idle;

                        // 스폰 위치
                        Vector2 aoniPos = new Vector2();
                        aoniPos.X = spawnpos.x;
                        aoniPos.Y = spawnpos.y;
                        aoni.objectInfo.Position = aoniPos;

                        // 맵에 업데이트
                        UpdatePosition(aoniPos, aoni);

                        // 반환값에 추가
                        ret.Add(aoni.objectInfo.ObjectId, aoni);
                    }
                    return ret;
            }

            return null;
        }

        public void UpdatePosition(Vector2 next, Creature obj)
        {
            Vector2 collisionCoord = new Vector2(obj.objectInfo.Position);
            collisionCoord = CollisionCoordinate(collisionCoord.X, collisionCoord.Y);

            // map은 x,y가 거꾸로 되있으니 주의
            if (_objects[collisionCoord.Y, collisionCoord.X] == obj)
            {
                _objects[collisionCoord.Y, collisionCoord.X] = null;
            }

            Vector2 nextPos = CollisionCoordinate(next.X, next.Y);
            
            // Map의 플레이어 위치 업데이트
            _objects[nextPos.Y, nextPos.X] = obj;

            // 플레이어 안의 ObjectInfo.Position 업데이트
            obj.objectInfo.Position = new Vector2(next);
        }

        public Vector2 CollisionCoordinate(int x, int y)
        {
            Vector2 vec = new Vector2();
            vec.X = x - MinX;
            vec.Y = MaxY - y;

            return vec;
        }

        public bool CanGo(Vector2 cellPos)
        {
            if (BoundCheck(cellPos) == false)
                return false;

            // 셀 좌표계 -> 맵을 이진수로 표현한 배열 좌표계
            Vector2 vec = CollisionCoordinate(cellPos.X, cellPos.Y);

            if (_collision[vec.Y, vec.X] == true)
            {
                return false;
            }

            if (_objects[vec.Y, vec.X] != null)
            {
                return false;
            }

            return true;
        }

        public bool CanGo(Vector2 cellPos, bool ignoreObject = true)
        {
            if (BoundCheck(cellPos) == false)
                return false;

            // 셀 좌표계 -> 맵을 이진수로 표현한 배열 좌표계
            Vector2 vec = CollisionCoordinate(cellPos.X, cellPos.Y);

            if (_collision[vec.Y, vec.X] == true)
            {
                return false;
            }

            if (ignoreObject == false)
            {
                if (_objects[vec.Y, vec.X] != null)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsCreatureAt(Vector2 cellPos)
        {
            if (BoundCheck(cellPos) == false)
                return false;

            Vector2 vec = CollisionCoordinate(cellPos.X, cellPos.Y);

            if ((_objects[vec.Y, vec.X] != null))
            {
                return true;
            }

            return false;
        }

        public Creature CreatureAt(Vector2 cellPos)
        {
            if (BoundCheck(cellPos) == false)
                return null;

            // 셀 좌표계 -> 맵을 이진수로 표현한 배열 좌표계
            Vector2 vec = CollisionCoordinate(cellPos.X, cellPos.Y);

            if (_objects[vec.Y, vec.X] != null)
            {
                return _objects[vec.Y, vec.X];
            }

            return null;
        }

        public bool RemoveCreature(Vector2 cellPos)
        {
            Vector2 vec = CollisionCoordinate(cellPos.X, cellPos.Y);

            if (_objects[vec.Y, vec.X] != null)
            {
                _objects[vec.Y, vec.X] = null;
                return true;
            }

            return false;
        }

        bool BoundCheck(Vector2 cellPos)
        {
            if (cellPos.X < MinX || cellPos.X > MaxX)
                return false;
            if (cellPos.Y < MinY || cellPos.Y > MaxY)
                return false;

            return true;
        }

        public void RespawnUpdate()
        {
            if (Respawns.Peek() != null)
            {
                if (Respawns.Peek().respawnTime < System.Environment.TickCount)
                {
                    Creature respawn = Respawns.Pop().creature;
                    ObjectCode code = (ObjectCode)respawn.objectInfo.ObjectCode;
                    ObjectType type = ObjectFactory.GetObjectType(code);

                    switch (type)
                    {
                        case ObjectType.OtPlayer:
                            {
                                // 스폰 위치 정하기
                                respawn.objectInfo.Position = DataManager.Instance.SpawnData.GetRandomPosition(Id);

                                // HP 복구
                                respawn.objectInfo.Stat.Hp = 100;

                                // 맵에 업데이트
                                UpdatePosition(respawn.objectInfo.Position, respawn);

                                // 방 안의 모든 플레이어들에게 알리기
                                Console.WriteLine($"Object({respawn.objectInfo.ObjectId}) Respawn!");
                                S_Spawn spawnPacket = new S_Spawn();
                                spawnPacket.Objects.Add(respawn.objectInfo);
                                respawn.room.Push(respawn.room.BroadCast, spawnPacket);
                            }
                            break;
                    }
                }
            }
        }

        public void AddRespawn(Creature creature)
        {
            int tick = ObjectFactory.GetRespawnTime((ObjectCode)creature.objectInfo.ObjectCode);

            RespawnCreature respawnCreature = new RespawnCreature();
            respawnCreature.creature = creature;
            respawnCreature.respawnTime = System.Environment.TickCount + tick;

            DateTime cur = DateTime.Now;
            DateTime respawnTime = cur.AddSeconds(tick / 1000);

            Console.WriteLine($"Object({respawnCreature.creature.objectInfo.ObjectId}) will be respawn at [{respawnTime}]");
            Respawns.Push(respawnCreature);
        } 

        // A*
        public List<Vector2> FindPath(Vector2 startPos, Vector2 destPos, bool ignoreObject = true)
        {
            // 최종적으로 생성할 길
            List<Vector2> path = new List<Vector2>();
            // 비교 평가를 위한 딕셔너리
            Dictionary<Vector2, AstarCapsule> openMap = new Dictionary<Vector2, AstarCapsule>();
            // 평가할 노드 큐
            PriorityQueue<AstarCapsule> openQueue = new PriorityQueue<AstarCapsule>();
            // 해당 좌표가 어느 좌표로부터 왔는지
            Vector2[,] whereIcome = new Vector2[XLength, YLength];

            // 1. 시작 지점은 이미 다녀온 길
            openMap.Add(startPos, new AstarCapsule() { X = startPos.X, Y = startPos.Y, G = 0, H = 0});
            openQueue.Push(openMap[startPos]);
            
            // 2. Astar 시작
            int[] yCheck = new int[4] { -1, 0, 1, 0 }; // UP LEFT DOWN RIGHT
            int[] xCheck = new int[4] { 0, -1, 0, 1 };
            int[] cost = new int[4] { 1, 1, 1, 1 };
            while (openQueue.Count != 0)
            {
                // F값; 목적지까지 거리가 낮은 후보 추출
                AstarCapsule node = openQueue.Pop();
                // 2-1. 갈 수 있는 길 4 방향 검사
                for (int i = 0; i < 4; i++)
                {
                    Vector2 target = new Vector2() { X = node.X + xCheck[i], Y = node.Y + yCheck[i] };
                    // 2-1-1. 해당 방향으로 갈 수 있다면?
                    if (CanGo(target, ignoreObject))
                    {
                        AstarCapsule capsule = null;
                        // 최초 방문지역이라면?
                        if (openMap.TryGetValue(target, out capsule) == false)
                        {
                            capsule = new AstarCapsule();
                            capsule.X = target.X;
                            capsule.Y = target.Y;
                            capsule.G = node.G + cost[i];
                            capsule.H = Heuristics(target, destPos);
                            // 데이터 추가
                            openMap.Add(target, capsule);
                            openQueue.Push(capsule);
                            Vector2 collisionCoordinate = CollisionCoordinate(target.X, target.Y);
                            whereIcome[collisionCoordinate.X, collisionCoordinate.Y] = new Vector2() { X = node.X, Y = node.Y };
                        }
                        // 이미 방문했다면 지금 내가 있는 위치와 거리(F)를 비교해본다
                        else
                        {
                            AstarCapsule candidate = new AstarCapsule();
                            candidate.X = target.X;
                            candidate.Y = target.Y;
                            candidate.G = node.G + cost[i];
                            candidate.H = Heuristics(target, destPos);
                            // 지금 내가 있는 위치가 더 목적지에 가깝다면 교체
                            if (candidate.F < capsule.F)
                            {
                                // 데이터 추가
                                openMap[target] = candidate;
                                openQueue.Push(candidate);
                                Vector2 collisionCoordinate = CollisionCoordinate(target.X, target.Y);
                                whereIcome[collisionCoordinate.X, collisionCoordinate.Y] = new Vector2() { X = node.X, Y = node.Y };
                            }
                        }
                    }
                }
            }

            // 3. 길 만들기
            {
                Vector2 collisionCoordinate = CollisionCoordinate(destPos.X, destPos.Y);
                Vector2 parent = whereIcome[collisionCoordinate.X, collisionCoordinate.Y];
                while (parent != null)
                {
                    path.Add(parent);
                    Vector2 coord = CollisionCoordinate(parent.X, parent.Y);
                    parent = whereIcome[coord.X, coord.Y];
                }
            }

            // 4. 길 뒤집기
            path.Reverse();
            path.RemoveAt(0); // 자신의 위치 제외

            return path;
        }

        int Heuristics(Vector2 startPos, Vector2 destPos)
        {
            int result = -1;

            int xHeuristic = destPos.X - startPos.X;
            int xAbs = xHeuristic > 0 ? xHeuristic : -xHeuristic;

            int yHeuristic = destPos.Y - startPos.Y;
            int yAbs = yHeuristic > 0 ? yHeuristic : -yHeuristic;

            result = xAbs + yAbs;

            return result;
        }
    }
}

class AstarCapsule : IComparable<AstarCapsule>
{
    public int F { get => G + H; }
    public int G;
    public int H;
    public int X;
    public int Y;

    public int CompareTo([AllowNull] AstarCapsule other)
    {
        if (F == other.F)
            return 0;

        return F < other.F ? 1 : -1;
    }
}
