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
        Code = UICode.MakeRoom;
    }

    public void GameStart()
    {
        Manager.Network.RequestCreateRoom(_mapId);
    }

    public void ChangeMap()
    {
        _mapId = (MapId)_dropdown.value;
    }
}
