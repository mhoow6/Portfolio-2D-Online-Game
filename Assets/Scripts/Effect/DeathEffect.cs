using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    Animator _animator;

    private void Awake()
    {
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
