using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SpawnManager
{
    Dictionary<ObjectCode, List<BaseObject>> _objects = new Dictionary<ObjectCode, List<BaseObject>>();
    
    public BaseObject SpawnObject(ObjectCode code)
    {
        // 1. ������Ʈ�� �̹� �����ϴ� ���
        if (_objects.TryGetValue(code, out var list) == true)
        {
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.gameObject.activeSelf == false && item.code == code)
                    {
                        item.gameObject.SetActive(true);
                        return item;
                    }
                }
            }
        }

        // 2. ���� ������Ʈ�� ������ �ϴ� ���
        GameObject __obj = ObjectFactory.LoadGameObject(code);
        GameObject _obj = GameObject.Instantiate<GameObject>(__obj);
        _obj.transform.SetParent(Manager.Instance.Pool.transform);

        // 2-1. SpawnManager.objects�� �߰�
        if (_objects.TryGetValue(code, out var objList) == false) // ����Ʈ�� �� ���̶� �� ��������ٸ� -> ���� ����
        {
            objList = new List<BaseObject>();
            _objects.Add(code, objList);
        }
        BaseObject obj = ObjectFactory.AddComponentToObject(code, _obj);
        obj.code = code;
        objList.Add(obj);
        
        return obj;
    }

    public BaseObject SpawnObject(ObjectInfo objInfo)
    {
        // 1. ������Ʈ�� �̹� �����ϴ� ���
        if (_objects.TryGetValue((ObjectCode)objInfo.ObjectCode, out var list) == true)
        {
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.gameObject.activeSelf == false && item.code == (ObjectCode)objInfo.ObjectCode)
                    {
                        item.gameObject.SetActive(true);
                        item.id = objInfo.ObjectId;
                        return item;
                    }
                }
            }
        }

        // 2. ���� ������Ʈ�� ������ �ϴ� ���
        GameObject __obj = ObjectFactory.LoadGameObject((ObjectCode)objInfo.ObjectCode);
        GameObject _obj = GameObject.Instantiate<GameObject>(__obj);
        _obj.transform.SetParent(Manager.Instance.Pool.transform);

        // 2-1. SpawnManager.objects�� �߰�
        if (_objects.TryGetValue((ObjectCode)objInfo.ObjectCode, out var objList) == false) // ����Ʈ�� �� ���̶� �� ��������ٸ� -> ���� ����
        {
            objList = new List<BaseObject>();
            _objects.Add((ObjectCode)objInfo.ObjectCode, objList);
        }
        BaseObject obj = ObjectFactory.AddComponentToObject(objInfo, _obj);

        // 2-2 ��� ���� �ʱ�ȭ
        obj.ObjectInfo = objInfo;

        objList.Add(obj);

        return obj;
    }
}
