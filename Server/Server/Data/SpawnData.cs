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
        public DungeonSpawnData DungeonSpawnData { get; private set; }

        public SpawnData()
        {
            LoadData();
        }

        public void LoadData()
        {
            DungeonSpawnData = new DungeonSpawnData();
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
                        rndIndex = rnd.Next(0, DungeonSpawnData.DungeonPlayerSpawnPosition.Count - 1);
                        pos = DungeonSpawnData.DungeonPlayerSpawnPosition[rndIndex];
                        Vector2 position = new Vector2();
                        position.X = pos.x;
                        position.Y = pos.y;
                        return position;
                    }
            }

            return null;
        }
    }

    public class DungeonSpawnData
    {
        List<SpawnPosInfo> DungeonSpawnPosition = new List<SpawnPosInfo>();
        public List<SpawnPosInfo> DungeonPlayerSpawnPosition { get; private set; } = new List<SpawnPosInfo>();
        public List<SpawnPosInfo> DungeonAoniSpawnPosition { get; private set; } = new List<SpawnPosInfo>();

        public DungeonSpawnData()
        {
            List<string> text = Util.GetLinesFromTableFileStream(ResourcePath.DungeonSpawnPosition);

            for (int i = 1; i < text.Count; i++)
            {
                string[] datas = text[i].Split(',');

                SpawnPosInfo info;

                info.type = (ObjectType)int.Parse(datas[0]);
                info.x = int.Parse(datas[1]);
                info.y = int.Parse(datas[2]);

                switch (info.type)
                {
                    case ObjectType.OtPlayer:
                        DungeonPlayerSpawnPosition.Add(info);
                        break;
                    case ObjectType.OtMonster:
                        DungeonAoniSpawnPosition.Add(info);
                        break;
                }
                DungeonSpawnPosition.Add(info);
            }


        }
    }
}
