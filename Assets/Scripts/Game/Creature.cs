using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
    #region Virtual Functions
    public virtual void V_OnDead()
    {
        BaseObject deadEffect = Manager.Spawner.SpawnObject(ObjectType.EFFECT);
        deadEffect.transform.position = transform.position; // 게임 속에서의 위치 갱신
        deadEffect.CellPos = CellPos; // 서버로 넘겨줄 2차원 배열에서의 위치 갱신
        gameObject.SetActive(false);
    }
    #endregion
}
