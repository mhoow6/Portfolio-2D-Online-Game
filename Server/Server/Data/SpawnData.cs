using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public struct SpawnPosInfo
    {
        public ObjectType type;
        public int x;
        public int y;

        public static SpawnPosInfo Zero { get => new SpawnPosInfo(); }
    }

    public class SpawnData : ILoader
    {
        List<SpawnPosInfo> DungeonSpawnPosition = new List<SpawnPosInfo>();
        public List<SpawnPosInfo> DungeonPlayerSpawnPosition
        {
            get
            {
                List<SpawnPosInfo> list = new List<SpawnPosInfo>();

                foreach (var item in DungeonSpawnPosition)
                {
                    if (ObjectType.OtPlayer == item.type)
                        list.Add(item);
                }

                return list;
            }
        }
        public List<SpawnPosInfo> DungeonMonsterSpawnPosition
        {
            get
            {
                List<SpawnPosInfo> list = new List<SpawnPosInfo>();

                foreach (var item in DungeonSpawnPosition)
                {
                    if (ObjectType.OtMonster == item.type)
                        list.Add(item);
                }

                return list;
            }
        }

        public SpawnData()
        {
            LoadData();
        }

        public void LoadData()
        {
            List<string> text = Util.GetLinesFromTableFileStream(ResourcePath.DungeonSpawnPosition);

            for (int i = 1; i < text.Count; i++)
            {
                string[] datas = text[i].Split(',');

                SpawnPosInfo info;

                info.type = (ObjectType)int.Parse(datas[0]);
                info.x = int.Parse(datas[1]);
                info.y = int.Parse(datas[2]);

                DungeonSpawnPosition.Add(info);
            }
        }

        public Vector2 GetRandomPosition(MapId mapId)
        {
            switch (mapId)
            {
                case MapId.Dungeon:
                    {
                        // 랜덤 스폰 장소
                        Random rnd = new Random(System.Environment.TickCount);
                        int rndIndex = -1;
                        SpawnPosInfo pos = SpawnPosInfo.Zero;
                        rndIndex = rnd.Next(0, DataManager.Instance.SpawnData.DungeonPlayerSpawnPosition.Count - 1);
                        pos = DataManager.Instance.SpawnData.DungeonPlayerSpawnPosition[rndIndex];
                        Vector2 position = new Vector2();
                        position.X = pos.x;
                        position.Y = pos.y;
                        return position;
                    }
            }

            return null;
        }
    }
}
