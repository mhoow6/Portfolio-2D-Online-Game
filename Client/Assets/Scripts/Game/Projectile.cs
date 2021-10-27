using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Projectile : BaseObject
{
    public override ObjectType _type => ObjectType.PROJECTILE;
    public Creature _owner;

    public virtual void V_SetOwner(Creature owner)
    {
        _owner = owner;
        MoveController.SetDirection(_owner.MoveDir);

        // 기본 세팅
        transform.position = _owner.transform.position;
        CellPos = _owner.CellPos;
    }
}
