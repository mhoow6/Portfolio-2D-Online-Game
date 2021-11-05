using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server
{
    public struct SpawnPosInfo
    {
        public ObjectCode code;
        public int x;
        public int y;
    }

    public class DataManager : SingleTon<DataManager>
    {
        public List<SpawnPosInfo> DungeonSpawnPosition { get; private set; } = new List<SpawnPosInfo>();
        public List<SpawnPosInfo> DungeonPlayerSpawnPosition
        {
            get
            {
                List<SpawnPosInfo> list = new List<SpawnPosInfo>();

                foreach (var item in DungeonSpawnPosition)
                {
                    if (item.code == ObjectCode.Player)
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
                    if (item.code == ObjectCode.Monster)
                        list.Add(item);
                }

                return list;
            }
        }


        public void LoadData()
        {
            Console.WriteLine("Data Loading...");
            LoadDungeonSpawnPosition();

        }

        void LoadDungeonSpawnPosition()
        {
            List<string> text = Util.GetLinesFromTableFileStream(ResourcePath.DungeonSpawnPosition);

            for (int i = 1; i < text.Count; i++)
            {
                string[] datas = text[i].Split(',');

                SpawnPosInfo info;

                // 첫 번째 글자만 대문자로
                datas[0] = datas[0].ToUpper();
                string lowercase = datas[0].Substring(1).ToLower();
                datas[0] = datas[0].Replace(datas[0].Substring(1), lowercase);

                info.code = (ObjectCode)Enum.Parse(typeof(ObjectCode), datas[0]);
                info.x = int.Parse(datas[1]);
                info.y = int.Parse(datas[2]);

                DungeonSpawnPosition.Add(info);
            }
        }
    }
}
