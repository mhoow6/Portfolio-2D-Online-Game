using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SpawnManager
{
    ObjectFactory _objectFactory = new ObjectFactory();
    Dictionary<ObjectType, List<BaseObject>> _objects = new Dictionary<ObjectType, List<BaseObject>>();

    public BaseObject SpawnObject(ObjectType type)
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
        GameObject __obj = _objectFactory.LoadGameObject(type);
        GameObject _obj = GameObject.Instantiate<GameObject>(__obj);
        _obj.transform.SetParent(Manager.Instance.Pool.transform);

        // 2-1. SpawnManager.objects에 추가
        if (_objects.TryGetValue(type, out var objList) == false) // 리스트가 한 번이라도 안 만들어졌다면 -> 최초 생성
        {
            objList = new List<BaseObject>();
            _objects.Add(type, objList);
        }
        BaseObject obj = _objectFactory.AddComponentToObject(type, _obj);

        objList.Add(obj);
        

        return obj;
    }

    public BaseObject SpawnObject(ObjectType type, Vector3Int cellPos)
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
                        item.CellPos = cellPos;
                        return item;
                    }
                }
            }
        }

        // 2. 새로 오브젝트를 만들어야 하는 경우
        GameObject __obj = _objectFactory.LoadGameObject(type);
        GameObject _obj = GameObject.Instantiate<GameObject>(__obj);
        _obj.transform.SetParent(Manager.Instance.Pool.transform);

        // 2-1. SpawnManager.objects에 추가
        if (_objects.TryGetValue(type, out var objList) == false) // 리스트가 한 번이라도 안 만들어졌다면 -> 최초 생성
        {
            objList = new List<BaseObject>();
            _objects.Add(type, objList);
        }
        BaseObject obj = _objectFactory.AddComponentToObject(type, _obj);

        objList.Add(obj);
        obj.CellPos = cellPos;

        return obj;
    }
}
