using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;
using SimpleJSON;

namespace Server
{
    public struct SpawnPosInfo
    {
        public ObjectCode code;
        public int x;
        public int y;

        public static SpawnPosInfo Zero { get => new SpawnPosInfo(); }
    }

    public class PlayerInfo
    {
        public int hp;
        public int movespeed;
        public int code;
        public int weaponId;
    }

    public class WeaponInfo
    {
        public int id;
        public int type;
        public string name;
        public int damage;
    }

    public class BowInfo
    {
        public int id;
        public int arrowspeed;
        public int damage;
    }

    public class DataManager : SingleTon<DataManager>
    {
        List<SpawnPosInfo> DungeonSpawnPosition = new List<SpawnPosInfo>();
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

        Dictionary<int, WeaponInfo> WeaponInfoMap  = new Dictionary<int, WeaponInfo>();
        Dictionary<int, BowInfo> BowInfoMap = new Dictionary<int, BowInfo>();


        public void LoadData()
        {
            Console.WriteLine("Data Loading...");
            LoadDungeonSpawnPosition();
            LoadWeaponData();
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

        void LoadWeaponData()
        {
            // 무기
            {
                List<string> weaponData = Util.GetLinesFromTableFileStream(ResourcePath.WeaponData);
                for (int i = 1; i < weaponData.Count; i++)
                {
                    string[] split = weaponData[i].Split(',');

                    WeaponInfo weaponInfo = new WeaponInfo();
                    weaponInfo.id = int.Parse(split[0]);
                    weaponInfo.type = int.Parse(split[1]);
                    weaponInfo.name = split[2];
                    weaponInfo.damage = int.Parse(split[3]);

                    WeaponInfoMap.Add(weaponInfo.id, weaponInfo);
                }
            }

            // 활
            {
                List<string> weaponData = Util.GetLinesFromTableFileStream(ResourcePath.BowData);
                for (int i = 1; i < weaponData.Count; i++)
                {
                    string[] split = weaponData[i].Split(',');

                    BowInfo weaponInfo = new BowInfo();
                    weaponInfo.id = int.Parse(split[0]);
                    weaponInfo.arrowspeed = int.Parse(split[1]);
                    weaponInfo.damage = int.Parse(split[2]);

                    BowInfoMap.Add(weaponInfo.id, weaponInfo);
                }
            }
        }

        public PlayerInfo GetPlayerData()
        {
            string txt = Util.GetLinesWithFileStream(ResourcePath.PlayerData);
            if (txt != null)
            {
                JSONNode root = JSON.Parse(txt);
                JSONNode stat = root["Stat"];

                PlayerInfo playerStat = new PlayerInfo();
                playerStat.hp = int.Parse(stat["hp"]);
                playerStat.movespeed = int.Parse(stat["movespeed"]);
                playerStat.code = int.Parse(stat["code"]);
                playerStat.weaponId = int.Parse(stat["weaponId"]);
                return playerStat;
            }
            return null;
        }

        public WeaponInfo GetWeaponData(int weaponId)
        {
            WeaponInfo ret = null;
            if (WeaponInfoMap.TryGetValue(weaponId, out ret))
            {
                return ret;
            }
            return null;
        }

        public BowInfo GetBowData(int weaponId)
        {
            BowInfo ret = null;
            if (BowInfoMap.TryGetValue(weaponId, out ret))
            {
                return ret;
            }
            return null;
        }
    }
}
