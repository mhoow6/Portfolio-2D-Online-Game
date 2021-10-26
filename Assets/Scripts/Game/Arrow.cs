using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Arrow : Projectile
{
    private void Update()
    {
        V_UpdateObject();
    }


    #region Override
    protected override void V_OnAwake()
    {
        base.V_OnAwake();

        _moveSpeed = 10.0f; // TODO: ���Ŀ� json ����
    }

    protected override void V_MoveToNextPos()
    {
        if (Manager.Map.CanGo(GetFrontCellPos()))
        {
            CellPos = GetFrontCellPos(); // �̹� �������� �ִٴ� ���� �˸�
        }
        else
        {
            Creature target = Manager.Map.CreatureAt(GetFrontCellPos());

            if (target != null)
            {
                // TODO: ���� ����
                StateController.SetState(State.IDLE, MoveDir); 
                target.V_OnDead();
            }
            else
            {
                Debug.Log("���� ������ϴ�..");
            }

            Clear();
        }
    }

    public override void V_SetOwner(Creature owner)
    {
        base.V_SetOwner(owner);

        switch (MoveDir)
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

        State = State.MOVING;
    }
    #endregion

    void Clear()
    {
        _owner = null;
        gameObject.SetActive(false);
    }
}
