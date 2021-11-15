using System;
using System.Collections.Generic;
using System.Text;


namespace Server
{
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

    public class WeaponData : ILoader
    {
        Dictionary<int, WeaponInfo> WeaponInfoMap = new Dictionary<int, WeaponInfo>();
        Dictionary<int, BowInfo> BowInfoMap = new Dictionary<int, BowInfo>();

        public WeaponData()
        {
            LoadData();
        }

        public void LoadData()
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


