using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ObjectFactory
    {
        public ObjectType GetObjectType(ObjectCode code)
        {
            if (code >= ObjectCode.Player && code < ObjectCode.Monster)
            {
                return ObjectType.OtPlayer;
            }

            if (code >= ObjectCode.Monster && code < ObjectCode.Arrow)
            {
                return ObjectType.OtMonster;
            }

            if (code >= ObjectCode.Arrow && code < ObjectCode.DeadEffect)
            {
                return ObjectType.OtProjectile;
            }

            return ObjectType.OtNone;
        }
    }
}
