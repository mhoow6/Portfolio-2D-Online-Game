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
            CellPos = targetCellPos; // �̹� �������� �ִٴ� ���� �˸�
            StartCoroutine(SmoothMove(targetPos));
        }
        else
        {
            if (Manager.Map.IsCreatureAt(targetCellPos))
            {
                Debug.Log("�������� ������ϴ�!");
            }
            else
            {
                Debug.Log("���� ������ϴ�..");
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
