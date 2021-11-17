using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
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

                    // TODO: 몬스터 2마리 정도가 입구에서 대기
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

            Console.WriteLine($"Object({respawnCreature.creature.objectInfo.ObjectId}) will be respawn at {respawnCreature.respawnTime}");
            Respawns.Push(respawnCreature);
        }

        bool BoundCheck(Vector2 cellPos)
        {
            if (cellPos.X < MinX || cellPos.X > MaxX)
                return false;
            if (cellPos.Y < MinY || cellPos.Y > MaxY)
                return false;

            return true;
        }
    }
}
