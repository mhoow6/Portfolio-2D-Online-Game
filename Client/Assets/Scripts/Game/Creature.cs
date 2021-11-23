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
        // HpBar 달기
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
        deadEffect.transform.position = transform.position; // 게임 속에서의 위치 갱신
        deadEffect.CellPos = CellPos; // 서버로 넘겨줄 2차원 배열에서의 위치 갱신

        if (HpBar != null)
        {
            HpBar.Clear();
        }

        gameObject.SetActive(false);
    }
    #endregion
}
