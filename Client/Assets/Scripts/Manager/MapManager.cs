using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
 

public class MapManager
{
    public Grid CurrentGrid { get; private set; }
    bool[,] _collision;
    BaseObject[,] _objects;

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

    public void LoadMap(MapId mapId, int roomId)
    {
        // 현재 켜져있는 UI들은 모두 방 설정에 관한 UI이니까 꺼준다.
        UIManager.Instance.ClosePopupAll();

        DestroyMap();
        Screen.SetResolution(640, 480, false);

        GameObject _go = MapFactory.GetMapObject(mapId);
        if (_go != null)
        {
            GameObject go = GameObject.Instantiate<GameObject>(_go);
            go.AddComponent<BaseScene>().Initalize(mapId, roomId);
            go.name = "Map";

            GameObject collision = Util.FindChild(go, "Tilemap_Collision", true);
            if (collision != null)
                collision.SetActive(false);

            CurrentGrid = go.GetComponent<Grid>();
            TextAsset txt = MapFactory.GetMapCollisionTextAsset(mapId);

            using (StringReader sr = new StringReader(txt.text))
            {
                MinX = int.Parse(sr.ReadLine());
                MaxX = int.Parse(sr.ReadLine());
                MinY = int.Parse(sr.ReadLine());
                MaxY = int.Parse(sr.ReadLine());

                int xCount = MaxX - MinX + 1;
                int yCount = MaxY - MinY + 1;
                _collision = new bool[yCount, xCount];
                _objects = new BaseObject[yCount, xCount];

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
    }

    public void DestroyMap()
    {
        if (CurrentGrid != null)
        {
            GameObject.Destroy(CurrentGrid.gameObject);
            CurrentGrid = null;
        }
            
    }

    public void UpdatePosition(Vector3Int current, Vector3Int next, BaseObject obj)
    {
        Vector2Int currentPos = CollisionCoordinate(current.x, current.y);
        _objects[currentPos.y, currentPos.x] = null;

        Vector2Int nextPos = CollisionCoordinate(next.x, next.y);
        _objects[nextPos.y, nextPos.x] = obj;
    }


    // 좌표계 변환 왜 이렇게 이해하기 힘들까?
    public bool CanGo(Vector3Int cellPos)
    {
        if (BoundCheck(cellPos) == false)
            return false;

        // 셀 좌표계 -> 맵을 이진수로 표현한 배열 좌표계
        Vector2Int vec = CollisionCoordinate(cellPos.x, cellPos.y);

        if (_collision[vec.y, vec.x] == true)
        {
            return false;
        }

        if (_objects[vec.y, vec.x] != null)
        {
            if (_objects[vec.y, vec.x].code != ObjectCode.Arrow)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsCreatureAt(Vector3Int cellPos)
    {
        if (BoundCheck(cellPos) == false)
            return false;

        Vector2Int vec = CollisionCoordinate(cellPos.x, cellPos.y);

        if ((_objects[vec.y, vec.x] != null))
        {
            return true;
        }

        return false;
    }

    public Creature CreatureAt(Vector3Int cellPos)
    {
        if (BoundCheck(cellPos) == false)
            return null;

        // 셀 좌표계 -> 맵을 이진수로 표현한 배열 좌표계
        Vector2Int vec = CollisionCoordinate(cellPos.x, cellPos.y);

        if ((_objects[vec.y, vec.x] != null))
        {
            if ((_objects[vec.y, vec.x].code != ObjectCode.Arrow))
            {
                return _objects[vec.y, vec.x] as Creature;
            }
        }

        return null;
    }

    bool BoundCheck(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        return true;
    }

    public Vector2Int CollisionCoordinate(int x, int y)
    {
        return new Vector2Int(x - MinX, MaxY - y);
    }
}
