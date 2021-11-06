using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScene : BaseScene
{
    public override MapId mapId { get => MapId.Dungeon; }

    protected override void Init()
    {
        Screen.SetResolution(640, 480, false);

        // [TODO] 플레이어가 방 생성(맵 포함) 요청을 하면 서버에서 방을 만들고 -> 게임시작할 때 로드
        Manager.Map.LoadMap(mapId);
    }
}
