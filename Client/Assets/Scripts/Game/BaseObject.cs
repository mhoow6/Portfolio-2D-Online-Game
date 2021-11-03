using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public delegate void Callback();

public class BaseObject : MonoBehaviour
{
    public ObjectCode code { get; set; }

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
    public StateControl StateController { get => _stateController; }
    StateControl _stateController = null;

    // SetState �� �ڵ����� �ش� �ִϸ��̼� ȣ��
    public State State { get => _stateController.State; set => _stateController.SetState(value, MoveDir); }

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

    protected void OnAwake()
    {
        _moveController = new MoveControl(this.gameObject);
        _stateController = new StateControl(GetComponent<Animator>(), GetComponent<SpriteRenderer>());

        Manager.Map.UpdatePosition(CellPos, CellPos, this); // TODO: cellpos�� json Ȥ�� csv�� ó���� �ҷ��;���
    }

    protected void OnStart()
    {
        // ���� �׸����� ��ġ + new Vector3(0.5f, 0.5f) �� �� �������� �������� �����϶� �ڿ��������� �̷��� ��
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        _moveController.SetDirection(MoveDir.NONE);
        _stateController.SetState(State.NONE, MoveDir.NONE);
    }


    #region virtual
    // UpdateIdle, UpdateMoving, .. �� ����Ǵ� ��
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

        // ���� ���� üũ
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

    protected virtual void V_MoveToNextPos() { }

    protected virtual void V_UpdateAttack() { }

    protected virtual void V_UpdateDead() { }

    public virtual void V_Attack() { }
    #endregion
}
