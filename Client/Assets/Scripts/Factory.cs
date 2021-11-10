using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

#region State Factory
public class StateControl
{
    StateStrategy _strategy = null;

    Animator _animator = null;
    SpriteRenderer _sprite = null;
    public State State
    {
        get => _strategy._state;
    }

    public WeaponType Weapontype
    {
        get => _weaponType;
    }
    WeaponType _weaponType = WeaponType.BAREHAND;

    public StateControl(Animator animator, SpriteRenderer sprite)
    {
        _animator = animator;
        _sprite = sprite;
        _strategy = new StateIdle(animator, MoveDir.Up, sprite);
    }

    public void SetWeapon(WeaponType weaponType)
    {
        _weaponType = weaponType;
    }

    // State를 설정할 때 애니메이션이 바로 실행되도록 변경함.
    public void SetState(State state, MoveDir dir)
    {
        switch (state)
        {
            case State.Idle:
                _strategy = new StateIdle(_animator, dir, _sprite);
                break;
            case State.Moving:
                _strategy = new StateMoving(_animator, dir, _sprite);
                break;
            case State.Attack:
                _strategy = new StateAttack(_animator, dir, _sprite, _weaponType);
                break;
            case State.Skill:
                _strategy = new StateSkill(_animator, dir, _sprite);
                break;
            case State.Dead:
                _strategy = new StateDead(_animator, dir, _sprite);
                break;
        }

        PlayAnimation();
    }

    void PlayAnimation()
    {
        if (_strategy != null && _animator != null)
            _strategy.PlayAnimation();
    }

    public bool IsAnimationDone()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            return true;
        }

        return false;
    }
}
#endregion

#region State-Case
public abstract class StateStrategy
{
    public State _state = State.Idle;

    protected Animator _animator = null;
    protected MoveDir _dir = MoveDir.Up;
    protected SpriteRenderer _sprite = null;

    public StateStrategy(Animator animator, MoveDir dir, SpriteRenderer sprite)
    {
        _animator = animator;
        _dir = dir;
        _sprite = sprite;
    }

    public abstract void PlayAnimation();
}

public class StateIdle : StateStrategy
{
    public StateIdle(Animator animator, MoveDir dir, SpriteRenderer sprite) : base(animator, dir, sprite)
    {
        _state = State.Idle;
    }

    public override void PlayAnimation()
    {
        switch (_dir)
        {
            case MoveDir.Up:
                _animator.Play("IDLE_BACK");
                _sprite.flipX = false;
                break;
            case MoveDir.Down:
                _animator.Play("IDLE_FRONT");
                _sprite.flipX = false;
                break;
            case MoveDir.Left:
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = true;
                break;
            case MoveDir.Right:
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = false;
                break;
        }
    }
}

public class StateMoving : StateStrategy
{
    public StateMoving(Animator animator, MoveDir dir, SpriteRenderer sprite) : base(animator, dir, sprite)
    {
        _state = State.Moving;
    }

    public override void PlayAnimation()
    {
        switch (_dir)
        {
            case MoveDir.Up:
                _animator.Play("WALK_BACK");
                _sprite.flipX = false;
                break;
            case MoveDir.Down:
                _animator.Play("WALK_FRONT");
                _sprite.flipX = false;
                break;
            case MoveDir.Left:
                _animator.Play("WALK_RIGHT");
                _sprite.flipX = true;
                break;
            case MoveDir.Right:
                _animator.Play("WALK_RIGHT");
                _sprite.flipX = false;
                break;
        }
    }
}

public class StateAttack : StateStrategy
{
    WeaponType _weaponType;

    public StateAttack(Animator animator, MoveDir dir, SpriteRenderer sprite) : base(animator, dir, sprite)
    {
        _state = State.Attack;
    }

    public StateAttack(Animator animator, MoveDir dir, SpriteRenderer sprite, WeaponType weaponType) : base(animator, dir, sprite)
    {
        _state = State.Attack;
        _weaponType = weaponType;
    }

    public override void PlayAnimation()
    {
        switch (_weaponType)
        {
            case WeaponType.BAREHAND:
                switch (_dir)
                {
                    case MoveDir.Up:
                        _animator.Play("ATTACK_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("ATTACK_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play("ATTACK_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("ATTACK_RIGHT");
                        _sprite.flipX = false;
                        break;
                }
                break;
            case WeaponType.BOW:
                switch (_dir)
                {
                    case MoveDir.Up:
                        _animator.Play("ATTACK_WEAPON_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("ATTACK_WEAPON_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play("ATTACK_WEAPON_RIGHT");
                        _sprite.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("ATTACK_WEAPON_RIGHT");
                        _sprite.flipX = false;
                        break;
                }
                break;
        }
        
    }
}

public class StateSkill : StateStrategy
{
    public StateSkill(Animator animator, MoveDir dir, SpriteRenderer sprite) : base(animator, dir, sprite)
    {
        _state = State.Skill;
    }

    public override void PlayAnimation()
    {
        // TODO
    }
}

public class StateDead : StateStrategy
{
    public StateDead(Animator animator, MoveDir dir, SpriteRenderer sprite) : base(animator, dir, sprite)
    {
        _state = State.Dead;
    }

    public override void PlayAnimation()
    {
        // TODO
    }
}
#endregion

#region Move Factory
public class MoveControl
{
    MoveStrategy _strategy = null;
    GameObject _go = null;

    public MoveControl(GameObject go)
    {
        _go = go;
        _strategy = new MoveStay(go);
    }

    public MoveDir direction
    {
        get
        {
            return _strategy._dir;
        }
    }

    public MoveStrategy SetDirection(MoveDir moveDir)
    {
        switch (moveDir)
        {
            case MoveDir.Up:
                _strategy = new MoveUp(_go);
                break;
            case MoveDir.Down:
                _strategy = new MoveDown(_go);
                break;
            case MoveDir.Left:
                _strategy = new MoveLeft(_go);
                break;
            case MoveDir.Right:
                _strategy = new MoveRight(_go);
                break;
            default:
                _strategy = new MoveStay(_go);
                break;
        }

        return _strategy;
    }

    public Vector3 GetMovePos()
    {
        return _strategy.GetMovePos();
    }
}
#endregion

#region Move-Case
public abstract class MoveStrategy
{
    protected GameObject _go = null;
    protected readonly float _moveUnit = 1f;
    public MoveDir _dir = MoveDir.Up;

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
        _dir = MoveDir.Up;
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x, _go.transform.position.y + _moveUnit, 0);

        return Vector3.zero;
    }
}

public class MoveDown : MoveStrategy
{
    public MoveDown(GameObject go) : base(go)
    {
        _dir = MoveDir.Down;
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x, _go.transform.position.y - _moveUnit, 0);

        return Vector3.zero;
    }
}

public class MoveLeft : MoveStrategy
{
    public MoveLeft(GameObject go) : base(go)
    {
        _dir = MoveDir.Left;
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x - _moveUnit, _go.transform.position.y, 0);

        return Vector3.zero;
    }
}

public class MoveRight : MoveStrategy
{
    public MoveRight(GameObject go) : base(go)
    {
        _dir = MoveDir.Right;
    }

    public override Vector3 GetMovePos()
    {
        if (_go != null)
            return new Vector3(_go.transform.position.x + _moveUnit, _go.transform.position.y, 0);

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

#region Map Factory
public class MapFactory
{
    public static GameObject GetMapObject(MapId mapId)
    {
        GameObject map = null;

        switch (mapId)
        {
            case MapId.Town:
                map = Resources.Load<GameObject>(ResourcePaths.Map_Prefabs + "/Map_001");
                break;
            case MapId.Dungeon:
                map = Resources.Load<GameObject>(ResourcePaths.Map_Prefabs + "/Map_Dungeon");
                break;
        }

        return map;
    }

    public static TextAsset GetMapCollisionTextAsset(MapId mapId)
    {
        TextAsset map = null;

        switch (mapId)
        {
            case MapId.Town:
                map = Resources.Load<TextAsset>(ResourcePaths.Map_Collision_Save_Resource + "/Map_001");
                break;
            case MapId.Dungeon:
                map = Resources.Load<TextAsset>(ResourcePaths.Map_Collision_Save_Resource + "/Map_Dungeon");
                break;
        }

        return map;
    }
}
#endregion

#region Object Factory
public class ObjectFactory
{
    public static BaseObject AddComponentToObject(ObjectCode code, GameObject obj)
    {
        BaseObject ret;

        switch (code)
        {
            case ObjectCode.Player:
                if (obj.GetComponent<Player>() == null)
                {
                    ret = obj.AddComponent<Player>();
                    obj.name = "Player";
                    return ret;
                }
                break;
            case ObjectCode.Monster:
                if (obj.GetComponent<Monster>() == null)
                {
                    ret = obj.AddComponent<Monster>();
                    obj.name = "Monster";
                    return ret;
                }
                break;
            case ObjectCode.DeadEffect:
                if (obj.GetComponent<DeathEffect>() == null)
                {
                    ret = obj.AddComponent<DeathEffect>();
                    obj.name = "DeathEffect";
                    return ret;
                }
                break;
            case ObjectCode.Arrow:
                if (obj.GetComponent<Arrow>() == null)
                {
                    ret = obj.AddComponent<Arrow>();
                    obj.name = "Arrow";
                    return ret;
                }
                break;
        }

        return null;
    }

    public static BaseObject AddComponentToObject(ObjectInfo objInfo, GameObject obj)
    {
        BaseObject ret = null;

        switch ((ObjectCode)objInfo.ObjectCode)
        {
            case ObjectCode.Player:
                if (obj.GetComponent<Player>() == null && obj.GetComponent<Other>() == null)
                {
                    if (Manager.ObjectManager.Me == null)
                    {
                        ret = obj.AddComponent<Player>();
                        obj.name = "Player";
                        return ret;
                    }
                    else
                    {
                        ret = obj.AddComponent<Other>();
                        obj.name = "Other";
                        return ret;
                    }
                }
                break;
            case ObjectCode.Monster:
                if (obj.GetComponent<Monster>() == null)
                {
                    ret = obj.AddComponent<Monster>();
                    obj.name = "Monster";
                    return ret;
                }
                break;
            case ObjectCode.DeadEffect:
                if (obj.GetComponent<DeathEffect>() == null)
                {
                    ret = obj.AddComponent<DeathEffect>();
                    obj.name = "DeathEffect";
                    return ret;
                }
                break;
            case ObjectCode.Arrow:
                if (obj.GetComponent<Arrow>() == null)
                {
                    ret = obj.AddComponent<Arrow>();
                    obj.name = "Arrow";
                    return ret;
                }
                break;
        }
        
        return null;
    }

    public static GameObject LoadGameObject(ObjectCode code)
    {
        GameObject go = null;

        switch (code)
        {
            case ObjectCode.Player:
                go = Resources.Load<GameObject>(ResourcePaths.Player_Prefab);
                break;
            case ObjectCode.Monster:
                go = Resources.Load<GameObject>(ResourcePaths.Monster_Prefab);
                break;
            case ObjectCode.DeadEffect:
                go = Resources.Load<GameObject>(ResourcePaths.DeathEffect_Prefab);
                break;
            case ObjectCode.Arrow:
                go = Resources.Load<GameObject>(ResourcePaths.Arrow_Prefab);
                break;
        }

        return go;
    }

    static void InitalizeBaseObject(BaseObject obj, ObjectInfo objInfo)
    {
        obj.ObjectInfo = objInfo;
        obj.MoveDir = objInfo.MoveDir;
        obj.State = objInfo.State;

        obj.CellPos = new Vector3Int(objInfo.Position.X, objInfo.Position.Y, 0);
        Vector3 pos = Manager.Map.CurrentGrid.CellToWorld(obj.CellPos) + new Vector3(0.5f, 0.5f);
        obj.transform.position = pos;
    }
}
#endregion