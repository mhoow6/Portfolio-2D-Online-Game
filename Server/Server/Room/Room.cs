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

            ObjectCode code = PlayerManager.GetObjectCodeById(gameObject.objectInfo.ObjectId);

            // 랜덤 스폰을 위한 준비
            Random rnd = new Random(System.Environment.TickCount);
            int rndIndex = -1;
            SpawnPosInfo pos = SpawnPosInfo.Zero;

            switch (code)
            {
                case (ObjectCode.Player):
                    Player player = gameObject as Player;
                    _players.Add(gameObject.objectInfo.ObjectId, player);

                    // 1. 클라이언트에게 내가 들어왔다고 알림
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.objectInfo;

                    // 맵에 업데이트
                    Map.UpdatePosition(enterPacket.Player.Position, player);

                    player.session.Send(enterPacket);

                    /**************************************************************/

                    // 2. 현재 방에 있는 플레이어들의 정보를 내가 알아야 한다
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
                        {
                            spawnPacket.Objects.Add(p.objectInfo);
                        }
                    }
                    player.session.Send(spawnPacket);

                    /**************************************************************/

                    // 3. 랜덤 인덱스
                    rndIndex = rnd.Next(0, DataManager.Instance.DungeonPlayerSpawnPosition.Count - 1);
                    pos = DataManager.Instance.DungeonPlayerSpawnPosition[rndIndex];
                    break;
                case (ObjectCode.Monster):
                    break;
                case (ObjectCode.Arrow):
                    break;
            }

            // 4. 방에 있는 존재들에게 내가 스폰됬다고 알린다
            {
                S_Spawn spawnPacket = new S_Spawn();

                // 랜덤으로 정해진 위치를 패킷에 담는다.
                Vector2 position = new Vector2();
                position.X = pos.x;
                position.Y = pos.y;
                gameObject.objectInfo.Position = position;

                spawnPacket.Objects.Add(gameObject.objectInfo);
                foreach (Player p in _players.Values)
                {
                    // 나를 제외한 존재들에게만..
                    if (p.objectInfo.ObjectId != gameObject.objectInfo.ObjectId) 
                    {
                        p.session.Send(spawnPacket);
                    }
                }
            }
            
        }

        public void LeaveGame(int objectId)
        {
            ObjectCode code = PlayerManager.GetObjectCodeById(objectId);

            // [TODO] 방에 있는 오브젝트의 종류에 따라 할 일을 다르게 처리해야 함.
            switch (code)
            {
                case (ObjectCode.Player):
                    _players.Remove(objectId);
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
