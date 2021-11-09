using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class Arrow : Projectile
{
    private void Awake()
    {
        OnAwake();
        _moveSpeed = 10.0f; // TODO: ���Ŀ� json ����
    }

    private void Update()
    {
        V_UpdateObject();
    }


    #region Override
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
                State = State.Attack;
                target.V_Dead();
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
    #endregion
    void Clear()
    {
        _owner = null;
        gameObject.SetActive(false);
    }
}
