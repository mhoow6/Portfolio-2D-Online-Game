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

        // 플레이어 스폰 요청
        {
            C_Respawn spawnPkt = new C_Respawn();
            spawnPkt.ObjectCode = (int)ObjectCode.Player;
            Manager.Network.Send(spawnPkt);
        }

        // 몬스터 스폰 요청
        {
            C_Respawn spawnPkt = new C_Respawn();
            spawnPkt.ObjectCode = (int)ObjectCode.Monster;
            Manager.Network.Send(spawnPkt);
        }
    }
}