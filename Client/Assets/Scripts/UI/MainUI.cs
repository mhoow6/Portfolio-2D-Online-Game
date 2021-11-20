using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    public void JoinLobby()
    {
        GameObject lobby = Instantiate(UIFactory.LoadGameObject(UICode.Lobby));
        lobby.transform.SetParent(UIManager.Instance.transform);

        // Á¤Áß¾Ó¿¡ ÆË¾÷ ¶ç¿ì±â
        lobby.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        gameObject.SetActive(false);
    }
}
