using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class PlayerController : CreatureController
{
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
}