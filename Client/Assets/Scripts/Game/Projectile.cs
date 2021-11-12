using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseObject
{
    public Creature _owner;

    public virtual void V_SetOwner(Creature owner)
    {
        _owner = owner;
        MoveController.SetDirection(_owner.MoveDir);

        // �⺻ ����
        transform.position = _owner.transform.position;
        CellPos = _owner.CellPos;
    }
}
