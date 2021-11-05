using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class BaseObject
    {
        public ObjectCode code;
        public int id;
        public Vector2 position;
        public Room room;
    }
}
