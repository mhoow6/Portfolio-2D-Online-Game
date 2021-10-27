using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Arrow : Projectile
{
    private void Awake()
    {
        OnAwake();
        _moveSpeed = 10.0f; // TODO: 추후에 json 관리
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
            CellPos = GetFrontCellPos(); // 이미 목적지에 있다는 것을 알림
        }
        else
        {
            Creature target = Manager.Map.CreatureAt(GetFrontCellPos());

            if (target != null)
            {
                // TODO: 공격 판정
                StateController.SetState(State.IDLE, MoveDir); 
                target.V_Dead();
            }
            else
            {
                Debug.Log("벽을 맞췄습니다..");
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
