using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MakeRoomUI : PopupUI
{
    [SerializeField]
    TMP_Dropdown _dropdown;
    MapId _mapId;

    private void Awake()
    {
        Uid = UI.MakeRoom;
    }

    public void GameStart()
    {
        // TODO: 패킷 보내기
        Manager.Network.SendCreateRoomPacket(_mapId);
    }

    public void ChangeMap()
    {
        _mapId = (MapId)_dropdown.value;
    }
}
