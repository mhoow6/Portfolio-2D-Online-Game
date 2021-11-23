using Google.Protobuf.Protocol;
using SimpleJSON;
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

    public class MonsterFactory
    {
        public static StatInfo GetAoniInfo()
        {
            string txt = Util.GetLinesWithFileStream(ResourcePath.PlayerData);
            if (txt != null)
            {
                JSONNode root = JSON.Parse(txt);
                JSONNode stat = root["Stat"];

                StatInfo aoniStat = new StatInfo();
                aoniStat.Hp = int.Parse(stat["hp"]);
                aoniStat.Movespeed = int.Parse(stat["movespeed"]);
                aoniStat.OriginHp = aoniStat.Hp;
                aoniStat.WeaponId = int.Parse(stat["weaponId"]);
                return aoniStat;
            }
            return null;
        }
    }

    public class AttackHelper
    {
        public static void Attack(Creature attacker, Creature target, Action deadAfter = null)
        {
            Room room = attacker.room;
            if (room != null)
            {
                // 체력깎기
                Console.WriteLine($"Object({target.objectInfo.ObjectId}) got Damaged({attacker.Damage}) by Object({attacker.objectInfo.ObjectId})");
                target.objectInfo.Stat.Hp -= attacker.Damage; // TODO 데미지 공식

                // 체력을 깎았는데 죽어버렸으면?
                if (target.objectInfo.Stat.Hp <= 0)
                {
                    // 타겟이 죽었다고 사람들에게 알리기
                    Console.WriteLine($"Object({target.objectInfo.ObjectId}) dead by Object({attacker.objectInfo.ObjectId}) at [{DateTime.Now}]");
                    S_Dead deadPkt = new S_Dead();
                    deadPkt.ObjectId = target.objectInfo.ObjectId;
                    room.Push(room.BroadCast, deadPkt);

                    // 죽은 플레이어는 방에는 존재하지만 맵에는 존재하지 않아야 한다.
                    room.Map.RemoveCreature(target.objectInfo.Position);

                    // 죽은 플레이어는 리스폰 큐에 들어간다
                    room.Map.AddRespawn(target);

                    // 죽은 뒤의 콜백 함수
                    if (deadAfter != null)
                    {
                        deadAfter.Invoke();
                    }
                }
                else
                {
                    // 타겟이 맞았다고 사람들에게 알리기
                    S_Attack response = new S_Attack();
                    response.TargetInfo = target.objectInfo;
                    response.AttackerInfo = attacker.objectInfo;
                    response.Damage = attacker.Damage;
                    room.Push(room.BroadCast, response);
                }
            }
            
        }
    }
}
