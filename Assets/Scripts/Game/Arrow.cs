using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Arrow : Projectile
{
    protected override void V_OnAwake()
    {
        base.V_OnAwake();

        _moveSpeed = 20.0f; // TODO: ���Ŀ� json ����
    }

    protected override void V_OnStart()
    {
        base.V_OnStart();
    }

    public override void V_Move()
    {
        _mc.SetDirection(_lastDir);

        // ���⿡ �°� ȸ��
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
            CellPos = targetCellPos; // �̹� �������� �ִٴ� ���� �˸�
            StartCoroutine(SmoothMove(targetPos));
        }
        else
        {
            _mc.SetDirection(MoveDir.NONE);
        }
    }
}
