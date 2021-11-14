using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Creature : BaseObject
    {
        public int Hp { get => objectInfo.Stat.Hp; set { objectInfo.Stat.Hp = value; } }
        public int Movespeed { get => objectInfo.Stat.Movespeed; set { objectInfo.Stat.Movespeed = value; } }
        public int Damage { get => DataManager.Instance.GetWeaponData(objectInfo.Stat.WeaponId).damage;}
    }
}
