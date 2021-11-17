using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Protocol;

public class Other : Player
{
    private void Awake()
    {
        OnAwake();
    }

    private void OnEnable()
    {
        if (HpBar != null)
        {
            HpBar.SetOwner(this);
        }
    }

    private void Start()
    {
        // �ʱ⿡ ������� ȭ�� ���� ���� ������ �̸� ��Ȱ��ȭ�� ȭ���� ������.
        {
            List<BaseObject> arrows = new List<BaseObject>();
            for (int i = 0; i < 10; i++)
            {
                var obj = Manager.Spawner.SpawnObject(ObjectCode.Arrow);
                arrows.Add(obj);
            }
            foreach (var arrow in arrows)
            {
                arrow.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        V_UpdateObject();
    }

    protected override void V_UpdateObject()
    {
        switch (State)
        {
            case State.Idle:
                V_UpdateIdle();
                break;
            case State.Moving:
                V_UpdateMoving();
                break;
            case State.Attack:
                V_UpdateAttack();
                break;
            case State.Dead:
                V_UpdateDead();
                break;
        }

        HpBar.UpdateHpBar();
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
