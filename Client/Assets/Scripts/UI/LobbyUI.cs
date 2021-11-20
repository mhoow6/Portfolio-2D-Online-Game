using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public UICode Code { get; private set; }

    [SerializeField]
    RectTransform _content;
    [SerializeField]
    RoomElementUI _dummy;

    Dictionary<int, RoomElementUI> _rooms = new Dictionary<int, RoomElementUI>();
    RoomElementUI _selectedRoom;

    private void Awake()
    {
        Code = UICode.Lobby;
        UIManager.Instance.Lobby = this;
    }

    private void Start()
    {
        // 더미는 실제로 사용되는 UI가 아님. 만들기 용도
        _dummy.gameObject.SetActive(false);
    }

    // Make Room Button
    public void MakeRoom()
    {
        UIManager.Instance.OpenPopup(UICode.MakeRoom);
    }

    // Join Room Button
    public void JoinRoom()
    {
        // 맵 로드
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
            // 새로운 방 생성
            if (_rooms.TryGetValue(pkt.RoomId, out roomElement) == false) 
            {
                // UI 인스턴싱
                RoomElementUI newRoom = Instantiate(_dummy);

                // 방 설정 초기화
                newRoom.SetRoom(new RoomInfo(pkt));

                newRoom.transform.SetParent(_content);
                newRoom.gameObject.SetActive(true);
                _rooms.Add(pkt.RoomId, newRoom);
            }
            else
            {
                // 플레이어 수 갱신
                roomElement.SetRoom(new RoomInfo(pkt));
            }
        }

        // 갱신 주기 도중에 사라진 방 처리
        foreach (var roomId in packet.DeletedRooms)
        {
            RoomElementUI roomElement = null;
            if (_rooms.TryGetValue(roomId, out roomElement) == true)
            {
                Destroy(roomElement.gameObject);
                _rooms.Remove(roomId);
            }
        }
        
    }
}
