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

    public class DataManager
    {
        public static DataManager Instance { get; private set; } = new DataManager();
        public List<SpawnPosInfo> DungeonSpawnPosition { get; private set; }

        public void LoadData()
        {
            Console.WriteLine("Data Loading...");
            List<string> text = Util.GetLinesFromTableFileStream("../../../../../Common/SpawnPosition/Map_Dungeon.txt");

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
