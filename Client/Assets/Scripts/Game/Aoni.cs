using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ƿ����ϴ� AI
public class Aoni : Creature
{
    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        V_UpdateObject();
        Debug.Log($"{State}");
    }

    protected override void V_UpdateIdle()
    {
        // State, MoveDir, CellPos�� ��Ŷ���� �����ϹǷ� ���� ���� Ŭ�󿡼� ��Ʈ���� �ʿ����
    }

    protected override void V_MoveToNextPos()
    {
        // State, MoveDir, CellPos�� ��Ŷ���� �����ϹǷ� ���� ���� Ŭ�󿡼� ��Ʈ���� �ʿ����
    }

    protected override void V_UpdateAttack()
    {
        // State, MoveDir, CellPos�� ��Ŷ���� �����ϹǷ� ���� ���� Ŭ�󿡼� ��Ʈ���� �ʿ����
    }
}
