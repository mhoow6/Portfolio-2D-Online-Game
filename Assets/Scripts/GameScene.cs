using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override MapId mapId { get => MapId.TOWN; }

    protected override void Init()
    {
        base.Init();

        Manager.Map.LoadMap(mapId);
    }
}
