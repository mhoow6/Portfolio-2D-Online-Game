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
        // 더미는 실제로 사용되는 UI가 아님. 만들기 용도
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
                newRoom.SetRoom(new RoomInfo() { MapId = pkt.MapId, RoomId = pkt.RoomId });

                newRoom.transform.SetParent(_content);
                newRoom.gameObject.SetActive(true);
                _rooms.Add(pkt.RoomId, newRoom);
            }
            // 기존 방에서 갱신이 일어난거라면? -> 게임 시작 도중에 바뀌는 정보가 없으므로 "지금은" 아무것도 안 해도 괜찮음
        }

        // 갱신 주기 도중에 사라진 방 처리
        foreach (var roomId in packet.DeletedRooms)
        {
            RoomElementUI roomElement = null;
            if (_rooms.TryGetValue(roomId, out roomElement) == true)
            {
                // TODO: 풀링
                Destroy(roomElement.gameObject);
                _rooms.Remove(roomId);
            }
        }
        
    }
}
