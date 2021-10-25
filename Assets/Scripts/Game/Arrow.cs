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

    protected override void V_OnUpdate()
    {
        if (_mc.direction == MoveDir.NONE)
        {
            MoveToNextPos();
        }
    }

    public override void V_Move()
    {
        switch (_mc.direction)
        {
            case MoveDir.UP:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case MoveDir.DOWN:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                break;
            case MoveDir.LEFT:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case MoveDir.RIGHT:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
        }

        base.V_Move();
    }

    void MoveToNextPos()
    {
        _mc.SetDirection(_lastDir);

        Vector3 targetPos = _mc.GetMovePos();
        Vector3Int targetCellPos = Manager.Map.CurrentGrid.WorldToCell(targetPos);

        if (Manager.Map.CanGo(targetCellPos))
        {
            CellPos = targetCellPos; // 이미 목적지에 있다는 것을 알림
            StartCoroutine(SmoothMove(targetPos));
        }
        else
        {
            if (Manager.Map.IsCreatureAt(targetCellPos))
            {
                Debug.Log("누군가를 맞췄습니다!");
            }
            else
            {
                Debug.Log("벽을 맞췄습니다..");
            }

            Clear();
        }
    }

    void Clear()
    {
        _owner = null;

        gameObject.SetActive(false);
    }
}
