using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;

namespace Server
{
    public class Room : JobSerializer
    {
        public int roomId;
        public Map Map { get; private set; } = new Map();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        public void Make(int mapId)
        {
            Map.LoadMap((MapId)mapId);
        }

        public void Update()
        {
            // [TODO] 방 안에 있는 모든 게임 오브젝트들을 갱신


            // 패킷으로 인해 오브젝트가 변동되는 일들을 처리한다.
            Flush();
        }

        public void EnterGame(BaseObject gameObject)
        {
            if (gameObject == null)
                return;

            ObjectCode code = ObjectManager.GetObjectCodeById(gameObject.id);

            // [TODO] 방에 들어온 오브젝트의 종류에 따라 할 일을 다르게 처리해야 함.
            switch (code)
            {
                case (ObjectCode.Player):
                    break;
                case (ObjectCode.Monster):
                    break;
                case (ObjectCode.Arrow):
                    break;
            }
        }

        public void LeaveGame(int objectId)
        {
            ObjectCode code = ObjectManager.GetObjectCodeById(objectId);

            // [TODO] 방에 있는 오브젝트의 종류에 따라 할 일을 다르게 처리해야 함.
            switch (code)
            {
                case (ObjectCode.Player):
                    break;
                case (ObjectCode.Monster):
                    break;
                case (ObjectCode.Arrow):
                    break;
            }
        }

        public void BroadCast(IMessage packet)
        {
            foreach (Player player in _players.Values)
            {
                player.session.Send(packet);
            }
        }
    }
}
