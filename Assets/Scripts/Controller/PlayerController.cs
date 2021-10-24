using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class PlayerController : CreatureController
{
    public int count = 0;

    void KeyboardInputControl()
    {
        if (_mc.direction == MoveDir.NONE)
        {
            if (Input.GetKey(KeyCode.W))
            {
                _mc.SetDirection(MoveDir.UP);
                Move();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _mc.SetDirection(MoveDir.DOWN);
                Move();
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _mc.SetDirection(MoveDir.LEFT);
                Move();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _mc.SetDirection(MoveDir.RIGHT);
                Move();
            }
        }

        if (_sc.State != State.ATTACK)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _sc.SetState(State.ATTACK, _lastDir);
            }
        }
        
    }

    protected override void V_OnAwake()
    {
        base.V_OnAwake();
    }

    protected override void V_OnStart()
    {
        base.V_OnStart();
    }

    protected override void V_OnUpdate()
    {
        KeyboardInputControl();
        base.V_OnUpdate();
        
    }

    public override void V_Attack()
    {
        switch (_sc.State)
        {
            case State.ATTACK:
                #region 공격좌표 계산
                int x = CellPos.x;
                switch(_lastDir)
                {
                    case MoveDir.LEFT:
                        x = CellPos.x - 1; // TODO: 공격거리에 따라 바뀐다.
                        break;
                    case MoveDir.RIGHT:
                        x = CellPos.x + 1; // TODO: 공격거리에 따라 바뀐다.
                        break;
                }
                int y = CellPos.y;
                switch (_lastDir)
                {
                    case MoveDir.UP:
                        y = CellPos.y + 1; // TODO: 공격거리에 따라 바뀐다.
                        break;
                    case MoveDir.DOWN:
                        y = CellPos.y - 1; // TODO: 공격거리에 따라 바뀐다.
                        break;
                }
                #endregion
                if (Manager.Map.IsCreatureAt(new Vector3Int(x, y, 0)))
                {
                    Debug.Log("Hit Monster!"); // TODO: 체력 깎기
                }
                break;
            case State.SKILL:
                break;
        }
    }

    IEnumerator AnimationDoneCheck()
    {
        while (true)
        {
            if (_sc.IsAnimationDone())
            {
                _animCallback = null;
                Debug.Log("Animation Done!");
                _sc.SetState(State.IDLE, _lastDir);
                yield break;
            }

            yield return null;
        }
    }
}