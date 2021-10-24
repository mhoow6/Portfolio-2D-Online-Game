using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public delegate void AnimationCallback();

public class CreatureController : MonoBehaviour
{
    protected float _moveSpeed = 5.0f;

    [SerializeField]
    protected MoveDir _lastDir = MoveDir.NONE;
    public MoveDir LastDir { get => _lastDir; }

    [SerializeField]
    Vector3Int _cellPos = Vector3Int.zero;
    public Vector3Int CellPos
    {
        get => _cellPos;
        set
        {
            Manager.Map.UpdatePosition(_cellPos, value, gameObject);
            _cellPos = value;
        }
    }

    protected MoveControl _mc = null;
    public StateControl StateHandler { get => _sc; }
    protected StateControl _sc = null;
    protected AnimationCallback _animCallback;

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

    

    #region GameObject Move
    protected void Move()
    {
        _lastDir = _mc.direction;

        Vector3 targetPos = _mc.GetMovePos();
        Vector3Int targetCellPos = Manager.Map.CurrentGrid.WorldToCell(targetPos);

        if (Manager.Map.CanGo(targetCellPos))
        {
            CellPos = targetCellPos; // 이미 목적지에 있다는 것을 알림
            StartCoroutine(SmoothMove(targetPos));
        }
        else
        {
            _mc.SetDirection(MoveDir.NONE);
        }
    }

    IEnumerator SmoothMove(Vector3 targetPos)
    {
        while (true)
        {
            // 클라이언트 상에서는 서서히 이동하는 것처럼 보이게 한다.
            Vector3 dir = targetPos - transform.position;
            float distance = dir.magnitude;
            if (distance < _moveSpeed * Time.deltaTime)
            {
                transform.position = targetPos;
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
    public virtual void V_Attack()
    {

    }

    protected virtual void V_UpdateAnimation()
    {
        // 캐릭터 이동 애니메이션
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
                if (_sc.State == State.IDLE || _sc.State == State.MOVING)
                    _sc.SetState(State.IDLE, _lastDir);
                break;
        }

        _sc.PlayAnimation();
        if (_animCallback != null)
            _animCallback.Invoke();

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
        // 현재 그리드의 위치 + new Vector3(0.5f, 0.5f);
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        _mc.SetDirection(MoveDir.NONE);
        _sc.SetState(State.NONE, MoveDir.NONE);
    }

    protected virtual void V_OnUpdate()
    {
        V_UpdateAnimation();
    }
    #endregion
}
