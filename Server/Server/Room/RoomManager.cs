using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class RoomManager : SingleTon<RoomManager>
    {
        object _lock = new object();
        public Dictionary<int, Room> Rooms { get; private set; } =
        new Dictionary<int, Room>();
        Queue<int> _deletedRooms = new Queue<int>();

        public int RoomId { get; private set; } = 1;
        
        public Room Add(int mapId)
        {
            Room gameRoom = new Room();
            gameRoom.Push(gameRoom.Make, mapId);
            
            lock (_lock)
            {
                gameRoom.roomId = RoomId;
                Rooms.Add(RoomId, gameRoom);
                RoomId++;
            }

            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                _deletedRooms.Enqueue(roomId);
                return Rooms.Remove(roomId);
            }
        }

        public Room Find(int roomId)
        {
            lock (_lock)
            {
                Room room = null;
                if (Rooms.TryGetValue(roomId, out room))
                    return room;

                return null;
            }
        }

        public void RoomListUpdate()
        {
            if (SessionManager.Instance.LobbySessions.Count > 0)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine($"Updating room list for {SessionManager.Instance.LobbySessions.Count} Sessions. ({DateTime.Now})");
                Console.WriteLine("-------------------------------------------------------");
                foreach (var session in SessionManager.Instance.LobbySessions.Values)
                {
                    S_ShowRoom pkt = new S_ShowRoom();

                    // 현재 존재하는 방 리스트
                    foreach (var room in Rooms.Values)
                    {
                        RoomInfo info = new RoomInfo();
                        info.MapId = (int)room.Map.Id;
                        info.RoomId = room.roomId;
                        info.Players = room.PlayerCount;
                        pkt.Rooms.Add(info);
                    }

                    while (_deletedRooms.Count != 0)
                    {
                        pkt.DeletedRooms.Add(_deletedRooms.Dequeue());
                    }

                    session.Send(pkt);
                }
            }
        }
            
    }
}
