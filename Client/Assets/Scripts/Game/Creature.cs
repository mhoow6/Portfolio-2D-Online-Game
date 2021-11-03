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
        deadEffect.transform.position = transform.position; // ���� �ӿ����� ��ġ ����
        deadEffect.CellPos = CellPos; // ������ �Ѱ��� 2���� �迭������ ��ġ ����
        gameObject.SetActive(false);
    }
    #endregion
}
