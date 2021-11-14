using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class Arrow : Projectile
{
    private void Awake()
    {
        OnAwake();
    }

    private void OnEnable()
    {
        _owner = Manager.ObjectManager.Find(ObjectInfo.SpawnerId) as Creature;
        MoveDir = _owner.MoveDir;

        switch (MoveDir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
        }

        State = State.Moving;
    }

    private void Update()
    {
        V_UpdateObject();
    }

    protected override void V_MoveToNextPos()
    {
        // 서버에서 컨트롤
    }

    public override void V_Clear()
    {
        _owner = null;
        base.V_Clear();
    }
}
