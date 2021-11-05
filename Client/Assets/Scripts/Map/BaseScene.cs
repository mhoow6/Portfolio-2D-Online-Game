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
        // [TODO] 플레이어가 방 생성(맵 포함) 요청을 하면 서버에서 방을 만들고 -> 게임시작할 때 로드
        Manager.Map.LoadMap(mapId);

        // 플레이어 스폰 요청
        {
            C_Spawn spawnPkt = new C_Spawn();
            spawnPkt.ObjectCode = (int)ObjectCode.Player;
            spawnPkt.ObjectId = -1;
            Manager.Network.Send(spawnPkt);
        }
    }
}