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
        #region ��Ȱ���� �����ϸ� ��Ȱ��
        PopupUI exist = null;
        if (_pendingPopups.TryGetValue(uid, out exist) == true)
        {
            exist.gameObject.SetActive(true);

            // �˾��� �� �տ� �־�� �Ѵ�
            exist.transform.SetSiblingIndex(transform.childCount - 1); 

            // ������� �˾��� �ƴϹǷ� ����
            _pendingPopups.Remove(uid);

            // �˾� ���ÿ� �߰�
            _openPopups.Push(exist);
            return;
        }
        #endregion

        #region ������Ʈ ���� ����
        GameObject _obj = null;
        _obj = UIFactory.LoadGameObject(uid);
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
