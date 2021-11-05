using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public abstract class BaseScene : MonoBehaviour
{
    public abstract MapId mapId
    {
        get;
    }

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        // [TODO] �÷��̾ �� ����(�� ����) ��û�� �ϸ� �������� ���� ����� -> ���ӽ����� �� �ε�
        Manager.Map.LoadMap(mapId);

        // �÷��̾� ���� ��û
        {
            C_Spawn spawnPkt = new C_Spawn();
            spawnPkt.ObjectCode = (int)ObjectCode.Player;
            spawnPkt.ObjectId = -1;
            Manager.Network.Send(spawnPkt);
        }
    }
}