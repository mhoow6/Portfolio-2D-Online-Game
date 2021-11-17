using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : PopupUI
{
    private void Awake()
    {
        Uid = UI.Lobby;
    }

    public void MakeRoom()
    {
        UIManager.Instance.OpenPopup(UI.MakeRoom);
    }

    public void JoinRoom()
    {
        UIManager.Instance.OpenPopup(UI.JoinRoom);
    }
}
