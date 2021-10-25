using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Projectile : BaseObject
{
    public override ObjectType _type => ObjectType.PROJECTILE;
    public BaseObject _owner;

    public void SetOwner(Creature owner)
    {
        _owner = owner;
    }

    public void SetDirection(MoveDir dir)
    {
        _mc.SetDirection(dir);
    }

    
}
