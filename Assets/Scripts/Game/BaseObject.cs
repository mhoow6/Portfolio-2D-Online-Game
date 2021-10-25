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
    protected MoveDir _lastDir = MoveDir.NONE;
    public MoveDir LastDir { get => _lastDir; }

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

    protected MoveControl _mc = null;

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

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (_lastDir)
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

    #region GameObject Move
    public virtual void V_Move()
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
    #endregion

    #region Virtual Life-cycle Functions
    protected virtual void V_OnAwake()
    {
        _mc = new MoveControl(this.gameObject);
    }

    protected virtual void V_OnStart()
    {
        // 현재 그리드의 위치 + new Vector3(0.5f, 0.5f);
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        _mc.SetDirection(MoveDir.NONE);
    }

    protected virtual void V_OnUpdate()
    {
        
    }
    #endregion
}
