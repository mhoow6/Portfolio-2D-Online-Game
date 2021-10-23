using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{

    protected override void V_OnAwake()
    {
        base.V_OnAwake();
    }

    protected override void V_OnStart()
    {
        CellPos = Manager.Map.CurrentGrid.WorldToCell(Vector3.one);

        base.V_OnStart();
    }

    protected override void V_OnUpdate()
    {
        base.V_OnUpdate();
    }
}
