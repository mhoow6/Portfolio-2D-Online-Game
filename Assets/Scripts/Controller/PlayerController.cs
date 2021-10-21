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
    private MoveControl mc = null;

    private void Awake()
    {
        mc = new MoveControl(this.gameObject);
    }

    private void Start()
    {
        // TODO: 현재 그리드의 위치 + new Vector3(0.5f, 0.5f);
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    private void Update()
    {
        KeyboardInputControl();
    }

    void KeyboardInputControl()
    {
        #region Keyboard
        if (Input.GetKey(KeyCode.W))
        {
            mc.strategy = mc.GetMoveStrategy(KeyCode.W);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            mc.strategy = mc.GetMoveStrategy(KeyCode.S);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            mc.strategy = mc.GetMoveStrategy(KeyCode.A);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            mc.strategy = mc.GetMoveStrategy(KeyCode.D);
        }
        else
        {
            mc.strategy = mc.GetMoveStrategy(KeyCode.None);
        }
        #endregion

        #region Move
        {
            Vector3 targetPos = mc.strategy.GetMovePos();
            Vector3Int targetCellPos = Manager.Map.CurrentGrid.WorldToCell(targetPos);

            if (targetPos != transform.position)
            {
                if (Manager.Map.CanGo(targetCellPos))
                {
                    cellPos = targetCellPos;
                    Vector3 dir = targetPos - transform.position;

                    // 목적지까지의 거리 계산
                    float distance = dir.magnitude;
                    if (distance < _moveSpeed * Time.deltaTime)
                    {
                        transform.position = targetPos;
                    }
                    else
                    {
                        transform.position += dir.normalized * _moveSpeed * Time.deltaTime;
                    }

                }
            }
        }
        
        #endregion
    }
}

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

    public MoveStrategy GetMoveStrategy(KeyCode keycode)
    {
        if (keycode == KeyCode.W)
        {
            strategy = new MoveUp(_go);
            return strategy;
        }
        else if (keycode == KeyCode.S)
        {
            strategy = new MoveDown(_go);
            return strategy;
        }
        else if (keycode == KeyCode.A)
        {
            strategy = new MoveLeft(_go);
            return strategy;
        }
        else if (keycode == KeyCode.D)
        {
            strategy = new MoveRight(_go);
            return strategy;
        }

        return new MoveStay(_go);
    }
}

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

