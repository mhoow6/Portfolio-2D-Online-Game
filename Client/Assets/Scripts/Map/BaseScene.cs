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
        Manager.Spawner.SpawnObject(ObjectCode.PLAYER); // 스폰 위치는 임의
        Manager.Spawner.SpawnObject(ObjectCode.MONSTER); // 스폰 위치는 임의
    }
}