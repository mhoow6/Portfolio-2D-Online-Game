using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class BaseScene : MonoBehaviour
{
    MapId _mapId;
    int _roomId;

    public void Initalize(MapId mapId, int roomId)
    {
        _mapId = mapId;
        _roomId = roomId;

        // ���� �������� �÷��̾� ���� ��û
        Manager.Network.RequestEnterGame(new RoomInfo() { MapId = (int)_mapId, RoomId = _roomId });
    }
}