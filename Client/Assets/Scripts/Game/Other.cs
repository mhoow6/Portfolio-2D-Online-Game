using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class Other : Player
{
    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        V_UpdateObject();
    }

    protected override void V_UpdateObject()
    {
        switch (State)
        {
            case State.Idle:
                V_UpdateIdle();
                break;
            case State.Moving:
                V_UpdateMoving();
                break;
            case State.Attack:
                V_UpdateAttack();
                break;
            case State.Dead:
                V_UpdateDead();
                break;
        }
    }

    protected override void V_UpdateIdle()
    {
        
    }

    protected override void V_MoveToNextPos()
    {
        #region 참고사항
        // State, MoveDir, CellPos을 패킷으로 조작하므로 굳이 개별 클라에서 컨트롤할 필요없음
        /*if (_moveKeyPressed == false)
        {
            State = State.Idle;
            return;
        }*/


        /*if (Manager.Map.CanGo(GetFrontCellPos()))
        {
            if (Manager.Map.CreatureAt(GetFrontCellPos()) == null)
            {
                CellPos = GetFrontCellPos();
            }
        }*/
        #endregion
    }

}
