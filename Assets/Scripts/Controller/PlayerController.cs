using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MoveDir
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    NONE = -1
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _moveSpeed = 5.0f;

    public Vector3Int cellPos { get; set; } = Vector3Int.zero;
    private MoveControl _mc = null;
    int callCount = 0; // Test

    private void Awake()
    {
        _mc = new MoveControl(this.gameObject);
    }

    private void Start()
    {
        // TODO: 현재 그리드의 위치 + new Vector3(0.5f, 0.5f);
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        _mc.strategy = _mc.SetStrategy(MoveDir.NONE);
    }

    private void Update()
    {
        KeyboardInputControl();
        Move();
    }

    void KeyboardInputControl()
    {
        if (_mc.direction == MoveDir.NONE)
        {
            if (Input.GetKey(KeyCode.W))
            {
                _mc.strategy = _mc.SetStrategy(MoveDir.UP);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _mc.strategy = _mc.SetStrategy(MoveDir.DOWN);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _mc.strategy = _mc.SetStrategy(MoveDir.LEFT);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _mc.strategy = _mc.SetStrategy(MoveDir.RIGHT);
            }
        }
    }

    void Move()
    {
        if (_mc.direction == MoveDir.NONE)
        {
            Vector3 targetPos = _mc.GetMovePos();
            Vector3Int targetCellPos = Manager.Map.CurrentGrid.WorldToCell(targetPos);

            if (Manager.Map.CanGo(targetCellPos))
            {
                // 플레이어는 이미 목표 위치에 도달해있는 상태
                cellPos = targetCellPos;
                StartCoroutine(SmoothMove(targetPos));
            }
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
                _mc.strategy = _mc.SetStrategy(MoveDir.NONE);
                yield break;
            }
            else
            {
                transform.position += dir.normalized * _moveSpeed * Time.deltaTime;
            }

            yield return null;
        }
    }

    void CountTest()
    {
        callCount++;
        Debug.Log(callCount);
    }
}

#region Move Factory
class MoveControl
{
    public MoveStrategy strategy = null;
    GameObject _go = null;

    public MoveControl(GameObject go)
    {
        _go = go;
        strategy = new MoveStay(go);
    }

    public MoveDir direction
    {
        get
        {
            return strategy.dir;
        }
    }

    public MoveStrategy SetStrategy(MoveDir moveDir)
    {
        switch (moveDir)
        {
            case MoveDir.UP:
                strategy = new MoveUp(_go);
                break;
            case MoveDir.DOWN:
                strategy = new MoveDown(_go);
                break;
            case MoveDir.LEFT:
                strategy = new MoveLeft(_go);
                break;
            case MoveDir.RIGHT:
                strategy = new MoveRight(_go);
                break;
            case MoveDir.NONE:
                strategy = new MoveStay(_go);
                break;
        }

        return strategy;
    }

    public Vector3 GetMovePos()
    {
        return strategy.GetMovePos();
    }
}
#endregion

#region Move-Case
public abstract class MoveStrategy
{
    protected GameObject _go = null;
    public MoveDir dir = MoveDir.NONE;

    public MoveStrategy(GameObject go)
    {
        _go = go;
    }

    public abstract Vector3 GetMovePos();
}

public class MoveUp : MoveStrategy
{
    public MoveUp(GameObject go) : base(go)
    {
        dir = MoveDir.UP;
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x, _go.transform.position.y + 1, 0);

        return Vector3.zero;
    }
}

public class MoveDown : MoveStrategy
{
    public MoveDown(GameObject go) : base(go)
    {
        dir = MoveDir.DOWN;
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x, _go.transform.position.y - 1, 0);

        return Vector3.zero;
    }
}

public class MoveLeft : MoveStrategy
{
    public MoveLeft(GameObject go) : base(go)
    {
        dir = MoveDir.LEFT;
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x - 1, _go.transform.position.y, 0);

        return Vector3.zero;
    }
}

public class MoveRight : MoveStrategy
{
    public MoveRight(GameObject go) : base(go)
    {
        dir = MoveDir.RIGHT;
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x + 1, _go.transform.position.y, 0);

        return Vector3.zero;
    }
}

public class MoveStay : MoveStrategy
{
    public MoveStay(GameObject go) : base(go)
    {
        
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x, _go.transform.position.y, 0);

        return Vector3.zero;
    }
}
#endregion