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
        switch (pkt.ObjectInfo.ObjectCode)
        {
            case (int)ObjectCode.Player:
                break;
            case (int)ObjectCode.Monster:
                break;
        }
    }
}
