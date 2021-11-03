using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
    protected bool _attackOnce;

    #region virtual
    public virtual void V_Dead()
    {
        BaseObject deadEffect = Manager.Spawner.SpawnObject(ObjectCode.DEAD_EFFECT);
        deadEffect.transform.position = transform.position; // 게임 속에서의 위치 갱신
        deadEffect.CellPos = CellPos; // 서버로 넘겨줄 2차원 배열에서의 위치 갱신
        gameObject.SetActive(false);
    }
    #endregion
}
