using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : BaseObject
{
    Animator _animator;
    public override Vector3Int CellPos { get => base.CellPos; set => base.CellPos = value; }

    private void Awake()
    {
        OnAwake();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StartCoroutine(AutoDisapper());
    }


    IEnumerator AutoDisapper()
    {
        while (true)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }
}
