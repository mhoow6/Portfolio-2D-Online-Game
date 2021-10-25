using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Arrow : Projectile
{
    protected override void V_OnAwake()
    {
        base.V_OnAwake();

        _moveSpeed = 20.0f; // TODO: 추후에 json 관리
    }

    protected override void V_OnStart()
    {
        base.V_OnStart();
    }

    public override void V_Move()
    {
        _mc.SetDirection(_lastDir);

        // 방향에 맞게 회전
        switch (_lastDir)
        {
            case MoveDir.UP:
                break;
            case MoveDir.DOWN:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -180));
                break;
            case MoveDir.LEFT:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case MoveDir.RIGHT:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
        }

        Vector3 targetPos = _mc.GetMovePos();
        Vector3Int targetCellPos = Manager.Map.CurrentGrid.WorldToCell(targetPos);

        if (Manager.Map.CanGo(targetCellPos))
        {
            CellPos = targetCellPos; // 이미 목적지에 있다는 것을 알림
            StartCoroutine(SmoothMove(targetPos));
        }
        else
        {
            _mc.SetDirection(MoveDir.NONE);
        }
    }
}
