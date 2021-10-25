using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
    public StateControl StateHandler { get => _sc; }

    protected StateControl _sc = null;
    protected Callback _animCallback;

    #region Virtual Functions
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
    protected override void V_OnAwake()
    {
        base.V_OnAwake();
        _sc = new StateControl(GetComponent<Animator>(), GetComponent<SpriteRenderer>());
    }

    protected override void V_OnStart()
    {
        base.V_OnStart();
        _sc.SetState(State.NONE, MoveDir.NONE);
    }

    protected override void V_OnUpdate()
    {
        V_UpdateAnimation();
    }
    #endregion
}
