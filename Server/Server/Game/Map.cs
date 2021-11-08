using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    public class Map
    {
        bool[,] _collision;
        BaseObject[,] _objects;

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
            Console.WriteLine("Map Loading...");
            string fileName = string.Empty;

            // 맵 마다 해야할 일을 정하자.
            switch (mapId)
            {
                case MapId.Town:
                    break;
                case MapId.Dungeon:
                    fileName = ResourcePath.DungeonCollision;

                    // [TODO] 몬스터 2마리 정도가 입구에서 대기
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
                    _objects = new BaseObject[yCount, xCount];

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

        public void UpdatePosition(Vector2 current, Vector2 next, BaseObject obj)
        {
            Vector2 currentPos = CollisionCoordinate(current.X, current.Y);
            _objects[currentPos.Y, currentPos.X] = null;

            Vector2 nextPos = CollisionCoordinate(next.X, next.Y);
            _objects[nextPos.Y, nextPos.X] = obj;
        }

        public void UpdatePosition(Vector2 next, BaseObject obj)
        {
            Vector2 collisionCoord = new Vector2(obj.objectInfo.Position);
            collisionCoord = CollisionCoordinate(collisionCoord.X, collisionCoord.Y);

            // map은 x,y가 거꾸로 되있으니 주의
            if (_objects[collisionCoord.Y, collisionCoord.X] != null)
            {
                _objects[collisionCoord.Y, collisionCoord.X] = null;
            }


            Vector2 nextPos = CollisionCoordinate(next.X, next.Y);
            
            // Map의 플레이어 위치 업데이트
            _objects[nextPos.Y, nextPos.X] = obj;

            // 플레이어 안의 ObjectInfo.Position 업데이트
            obj.objectInfo.Position = new Vector2(next);
        }

        // public void UpdatePosition(C_Move packet, BaseObject obj)

        public Vector2 CollisionCoordinate(int x, int y)
        {
            Vector2 vec = new Vector2();
            vec.X = x - MinX;
            vec.Y = MaxY - y;

            return vec;
        }
    }
}
