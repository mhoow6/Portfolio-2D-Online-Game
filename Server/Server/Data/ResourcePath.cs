using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public struct ResourcePath
    {
        public readonly static string DungeonSpawnPosition = "../../../../../Common/SpawnPosition/Map_Dungeon.txt";
        public readonly static string DungeonCollision = "../../../../../Common/Collision/Map_Dungeon.txt";
        public readonly static string PlayerData = "../../../../../Common/Data/PlayerData.json";
        public readonly static string WeaponData = "../../../../../Common/Data/WeaponData.csv";
        public readonly static string BowData = "../../../../../Common/Data/BowData.csv";
        public readonly static string AoniData = "../../../../../Common/Data/AoniData.json";
    }
}
