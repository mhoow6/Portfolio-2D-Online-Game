using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TownScene : BaseScene
{
    public override MapId mapId { get => MapId.TOWN; }

    protected override void Init()
    {
        base.Init();

        Manager.Map.LoadMap(mapId);
        Manager.Spawner.SpawnObject(ObjectCode.PLAYER);
        Manager.Spawner.SpawnObject(ObjectCode.MONSTER);
    }
}
