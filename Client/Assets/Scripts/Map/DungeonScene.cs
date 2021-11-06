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

        // [TODO] �÷��̾ �� ����(�� ����) ��û�� �ϸ� �������� ���� ����� -> ���ӽ����� �� �ε�
        Manager.Map.LoadMap(mapId);
    }
}
