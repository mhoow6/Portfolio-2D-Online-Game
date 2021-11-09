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
        #region �������
        // State, MoveDir, CellPos�� ��Ŷ���� �����ϹǷ� ���� ���� Ŭ�󿡼� ��Ʈ���� �ʿ����
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
