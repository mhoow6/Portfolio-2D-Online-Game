using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseObject
{
    public Creature _owner;

    public void SetOwner(int spawnerId) { _owner = Manager.ObjectManager.Find(ObjectInfo.SpawnerId) as Creature; }
}
