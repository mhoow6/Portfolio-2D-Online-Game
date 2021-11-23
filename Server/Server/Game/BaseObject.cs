using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class BaseObject
    {
        public ObjectInfo objectInfo;
        public Room room;

        public Vector2 GetFrontCellPos()
        {
            Vector2 cellPos = objectInfo.Position;

            switch (objectInfo.MoveDir)
            {
                case MoveDir.Up:
                    {
                        Vector2 frontcell = new Vector2(cellPos);
                        frontcell.X += 0; frontcell.Y += 1;
                        cellPos = frontcell;
                    }
                    break;
                case MoveDir.Down:
                    {
                        Vector2 frontcell = new Vector2(cellPos);
                        frontcell.X += 0; frontcell.Y += -1;
                        cellPos = frontcell;
                    }
                    break;
                case MoveDir.Left:
                    {
                        Vector2 frontcell = new Vector2(cellPos);
                        frontcell.X += -1; frontcell.Y += 0;
                        cellPos = frontcell;
                    }
                    break;
                case MoveDir.Right:
                    {
                        Vector2 frontcell = new Vector2(cellPos);
                        frontcell.X += 1; frontcell.Y += 0;
                        cellPos = frontcell;
                    }
                    break;
            }

            return cellPos;
        }

        protected void SendMovePacket(Room here)
        {
            S_Move movePkt = new S_Move();
            movePkt.Objects.Add(objectInfo);
            here.BroadCast(movePkt);
        }

        #region virtual

        public virtual void V_UpdateObject()
        {
            switch (objectInfo.State)
            {
                case State.Idle:
                    V_UpdateIdle();
                    break;
                case State.Moving:
                    V_UpdateMoving();
                    break;
                case State.Attack:
                    V_UpdateAttack();
                    break;
                case State.Dead:
                    V_UpdateDead();
                    break;
            }
        }

        protected virtual void V_UpdateIdle() { }

        protected virtual void V_UpdateMoving() { }

        protected virtual void V_MoveToNextPos() { }

        protected virtual void V_UpdateAttack() { }

        protected virtual void V_UpdateDead() { }

        public virtual void V_Dead() { Console.WriteLine("V_Dead"); }
        #endregion
    }
}
