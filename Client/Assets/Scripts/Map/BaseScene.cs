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
        Manager.Map.LoadMap(mapId);

        // �÷��̾� ���� ��û
        {
            C_Respawn spawnPkt = new C_Respawn();
            spawnPkt.ObjectCode = (int)ObjectCode.Player;
            Manager.Network.Send(spawnPkt);
        }

        // ���� ���� ��û
        {
            C_Respawn spawnPkt = new C_Respawn();
            spawnPkt.ObjectCode = (int)ObjectCode.Monster;
            Manager.Network.Send(spawnPkt);
        }
    }
}