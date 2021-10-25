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

    void KeyboardInputControl()
    {
        if (_mc.direction == MoveDir.NONE)
        {
            if (Input.GetKey(KeyCode.W))
            {
                _mc.SetDirection(MoveDir.UP);
                V_Move();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _mc.SetDirection(MoveDir.DOWN);
                V_Move();
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _mc.SetDirection(MoveDir.LEFT);
                V_Move();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _mc.SetDirection(MoveDir.RIGHT);
                V_Move();
            }
        }

        if (_sc.State != State.ATTACK)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _sc.SetWeapon(WeaponType.BAREHAND);
                _sc.SetState(State.ATTACK, _lastDir);
            }
            else if (Input.GetKey(KeyCode.LeftControl)) // TODO: 추후에 활 착용시 바뀌도록 해결
            {
                _sc.SetWeapon(WeaponType.BOW);
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
                switch (_sc.Weapontype)
                {
                    case WeaponType.BAREHAND:
                        if (Manager.Map.IsCreatureAt(GetFrontCellPos()))
                        {
                            Debug.Log("Hit Monster!"); // TODO: 체력 깎기
                        }
                        break;
                    case WeaponType.BOW:
                        Arrow arrow = Manager.Spawner.SpawnObject(ObjectType.PROJECTILE, CellPos) as Arrow;
                        arrow.SetOwner(this);
                        arrow.V_Move();
                        break;
                }
                break;
        }
    }
}