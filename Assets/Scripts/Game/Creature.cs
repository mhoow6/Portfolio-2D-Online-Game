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
        deadEffect.transform.position = transform.position; // ���� �ӿ����� ��ġ ����
        deadEffect.CellPos = CellPos; // ������ �Ѱ��� 2���� �迭������ ��ġ ����
        gameObject.SetActive(false);
    }
    #endregion
}
