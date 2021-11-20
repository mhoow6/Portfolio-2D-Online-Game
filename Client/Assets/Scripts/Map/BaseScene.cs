using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class BaseScene : MonoBehaviour
{
    public MapId MapId { get; private set; }
    public int RoomId { get; private set; }

    public void Initalize(MapId mapId, int roomId)
    {
        MapId = mapId;
        RoomId = roomId;
    }
}