using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server
{
    enum AoniPattern
    {
        Search,
        Trace
    }

    public class Aoni : Creature
    {
        const int SearchCellRange = 10;
        const int TraceCellRange = 20;
        long _nextSearchTick;
        long _nextMoveTick;
        AoniPattern _pattern;

        Player _target;
        Vector2 _targetDis
        {
            get => Vector2Helper.Minus(objectInfo.Position, _target.objectInfo.Position);
        }

        float _targetPowDistance
        {
            get
            {
                if (_targetDis != null)
                {
                    return Vector2Helper.PowMagnitude(_targetDis);
                }
                return 0;
            }
        }


        public override void V_UpdateObject()
        {
            base.V_UpdateObject();
        }

        protected override void V_UpdateIdle()
        {
            // 1초 단위로 시야 안에 있는 플레이어 찾기
            if (_nextSearchTick >= Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            if (FindPlayer() == true)
            {
                objectInfo.State = State.Moving;
                _pattern = AoniPattern.Trace;
            }
            else
            {
                /*objectInfo.State = State.Moving;
                _pattern = AoniPattern.Search;*/
            }
        }

        protected override void V_UpdateMoving()
        {
            // json 파일에서 정한 이동속도만큼 움직인다.
            long tick = (long)(1000 / objectInfo.Stat.Movespeed);
            _nextMoveTick = Environment.TickCount64 + tick;

            switch (_pattern)
            {
                case AoniPattern.Search:
                    {
                        // TODO: 서칭
                        Search();
                    }
                    break;
                case AoniPattern.Trace:
                    {
                        // TODO: 추적
                        Trace();
                    }
                    break;
                default:
                    break;
            }

            
        }

        // TODO: 시야각 적용
        bool FindPlayer()
        {
            _target = room.FindNearestPlayer(SearchCellRange, objectInfo.Position);
            if (_target != null)
            {
                Console.WriteLine($"Aoni({objectInfo.ObjectId}) find target({_target.objectInfo.ObjectId})");
                return true;
            }
            return false;
        }

        void Search()
        {
            if (_pattern != AoniPattern.Search)
                return;
        }

        void Trace()
        {
            if (_pattern != AoniPattern.Trace)
                return;

            // 예외처리: 쫓고 있는 플레이어가 나가거나 다른 방을 가면 종료
            if (_target == null || _target.room != room)
            {
                _target = null;
                objectInfo.State = State.Idle;

                // 이동 동기화
                SendMovePacket(room);

                return;
            }

            // TODO: 내 눈 앞에 바로 있으면 공격
            if (GetFrontCellPos() == _target.objectInfo.Position)
            {
                objectInfo.State = State.Idle;
                SendMovePacket(room);
                return;
            }

            // 거리가 멀어지면
            if (_targetPowDistance > MathF.Pow(TraceCellRange, 2))
            {
                // 추적 포기 후 원 상태로 복구
                _target = null;
                objectInfo.State = State.Idle;
            }
            else
            {
                // A*를 통해 길찾기 후 정해진 길을 걸어간다
                var path = _target.room.Map.FindPath(objectInfo.Position, _target.objectInfo.Position);

                if (path.Count != 0)
                {
                    // 열심히 길을 찾았지만 결국 바로 앞에만 가야함 (이동하면서 플레이어가 다른데로 가거나, 장애물이 생길 수 있기 때문)
                    objectInfo.Position = path[0];
                    // objectInfo.MoveDir =

                    // 클라이언트에게도 이동했다고 알림
                    SendMovePacket(room);
                    Console.WriteLine($"Aoni is moving to ({path[0].X},{path[0].Y})");
                }
                
            }
        }
    }
}
