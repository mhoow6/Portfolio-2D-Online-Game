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
    public static void C_RespawnHandler(PacketSession session, IMessage packet)
    {
        C_Respawn pkt = packet as C_Respawn;

        // Objectcode에 따라 스폰 지점 결정
    }
}
