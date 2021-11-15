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
        deadEffect.transform.position = transform.position; // ���� �ӿ����� ��ġ ����
        deadEffect.CellPos = CellPos; // ������ �Ѱ��� 2���� �迭������ ��ġ ����

        // ������ ��û
        SpawnInfo spawnInfo = new SpawnInfo() { SpawnerId = ObjectInfo.ObjectId, RoomId = ObjectInfo.RoomId, ObjectCode = ObjectInfo.ObjectCode };
        Manager.Network.SendSpawnPacket(spawnInfo);

        gameObject.SetActive(false);
    }
    #endregion
}
