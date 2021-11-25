using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아오오니는 AI
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
        // State, MoveDir, CellPos을 패킷으로 조작하므로 굳이 개별 클라에서 컨트롤할 필요없음
    }

    protected override void V_MoveToNextPos()
    {
        // State, MoveDir, CellPos을 패킷으로 조작하므로 굳이 개별 클라에서 컨트롤할 필요없음
    }

    protected override void V_UpdateAttack()
    {
        // State, MoveDir, CellPos을 패킷으로 조작하므로 굳이 개별 클라에서 컨트롤할 필요없음
    }
}
