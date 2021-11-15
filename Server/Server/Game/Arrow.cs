using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Arrow : Projectile
    {
        long _nextMoveTick;

        public override void V_SetOwner(Creature owner)
        {
            base.V_SetOwner(owner);

            int originId = objectInfo.ObjectId;

            objectInfo = owner.objectInfo;
            objectInfo.SpawnerId = owner.objectInfo.ObjectId;
            objectInfo.ObjectId = originId;
            objectInfo.State = State.Moving;
            objectInfo.ObjectCode = (int)ObjectCode.Arrow;
            objectInfo.Stat.Movespeed = DataManager.Instance.WeaponData.GetBowData(objectInfo.Stat.WeaponId).arrowspeed;
        }

        public override void V_UpdateObject()
        {
            // 실행할 시간이 아니면 종료
            if (_nextMoveTick >= Environment.TickCount64)
                return;

            base.V_UpdateObject();
        }

        protected override void V_UpdateMoving()
        {
            if (_owner.room != null)
            {
                long tick = (long)(1000 / objectInfo.Stat.Movespeed); // tick -> ms
                _nextMoveTick = Environment.TickCount64 + tick;

                Vector2 destPos = GetFrontCellPos();
                if (_owner.room.Map.CanGo(destPos)) // 앞에 뭔가 있나?
                {
                    objectInfo.Position = destPos; // 다음 Update에서 이동하자!

                    S_Move movePkt = new S_Move();
                    movePkt.Objects.Add(objectInfo);
                    _owner.room.BroadCast(movePkt);

                    Console.WriteLine($"Arrow is moving to ({destPos.X},{destPos.Y})");
                }
                else // 뭔가가 있다..
                {
                    Creature target = _owner.room.Map.CreatureAt(GetFrontCellPos());

                    if (target != null) // 적이라면?
                    {
                        // TODO: 공격 판정
                        target.V_Dead(); // 일단은 바로 죽게 하자.
                    }

                    // 어찌됐든 화살을 사라지게 함
                    _owner.room.C_LeaveGame(objectInfo.ObjectId);
                }
            }
        }

        protected override void V_MoveToNextPos()
        {
            base.V_MoveToNextPos();
        }
    }
}
