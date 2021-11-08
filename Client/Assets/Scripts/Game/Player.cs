using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public class Player : Creature
{
    public int count = 0;
    bool _moveKeyPressed = true;
    Camera _myCamera;

    private void Awake()
    {
        OnAwake();

        _myCamera = Camera.main;
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        V_UpdateObject();
    }

    private void LateUpdate()
    {
        if (_myCamera != null)
            _myCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
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
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                StateController.SetWeapon(WeaponType.BOW);
                StateController.SetState(State.ATTACK, MoveDir);
            }
        }
    }

    protected override void V_UpdateAttack()
    {
        if (StateController.IsAnimationDone())
        {
            _attackOnce = false;
            StateController.SetState(State.IDLE, MoveDir);
        }
        else
        {
            if (_attackOnce == false)
            {
                switch (StateController.Weapontype)
                {
                    case WeaponType.BAREHAND:
                        if (Manager.Map.IsCreatureAt(GetFrontCellPos()))
                        {
                            Debug.Log("Hit Monster!"); // TODO: Ã¼·Â ±ð±â
                        }
                        break;
                    case WeaponType.BOW:
                        Arrow arrow = Manager.Spawner.SpawnObject(ObjectCode.Arrow) as Arrow;
                        arrow.V_SetOwner(this);
                        break;
                }

                _attackOnce = true;
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
                            Debug.Log("Hit Monster!"); // TODO: Ã¼·Â ±ð±â
                        }
                        break;
                    case WeaponType.BOW:
                        Arrow arrow = Manager.Spawner.SpawnObject(ObjectCode.Arrow) as Arrow;
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
                SendMovePacket(ObjectInfo);
            }
        }
    }
    #endregion

}