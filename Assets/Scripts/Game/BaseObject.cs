using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public delegate void Callback();

public class BaseObject : MonoBehaviour
{
    public virtual ObjectType _type { get => ObjectType.NONE; }
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
        }
    }

    public MoveControl MoveController { get => _moveController; }
    MoveControl _moveController = null;
    public MoveDir MoveDir { get => _moveController.direction; set => _moveController.SetDirection(value); }
    StateControl _stateController = null;
    public StateControl StateController { get => _stateController; }
    public State State { get => _stateController.State; set => _stateController.SetState(value, MoveDir); }

    private void Awake()
    {
        V_OnAwake();
    }

    private void Start()
    {
        V_OnStart();
    }

    protected Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (MoveDir)
        {
            case MoveDir.UP:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.DOWN:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.LEFT:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.RIGHT:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    protected IEnumerator SmoothMove(Vector3 targetPos)
    {
        while (true)
        {
            // 클라이언트 상에서는 서서히 이동하는 것처럼 보이게 한다.
            Vector3 dir = targetPos - transform.position;
            float distance = dir.magnitude;
            if (distance < _moveSpeed * Time.deltaTime)
            {
                transform.position = targetPos;
                _moveController.SetDirection(MoveDir.NONE);
                yield break;
            }
            else
            {
                transform.position += dir.normalized * _moveSpeed * Time.deltaTime;
            }

            yield return null;
        }
    }


    #region Virtual Functions
    // UpdateIdle, UpdateMoving, .. 이 실행되는 곳
    protected virtual void V_UpdateObject()
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
            StateController.SetState(State.MOVING, MoveDir);
        }
    }

    protected virtual void V_UpdateAttack() { }

    protected virtual void V_UpdateDead() { }

    public virtual void V_Attack() { }

    protected virtual void V_MoveToNextPos() { }
    #endregion

    #region Virtual Life-cycle Functions
    protected virtual void V_OnAwake()
    {
        _moveController = new MoveControl(this.gameObject);
        _stateController = new StateControl(GetComponent<Animator>(), GetComponent<SpriteRenderer>());

        Manager.Map.UpdatePosition(CellPos, CellPos, this); // TODO: cellpos는 json 혹은 csv로 처음에 불러와야함
    }

    protected virtual void V_OnStart()
    {
        // 현재 그리드의 위치 + new Vector3(0.5f, 0.5f) 가 셀 포지션을 기준으로 움직일때 자연스러워서 이렇게 함
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        _moveController.SetDirection(MoveDir.NONE);
        _stateController.SetState(State.NONE, MoveDir.NONE);
    }
    #endregion
}
