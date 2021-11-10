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
                        frontcell.X += 0; frontcell.Y += 1;
                        cellPos = frontcell;
                    }
                    break;
            }

            return cellPos;
        }
    }
}
