using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Player : Creature
    {
        public ClientSession session;

        public Player(ObjectCode code)
        {
            this.code = code;
        }
    }
}
