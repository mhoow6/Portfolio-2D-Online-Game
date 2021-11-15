using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
 

public class Player : Creature
{
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
            MoveDir = MoveDir.Up;
            _moveKeyPressed = true;
            State = State.Moving;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveDir = MoveDir.Down;
            _moveKeyPressed = true;
            State = State.Moving;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            MoveDir = MoveDir.Left;
            _moveKeyPressed = true;
            State = State.Moving;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveDir = MoveDir.Right;
            _moveKeyPressed = true;
            State = State.Moving;
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
            case State.Idle:
                InputMoveControl();
                break;
            case State.Moving:
                InputMoveControl();
                break;
        }

        base.V_UpdateObject();
    }

    protected override void V_UpdateIdle()
    {
        if (_moveKeyPressed == true)
        {
            State = State.Moving;
            return;
        }
        else
        {
            State = State.Idle;

            if (Input.GetKey(KeyCode.Space))
            {
                Weapon = ObjectCode.Barehand;
                State = State.Attack;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                Weapon = ObjectCode.Bow;
                State = State.Attack;
            }
        }
    }

    protected override void V_UpdateAttack()
    {
        if (IsAnimationDone)
        {
            _attackOnce = false;
            State = State.Idle;
            Manager.Network.SendSyncPacket(ObjectInfo);
        }
        else
        {
            if (_attackOnce == false)
            {
                switch (Weapon)
                {
                    case ObjectCode.Barehand:
                        Manager.Network.SendAttackPacket(ObjectInfo);
                        break;
                    case ObjectCode.Bow:
                        Manager.Network.SendSyncPacket(ObjectInfo);
                        Manager.Network.SendSpawnPacket(new SpawnInfo() { SpawnerId = this.id, ObjectCode = (int)ObjectCode.Arrow, RoomId = ObjectInfo.RoomId });
                        break;
                }

                _attackOnce = true;
            }
        }
    }

    protected override void V_MoveToNextPos()
    {
        if (_moveKeyPressed == false)
        {
            State = State.Idle;
            Manager.Network.SendMovePacket(ObjectInfo); // ½ÌÅ© ¿ëµµ
            return;
        }

        if (Manager.Map.CanGo(GetFrontCellPos()))
        {
            CellPos = GetFrontCellPos();
            Manager.Network.SendMovePacket(ObjectInfo);
        }
    }
    #endregion

}