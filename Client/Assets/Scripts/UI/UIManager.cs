using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PopUI
{
    Login,
    Lobby,
    MakeRoom,
    JoinRoom,
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    Stack<PopupUI> _openPopups = new Stack<PopupUI>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 현재 열려있는 UI를 스택에
        for (int i = 0; i < transform.childCount; i++)
        {
            PopupUI cur = transform.GetChild(i).GetComponent<PopupUI>();
            if (cur != null && cur.gameObject.activeSelf == true)
            {
                _openPopups.Push(cur);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    public void OpenPopup(PopUI ui)
    {
        GameObject _obj = null;

        // TODO: 기존에 팝업이 이미 생성되있으면 재활용하기
        PopupUI exist = FindPopup(ui);
        if (exist != null)
        {
            return;
        }

        switch (ui)
        {
            case PopUI.Login:
                {
                    _obj = Resources.Load<GameObject>(ResourceLoadPath.LoginPrefab);
                }
                break;
            case PopUI.Lobby:
                {
                    _obj = Resources.Load<GameObject>(ResourceLoadPath.LobbyPrefab);
                }
                break;
            case PopUI.MakeRoom:
                {
                    _obj = Resources.Load<GameObject>(ResourceLoadPath.MakeRoomPrefab);
                }
                break;
            default:
                break;
        }

        if (_obj != null)
        {
            GameObject obj = Instantiate(_obj);
            obj.transform.SetParent(transform);
            PopupUI popup = obj.GetComponent<PopupUI>();

            // 추후 순차적으로 UI 팝업을 끄기 위해 스택에 push
            if (popup != null)
            {
                _openPopups.Push(popup);
            }
            
            // 정중앙에 팝업 띄우기
            obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    public void ClosePopup()
    {
        if (_openPopups.Count != 0)
        {
            PopupUI lastLayer = _openPopups.Pop();
            lastLayer.gameObject.SetActive(false);
        }
        
    }

    public void ClosePopupAll()
    {
        while (_openPopups.Count != 0)
        {
            ClosePopup();
        }
    }

    public PopupUI FindPopup(PopUI ui)
    {
        foreach (var pop in _openPopups)
        {
            if (pop.Uid == ui)
                return pop;
        }
        return null;
    }
}
