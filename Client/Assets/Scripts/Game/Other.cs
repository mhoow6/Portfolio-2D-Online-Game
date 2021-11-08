using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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
            case State.IDLE:
                V_UpdateIdle();
                break;
            case State.MOVING:
                V_UpdateMoving();
                break;
            case State.ATTACK:
                V_UpdateAttack();
                break;
            case State.DEAD:
                V_UpdateDead();
                break;
        }
    }

}
