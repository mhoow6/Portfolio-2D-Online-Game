using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class RoomManager : SingleTon<RoomManager>
    {
        object _lock = new object();
        Dictionary<int, Room> _rooms = new Dictionary<int, Room>();
        public int RoomId { get; private set; } = 1;

        public Room Add(int mapId)
        {
            Room gameRoom = new Room();
            gameRoom.Push(gameRoom.Make, mapId);
            
            lock (_lock)
            {
                gameRoom.roomId = RoomId;
                _rooms.Add(RoomId, gameRoom);
                RoomId++;
            }

            Console.WriteLine($"Room Load Completed. : {RoomId}");
            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }
        }

        public Room Find(int roomId)
        {
            lock (_lock)
            {
                Room room = null;
                if (_rooms.TryGetValue(roomId, out room))
                    return room;

                return null;
            }
        }
    }
}
