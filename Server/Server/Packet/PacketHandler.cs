using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

// 네트워크 패킷이 조립되는 이 공간은 레드존임을 기억하자.
class PacketHandler
{
    public static void C_SpawnHandler(PacketSession session, IMessage packet)
    {
        C_Spawn pkt = packet as C_Spawn;
        
        ClientSession Session = session as ClientSession;

        // Objectcode에 따라 스폰 지점 결정
        switch (pkt.ObjectCode)
        {
            case (int)ObjectCode.Player:
                // 랜덤 스폰 위치
                Random rnd = new Random();
                int rndIndex = rnd.Next(0, DataManager.Instance.DungeonPlayerSpawnPosition.Count - 1);
                SpawnPosInfo pos = DataManager.Instance.DungeonPlayerSpawnPosition[rndIndex];
                Vector2 position = new Vector2();
                position.X = pos.x;
                position.Y = pos.y;

                // 서버의 오브젝트 관리자에 갱신
                BaseObject obj = null;
                if (pkt.ObjectId == ObjectManager.Unknown) // 처음 소환되는 것이라면..
                    obj = ObjectManager.Instance.Add((ObjectCode)pkt.ObjectCode, position);
                else
                    obj = ObjectManager.Instance.Find(pkt.ObjectId);

                // 서버의 방의 맵에 플레이어 갱신
                obj.room.Push(obj.room.Map.UpdatePosition, position, obj);
                
                // 클라이언트에게 재전송
                S_Spawn response = new S_Spawn();
                response.Position = position;
                Session.Send(response);

                // [TODO] 같은 방 안에 있는 플레이어들에게 알림
                break;
            case (int)ObjectCode.Monster:
                break;
        }
    }
}
