using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SpawnManager
{
    ObjectFactory _objectFactory = new ObjectFactory();
    Dictionary<ObjectType, List<GameObject>> _objects = new Dictionary<ObjectType, List<GameObject>>();

    public GameObject SpawnObject(ObjectType type)
    {
        // 1. ������Ʈ�� �̹� �����ϴ� ���
        if (_objects.TryGetValue(type, out var list) == true)
        {
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.gameObject.activeSelf == false)
                    {
                        item.gameObject.SetActive(true);
                        return item;
                    }
                }
            }
        }

        // 2. ���� ������Ʈ�� ������ �ϴ� ���
        GameObject _obj = _objectFactory.LoadGameObject(type);
        GameObject obj = GameObject.Instantiate<GameObject>(_obj);
        obj.transform.SetParent(Manager.Instance.gameObject.transform);

        // 2-1. SpawnManager.objects�� �߰�
        if (_objects.TryGetValue(type, out var objList) == false) // ����Ʈ�� �� ���̶� �� ��������ٸ� -> ���� ����
        {
            objList = new List<GameObject>();
            objList.Add(obj);
            _objects.Add(type, objList);
            _objectFactory.AddComponentToObject(type, obj);
        }
        else
        {
            objList.Add(obj);
        }

        return obj;
    }
}
