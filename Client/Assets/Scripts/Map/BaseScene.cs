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
        
    }
}