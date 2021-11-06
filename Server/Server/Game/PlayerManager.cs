using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server
{
    // 오브젝트 관리자
    public class PlayerManager : SingleTon<PlayerManager>
    {
        public static readonly int Unknown = -1;

        object _lock = new object();
        Dictionary<int, Player> _objects = new Dictionary<int, Player>();

        // UnUsed(1)ObjectCode(7)ObjectId(23)
        // [ ........ | ........ | ........ | ........ ]
        public int PlayerCount { get; private set; }

        public T Add<T>(ObjectCode code) where T : BaseObject, new()
        {
            T gameObject = new T();

            lock (_lock)
            {
                gameObject.objectInfo = new ObjectInfo();
                gameObject.objectInfo.ObjectCode = (int)code;
                gameObject.objectInfo.ObjectId = GenerateId(code);

                if (code == ObjectCode.Player || code == ObjectCode.Other)
                    _objects.Add(gameObject.objectInfo.ObjectId, gameObject as Player);
            }

            return gameObject;
        }

        public bool Remove(int objectId)
        {
            lock (_lock)
            {
                if (_objects.TryGetValue(objectId, out _))
                {
                    _objects.Remove(objectId);
                    return true;
                }
            }

            return false;
        }

        public Player Find(int objectId)
        {
            lock (_lock)
            {
                if (_objects.TryGetValue(objectId, out Player obj))
                    return obj;
            }

            return null;
        }

        int GenerateId(ObjectCode code)
        {
            // code       00000000 00000000 00000000 00000010
            // code << 24 00000010 00000000 00000000 00000000 [A]
            // ObjCount	  00000000 00000000 00000000 00000000 [B]
            // A OR B	  00000010 00000000 00000000 00000000 [ID]

            lock (_lock)
            {
                return ((int)code << 24) | (PlayerCount++);
            }
        }

        public static ObjectCode GetObjectCodeById(int id)
        {
            // id         00000010 00000000 00000000 00000000
            // id >> 24   00000000 00000000 00000000 00000010 [A]
            // 0x7F	      00000000 00000000 00000000 01111111 [B]
            // A AND B	  00000000 00000000 00000000 00000010 [TYPE]

            int code = (id >> 24) & 0x7F;
            return (ObjectCode)code;
        }
    }
}
