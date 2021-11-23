using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class Creature : BaseObject
{
    public HpBar HpBar { get; protected set; }
    protected bool _attackOnce;

    public void AttachHpBar()
    {
        // HpBar �ޱ�
        GameObject obj = Resources.Load<GameObject>(ResourceLoadPath.HpBarPrefab);
        GameObject hpBar = Instantiate<GameObject>(obj);

        HpBar = hpBar.AddComponent<HpBar>();
        HpBar.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, 0);
        HpBar.transform.SetParent(transform);

        HpBar.SetOwner(this);
    }

    #region virtual
    public override void V_Dead()
    {
        BaseObject deadEffect = Manager.Spawner.SpawnObject(ObjectCode.DeadEffect);
        deadEffect.transform.position = transform.position; // ���� �ӿ����� ��ġ ����
        deadEffect.CellPos = CellPos; // ������ �Ѱ��� 2���� �迭������ ��ġ ����

        if (HpBar != null)
        {
            HpBar.Clear();
        }

        gameObject.SetActive(false);
    }
    #endregion
}
