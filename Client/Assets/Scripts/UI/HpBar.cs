using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    Creature _owner;
    Transform _hp;
    public int originHp;
    public int currentHp;

    private void Awake()
    {
        _hp = Util.FindChild<Transform>(gameObject, "Hp", true);
    }

    public void SetOwner(Creature owner)
    {
        if (owner.ObjectInfo.Stat != null)
        {
            _owner = owner;
            originHp = owner.ObjectInfo.Stat.OriginHp;
            currentHp = originHp;
        }
    }

    public void UpdateHpBar()
    {
        if (_owner != null)
        {
            currentHp = _owner.ObjectInfo.Stat.Hp;
            float ratio = (float)currentHp / originHp;

            // HP가 줄어들 때 Bar가 왼쪽으로 줄어들기
            _hp.localPosition = new Vector3(0.4f * (ratio-1), 0, 0);
            _hp.localScale = new Vector3(ratio, 0.1f, 1);
        }
    }

    public void Clear()
    {
        _hp.localScale = new Vector3(1, 1, 1);
        originHp = 0;
        currentHp = 0;
    }
}
