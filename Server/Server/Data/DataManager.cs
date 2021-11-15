using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;
using SimpleJSON;

namespace Server
{
    public interface ILoader
    {
        void LoadData();
    }

    public class PlayerInfo
    {
        public int hp;
        public int movespeed;
        public int code;
        public int weaponId;
    }

    public class DataManager : SingleTon<DataManager>, ILoader
    {
        public SpawnData SpawnData { get; private set; } 
        public WeaponData WeaponData { get; private set; } 

        public void LoadData()
        {
            Console.WriteLine("Data Loading..");

            SpawnData = new SpawnData();
            WeaponData = new WeaponData();

            Console.WriteLine("Data Load Completed.");
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
    }
}
