using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class Creature : BaseObject
{
    protected bool _attackOnce;

    #region virtual
    public override void V_Dead()
    {
        BaseObject deadEffect = Manager.Spawner.SpawnObject(ObjectCode.DeadEffect);
        deadEffect.transform.position = transform.position; // 게임 속에서의 위치 갱신
        deadEffect.CellPos = CellPos; // 서버로 넘겨줄 2차원 배열에서의 위치 갱신

        // 리스폰 요청
        SpawnInfo spawnInfo = new SpawnInfo() { SpawnerId = ObjectInfo.ObjectId, RoomId = ObjectInfo.RoomId, ObjectCode = ObjectInfo.ObjectCode };
        Manager.Network.SendSpawnPacket(spawnInfo);

        gameObject.SetActive(false);
    }
    #endregion
}
