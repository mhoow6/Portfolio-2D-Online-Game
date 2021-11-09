using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public delegate void Callback();

public class BaseObject : MonoBehaviour
{
    public ObjectInfo ObjectInfo { get; set; } = new ObjectInfo();
    
    public ObjectCode code { get => (ObjectCode)ObjectInfo.ObjectCode; set { ObjectInfo.ObjectCode = (int)value; } }
    public int id { get => ObjectInfo.ObjectId; set { ObjectInfo.ObjectId = value; } }
    public int roomId { get => ObjectInfo.RoomId; set { ObjectInfo.RoomId = value; } }

    protected float _moveSpeed = 5.0f;

    [SerializeField]
    Vector3Int _cellPos = Vector3Int.zero;
    public virtual Vector3Int CellPos
    {
        get => _cellPos;
        set
        {
            Manager.Map.UpdatePosition(_cellPos, value, this);
            _cellPos = value;
            ObjectInfo.Position = new Google.Protobuf.Protocol.Vector2() { X = _cellPos.x, Y = _cellPos.y };
        }
    }

    public MoveControl MoveController { get => _moveController; }
    MoveControl _moveController = null;

    public MoveDir MoveDir
    {
        get => _moveController.direction;
        set
        {
            _moveController.SetDirection(value);
            ObjectInfo.MoveDir = value;
        }
    }

    // SetState 시 자동으로 해당 애니메이션 호출
    StateControl _stateController = null;

    // 상태는 이동방향을 꼭 정해놓고 할 것!!
    public State State
    {
        get => _stateController.State;
        set
        {
            _stateController.SetState(value, MoveDir);
            ObjectInfo.State = value;
        }
    }

    public WeaponType WeaponType
    {
        get => _stateController.Weapontype;

        set
        {
            _stateController.SetWeapon(value);
        }
    }

    public bool IsAnimationDone
    {
        get => _stateController.IsAnimationDone();
    }

    protected Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (MoveDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    protected void OnAwake()
    {
        _moveController = new MoveControl(this.gameObject);
        _stateController = new StateControl(GetComponent<Animator>(), GetComponent<SpriteRenderer>());
    }

    protected void OnStart()
    {
        _moveController.SetDirection(MoveDir.Up);
        _stateController.SetState(State.Idle, MoveDir);
    }

    #region virtual
    // UpdateIdle, UpdateMoving, .. 이 실행되는 곳
    protected virtual void V_UpdateObject()
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

    protected virtual void V_UpdateIdle(){ }

    protected virtual void V_UpdateMoving()
    {
        Vector3 destPos = Manager.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < _moveSpeed * Time.deltaTime)
        {
            transform.position = destPos;
            V_MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * _moveSpeed * Time.deltaTime;
            State = State.Moving;
        }
    }

    protected virtual void V_MoveToNextPos() { }

    protected virtual void V_UpdateAttack() { }

    protected virtual void V_UpdateDead() { }

    public virtual void V_Attack() { }
    #endregion
}
