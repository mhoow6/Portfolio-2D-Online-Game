using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    protected float _moveSpeed = 5.0f;

    [SerializeField]
    protected MoveDir _lastDir = MoveDir.NONE;

    [SerializeField]
    Vector3Int _cellPos = Vector3Int.zero;
    public Vector3Int CellPos
    {
        get => _cellPos;
        set
        {
            _cellPos = value;
        }
    }

    protected MoveControl _mc = null;
    protected StateControl _sc = null;

    private void Awake()
    {
        V_OnAwake();
    }

    private void Start()
    {
        V_OnStart();
    }

    private void Update()
    {
        V_OnUpdate();
    }

    protected void UpdateAnimation()
    {
        switch (_mc.direction)
        {
            case MoveDir.UP:
                _sc.SetState(State.MOVING, _mc.direction);
                break;
            case MoveDir.DOWN:
                _sc.SetState(State.MOVING, _mc.direction);
                break;
            case MoveDir.LEFT:
                _sc.SetState(State.MOVING, _mc.direction);
                break;
            case MoveDir.RIGHT:
                _sc.SetState(State.MOVING, _mc.direction);
                break;
            case MoveDir.NONE:
                _sc.SetState(State.IDLE, _lastDir);
                break;
        }

        _sc.PlayAnimation();
    }

    #region GameObject Move
    protected void Move()
    {
        _lastDir = _mc.direction;

        Vector3 targetPos = _mc.GetMovePos();
        Vector3Int targetCellPos = Manager.Map.CurrentGrid.WorldToCell(targetPos);

        if (Manager.Map.CanGo(targetCellPos))
        {
            StartCoroutine(SmoothMove(targetPos));
        }
        else
        {
            _mc.SetDirection(MoveDir.NONE);
        }
    }

    IEnumerator SmoothMove(Vector3 targetPos)
    {
        Vector3Int targetCellPos = Manager.Map.CurrentGrid.WorldToCell(targetPos);

        while (true)
        {
            // 클라이언트 상에서는 서서히 이동하는 것처럼 보이게 한다.
            Vector3 dir = targetPos - transform.position;
            float distance = dir.magnitude;
            if (distance < _moveSpeed * Time.deltaTime)
            {
                transform.position = targetPos;
                CellPos = targetCellPos;
                _mc.SetDirection(MoveDir.NONE);
                yield break;
            }
            else
            {
                transform.position += dir.normalized * _moveSpeed * Time.deltaTime;
            }

            yield return null;
        }
    }
    #endregion

    #region Virtual Functions
    public virtual void V_DestroyObject()
    {

    }
    #endregion

    #region Virtual Life-cycle Functions
    protected virtual void V_OnAwake()
    {
        _mc = new MoveControl(this.gameObject);
        _sc = new StateControl(GetComponent<Animator>(), GetComponent<SpriteRenderer>());
    }

    protected virtual void V_OnStart()
    {
        // TODO: 현재 그리드의 위치 + new Vector3(0.5f, 0.5f);
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        _mc.SetDirection(MoveDir.NONE);
        _sc.SetState(State.NONE, MoveDir.NONE);
    }

    protected virtual void V_OnUpdate()
    {
        UpdateAnimation();
    }
    #endregion
}
