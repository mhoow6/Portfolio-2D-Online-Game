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
        public int RoomId { get; private set; } = 1;
        public Queue<int> DeletedRooms { get; private set; } = new Queue<int>();

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
                DeletedRooms.Enqueue(roomId);
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
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine($"Updating Room List.. ({DateTime.Now})");
            Console.WriteLine("-------------------------------------------------------");
            foreach (var session in SessionManager.Instance.Sessions.Values)
            {
                S_ShowRoom pkt = new S_ShowRoom();

                // 현재 존재하는 방 리스트
                foreach (var room in Rooms.Values)
                {
                    RoomInfo info = new RoomInfo();
                    info.MapId = (int)room.Map.Id;
                    info.RoomId = room.roomId;
                    pkt.Rooms.Add(info);
                }

                while (DeletedRooms.Count != 0)
                {
                    pkt.DeletedRooms.Add(DeletedRooms.Dequeue());
                }

                session.Send(pkt);
            }
        }
    }
}
