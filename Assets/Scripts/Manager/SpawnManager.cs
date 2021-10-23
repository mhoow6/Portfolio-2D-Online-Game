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
        // 1. 오브젝트가 이미 존재하는 경우
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

        // 2. 새로 오브젝트를 만들어야 하는 경우
        GameObject _obj = _objectFactory.LoadGameObject(type);
        GameObject obj = GameObject.Instantiate<GameObject>(_obj);
        obj.transform.SetParent(Manager.Instance.gameObject.transform);

        // 2-1. SpawnManager.objects에 추가
        if (_objects.TryGetValue(type, out var objList) == false) // 리스트가 한 번이라도 안 만들어졌다면 -> 최초 생성
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
