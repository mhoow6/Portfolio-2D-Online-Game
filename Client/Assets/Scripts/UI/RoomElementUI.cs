using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomElementUI : MonoBehaviour
{
    [SerializeField]
    Image _mapThumbnail;

    public RoomInfo Info { get; private set; } = new RoomInfo();

    public void SetRoom(RoomInfo roomInfo)
    {
        Info = roomInfo;
        _mapThumbnail.sprite = MapFactory.GetMapThumbnail((MapId)roomInfo.MapId);
    }
}
