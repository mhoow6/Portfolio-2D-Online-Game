using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ObjectFactory
    {
        public static ObjectType GetObjectType(ObjectCode code)
        {
            if (code >= ObjectCode.ZeldaArcher && code < ObjectCode.ZeldaMonster)
            {
                return ObjectType.OtPlayer;
            }
            else if (code >= ObjectCode.ZeldaMonster && code < ObjectCode.Arrow)
            {
                return ObjectType.OtMonster;
            }
            else if (code >= ObjectCode.Arrow && code < ObjectCode.DeadEffect)
            {
                return ObjectType.OtProjectile;
            }
            else if (code >= ObjectCode.DeadEffect)
            {
                return ObjectType.OtEffect;
            }

            return ObjectType.OtNone;
        }

        public static int GetRespawnTime(ObjectCode code)
        {
            int respawnTime = 0; // ms

            ObjectType type = ObjectFactory.GetObjectType(code);
            switch (type)
            {
                case ObjectType.OtPlayer:
                    {
                        respawnTime = 5000;
                    }
                    break;
            }

            return respawnTime;
        }
    }

    public class ProjectileFactory
    {
        public static Projectile GetProjectile(ObjectCode code, Creature owner)
        {
            switch (code)
            {
                case ObjectCode.Arrow:
                    {
                        // arrow 소환 (TODO: 플레이어의 상태에 따라 소환가능한지 검증)
                        Arrow arrow = ObjectManager.Instance.Add<Arrow>(code);
                        arrow.V_SetOwner(owner);
                        return arrow;
                    }
            }

            return null;
        }
    }
}
