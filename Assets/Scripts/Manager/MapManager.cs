using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum MapId
{
    TOWN = 1,
    NONE = -1,
}

public class MapManager
{
    public Grid CurrentGrid { get; private set; }
    bool[,] _collision;
    MapFactory _factory = new MapFactory();

    public int XLength
    {
        get
        {
            return Mathf.Abs(MinX) + Mathf.Abs(MaxX);
        }
    }

    public int YLength
    {
        get
        {
            return Mathf.Abs(MinY) + Mathf.Abs(MaxY);
        }
    }


    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }

    public void LoadMap(MapId mapId)
    {
        DestroyMap();

        GameObject _go = _factory.GetMapObject(mapId);
        GameObject go = GameObject.Instantiate<GameObject>(_go);
        go.name = "Map";

        GameObject collision = Util.FindChild(go, "Tilemap_Collision", true);
        if (collision != null)
            collision.SetActive(false);

        CurrentGrid = go.GetComponent<Grid>();
        TextAsset txt = _factory.GetMapCollisionTextAsset(mapId);

        using (StringReader sr = new StringReader(txt.text))
        {
            MinX = int.Parse(sr.ReadLine());
            MaxX = int.Parse(sr.ReadLine());
            MinY = int.Parse(sr.ReadLine());
            MaxY = int.Parse(sr.ReadLine());

            int xCount = MaxX - MinX + 1;
            int yCount = MaxY - MinY + 1;
            _collision = new bool[yCount, xCount];

            // collision: 왼쪽 아래에서 오른쪽 위로 순회
            for (int y = 0; y < yCount; y++)
            {
                string line = sr.ReadLine();
                for (int x = 0; x < xCount; x++)
                {
                    _collision[y, x] = (line[x] == '1' ? true : false);
                }
            }
        }
    }

    public void DestroyMap()
    {
        if (CurrentGrid != null)
        {
            GameObject.Destroy(CurrentGrid.gameObject);
            CurrentGrid = null;
        }
            
    }


    // 좌표계 변환 왜 이렇게 이해하기 힘들까?
    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        // 셀 좌표계 -> 맵을 이진수로 표현한 배열 좌표계
        int collisionX = cellPos.x - MinX;
        int collisionY = MaxY - cellPos.y;

        if (_collision[collisionY, collisionX] == true)
        {
            return false;
        }

        return true;
    }
}

public class MapFactory
{
    public GameObject GetMapObject(MapId mapId)
    {
        GameObject map = null;

        switch (mapId)
        {
            case MapId.TOWN:
                map = Resources.Load<GameObject>(Paths.Map_Prefabs + "/Map_001");
                break;
        }

        return map;
    }

    public TextAsset GetMapCollisionTextAsset(MapId mapId)
    {
        TextAsset map = null;

        switch (mapId)
        {
            case MapId.TOWN:
                map = Resources.Load<TextAsset>(Paths.Map_Collision + "/Map_001");
                break;
        }

        return map;
    }
}
