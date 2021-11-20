using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomElementUI : MonoBehaviour
{
    [SerializeField]
    Image _mapThumbnail;
    [SerializeField]
    TMP_Text _playerCount;

    public RoomInfo Info { get; private set; } = new RoomInfo();

    public void SetRoom(RoomInfo roomInfo)
    {
        Info = roomInfo;
        _playerCount.text = $"플레이어 {roomInfo.Players}";
        _mapThumbnail.sprite = MapFactory.GetMapThumbnail((MapId)roomInfo.MapId);
    }
}
