using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UICode
{
    Login,
    Lobby,
    MakeRoom,
    JoinRoom,
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    public LobbyUI Lobby { get; set; }

    Stack<PopupUI> _openPopups = new Stack<PopupUI>();
    Dictionary<UICode, PopupUI> _pendingPopups = new Dictionary<UICode, PopupUI>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    public void OpenPopup(UICode uid)
    {
        #region 재활용이 가능하면 재활용
        PopupUI exist = null;
        if (_pendingPopups.TryGetValue(uid, out exist) == true)
        {
            exist.gameObject.SetActive(true);

            // 팝업은 맨 앞에 있어야 한다
            exist.transform.SetSiblingIndex(transform.childCount - 1); 

            // 대기중인 팝업이 아니므로 제거
            _pendingPopups.Remove(uid);

            // 팝업 스택에 추가
            _openPopups.Push(exist);
            return;
        }
        #endregion

        #region 오브젝트 새로 생성
        GameObject _obj = null;
        _obj = UIFactory.LoadGameObject(uid);
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
        #endregion
    }

    public void ClosePopup()
    {
        if (_openPopups.Count != 0)
        {
            PopupUI lastPopup = _openPopups.Pop();
            _pendingPopups.Add(lastPopup.Code, lastPopup);
            lastPopup.gameObject.SetActive(false);
        }
        
    }

    public void ClosePopupAll()
    {
        while (_openPopups.Count != 0)
        {
            ClosePopup();
        }
    }

    public void CloseAll()
    {
        ClosePopupAll();

        Lobby.gameObject.SetActive(false);
    }

    public PopupUI FindPopup(UICode ui)
    {
        foreach (var pop in _openPopups)
        {
            if (pop.Code == ui)
                return pop;
        }
        return null;
    }
}
