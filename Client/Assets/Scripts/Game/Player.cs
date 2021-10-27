using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class Player : Creature
{
    public override ObjectType _type => ObjectType.PLAYER;
    public int count = 0;
    bool _moveKeyPressed = true;

    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();

        // TEMP
        CellPos = Vector3Int.down;
        transform.position = Manager.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f, 0);
    }

    private void Update()
    {
        V_UpdateObject();
    }

    
    void InputMoveControl()
    {
        // MOVE
        if (Input.GetKey(KeyCode.W))
        {
            MoveDir = MoveDir.UP;
            _moveKeyPressed = true;
            StateController.SetState(State.MOVING, MoveDir);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveDir = MoveDir.DOWN;
            _moveKeyPressed = true;
            StateController.SetState(State.MOVING, MoveDir);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            MoveDir = MoveDir.LEFT;
            _moveKeyPressed = true;
            StateController.SetState(State.MOVING, MoveDir);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveDir = MoveDir.RIGHT;
            _moveKeyPressed = true;
            StateController.SetState(State.MOVING, MoveDir);
        }
        else
        {
            _moveKeyPressed = false;
        }
    }

    #region override
    protected override void V_UpdateObject()
    {
        switch (State)
        {
            case State.IDLE:
                InputMoveControl();
                break;
            case State.MOVING:
                InputMoveControl();
                break;
        }

        base.V_UpdateObject();
    }

    protected override void V_UpdateIdle()
    {
        if (_moveKeyPressed == true)
        {
            StateController.SetState(State.MOVING, MoveDir);
            return;
        }
        else
        {
            StateController.SetState(State.IDLE, MoveDir);

            if (Input.GetKey(KeyCode.Space))
            {
                StateController.SetWeapon(WeaponType.BAREHAND);
                StateController.SetState(State.ATTACK, MoveDir);
            }
            else if (Input.GetKey(KeyCode.LeftControl)) // TODO: 추후에 활 착용시 바뀌도록 해결
            {
                StateController.SetWeapon(WeaponType.BOW);
                StateController.SetState(State.ATTACK, MoveDir);
            }
        }
    }

    public override void V_Attack()
    {
        switch (State)
        {
            case State.ATTACK:
                switch (StateController.Weapontype)
                {
                    case WeaponType.BAREHAND:
                        if (Manager.Map.IsCreatureAt(GetFrontCellPos()))
                        {
                            Debug.Log("Hit Monster!"); // TODO: 체력 깎기
                        }
                        break;
                    case WeaponType.BOW:
                        Arrow arrow = Manager.Spawner.SpawnObject(ObjectCode.ARROW) as Arrow;
                        arrow.V_SetOwner(this);
                        break;
                }
                break;
        }
    }

    protected override void V_MoveToNextPos()
    {
        if (_moveKeyPressed == false)
        {
            StateController.SetState(State.IDLE, MoveDir);
            return;
        }

        if (Manager.Map.CanGo(GetFrontCellPos()))
        {
            if (Manager.Map.CreatureAt(GetFrontCellPos()) == null)
            {
                CellPos = GetFrontCellPos();
            }
        }
    }
    #endregion

}