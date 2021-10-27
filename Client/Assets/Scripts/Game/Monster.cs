using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature
{
    public override ObjectType _type => ObjectType.MONSTER;

    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();

        // TEMP
        CellPos = Vector3Int.one;
        transform.position = Manager.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f, 0);
    }
}
