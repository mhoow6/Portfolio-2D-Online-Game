using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Projectile : BaseObject
{
    public override ObjectType _type => ObjectType.PROJECTILE;
    public Creature _owner;

    protected override void V_OnAwake()
    {
        base.V_OnAwake();
    }

    protected override void V_OnStart()
    {
        base.V_OnStart();
    }

    public void SetOwner(Creature owner)
    {
        _owner = owner;
        _mc.SetDirection(_owner.LastDir);

        // 기본 세팅: 직접 바꿔줄 수도 있다.
        transform.position = _owner.transform.position;
        CellPos = _owner.CellPos;
    }

    public void SetDirection(MoveDir dir)
    {
        _mc.SetDirection(dir);
    }

    
}
