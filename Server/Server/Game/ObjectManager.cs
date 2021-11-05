﻿using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server
{
    // 오브젝트 관리자
    public class ObjectManager : SingleTon<ObjectManager>
    {
        public static readonly int Unknown = -1;

        object _lock = new object();
        Dictionary<int, BaseObject> _objects = new Dictionary<int, BaseObject>();

        // UnUsed(1)ObjectCode(7)ObjectId(23)
        // [ ........ | ........ | ........ | ........ ]
        public int ObjId { get; private set; }

        public BaseObject Add(ObjectCode code, Vector2 position)
        {
            BaseObject gameObject = new BaseObject();

            lock (_lock)
            {
                gameObject.code = code;
                gameObject.position = position;
                gameObject.id = GenerateId(code);
                _objects.Add(gameObject.id, gameObject);
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

        public BaseObject Find(int objectId)
        {
            lock (_lock)
            {
                if (_objects.TryGetValue(objectId, out BaseObject obj))
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
                return ((int)code << 24) | (ObjId++);
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
