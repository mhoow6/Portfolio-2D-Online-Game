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
        // ���� �����ִ� UI�� ���ÿ�
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

        // TODO: ������ �˾��� �̹� ������������ ��Ȱ���ϱ�
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

            // ���� ���������� UI �˾��� ���� ���� ���ÿ� push
            if (popup != null)
            {
                _openPopups.Push(popup);
            }
            
            // ���߾ӿ� �˾� ����
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
