using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : PopupUI
{
    [SerializeField]
    RectTransform _content;
    [SerializeField]
    RoomElementUI _dummy;

    Dictionary<int, RoomElementUI> _rooms = new Dictionary<int, RoomElementUI>();
    RoomElementUI _selectedRoom;

    private void Awake()
    {
        Uid = PopUI.Lobby;
    }

    private void Start()
    {
        // ���̴� ������ ���Ǵ� UI�� �ƴ�. ����� �뵵
        _dummy.gameObject.SetActive(false);
    }

    // Make Room Button
    public void MakeRoom()
    {
        UIManager.Instance.OpenPopup(PopUI.MakeRoom);
    }

    // Join Room Button
    public void JoinRoom()
    {
        // �� �ε�
        Manager.Map.LoadMap((MapId)_selectedRoom.Info.MapId, _selectedRoom.Info.RoomId);
    }

    // RoomElementUI Button
    public void SelectRoom(RoomElementUI room)
    {
        _selectedRoom = room;
    }

    public void ShowRoom(S_ShowRoom packet)
    {
        foreach (var pkt in packet.Rooms)
        {
            RoomElementUI roomElement = null;
            // ���ο� �� ����
            if (_rooms.TryGetValue(pkt.RoomId, out roomElement) == false) 
            {
                // UI �ν��Ͻ�
                RoomElementUI newRoom = Instantiate(_dummy);

                // �� ���� �ʱ�ȭ
                newRoom.SetRoom(new RoomInfo() { MapId = pkt.MapId, RoomId = pkt.RoomId });

                newRoom.transform.SetParent(_content);
                newRoom.gameObject.SetActive(true);
                _rooms.Add(pkt.RoomId, newRoom);
            }
            // ���� �濡�� ������ �Ͼ�Ŷ��? -> ���� ���� ���߿� �ٲ�� ������ �����Ƿ� "������" �ƹ��͵� �� �ص� ������
        }

        // ���� �ֱ� ���߿� ����� �� ó��
        foreach (var roomId in packet.DeletedRooms)
        {
            RoomElementUI roomElement = null;
            if (_rooms.TryGetValue(roomId, out roomElement) == true)
            {
                // TODO: Ǯ��
                Destroy(roomElement.gameObject);
                _rooms.Remove(roomId);
            }
        }
        
    }
}
