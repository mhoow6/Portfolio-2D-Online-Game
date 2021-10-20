using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CollisionGenerator
{
#if UNITY_EDITOR
    // % (Ctrl), # (Shift), & (Alt)
    [MenuItem("Tools/Collision Generate %#g")]
    public static void Generate()
    {
        if (EditorUtility.DisplayDialog("2D Map Collision Generator", "Create Collision?", "Create", "Cancel"))
        {
            if (!Directory.Exists($"{Paths.Map_Collision}"))
            {
                Directory.CreateDirectory($"{Paths.Map_Collision}");
                throw new DirectoryNotFoundException("디렉토리가 없어서 콜라이더 생성에 실패했습니다. 디렉토리를 만들었으니 다시 시도해주세요.");
            }

            GameObject[] gameObjects = Resources.LoadAll<GameObject>(Paths.Map_Prefabs);

            foreach (GameObject go in gameObjects)
            {
                Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

                using (StreamWriter sw = File.CreateText($"{Paths.Map_Collision}/{go.name}.txt"))
                {
                    int xMin = tm.cellBounds.xMin;
                    int xMax = tm.cellBounds.xMax;
                    int yMin = tm.cellBounds.yMin;
                    int yMax = tm.cellBounds.yMax;

                    sw.WriteLine(xMin); // 맵의 최소 x 좌표 쓰기
                    sw.WriteLine(xMax); // 맵의 최대 x 좌표 쓰기
                    sw.WriteLine(yMin); // 맵의 최소 y 좌표 쓰기
                    sw.WriteLine(yMax); // 맵의 최소 y 좌표 쓰기

                    // ⓑ 1 1 2 3 4 5 6 ..
                    // ⓐ 0 1 2 3 4 5 6 .. 
                    for (int y = yMax; y >= yMin; y--)
                    {
                        for (int x = xMin; x <= xMax; x++)
                        {
                            TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                            if (tile != null)
                            {
                                sw.Write("1"); // 막혔다
                            }
                            else
                            {
                                sw.Write("0"); // 안 막혔다
                            }
                        }
                        sw.WriteLine(); // 다음칸으로 넘겨
                    }
                }
            }
            Debug.Log($"Save Completed");
        }
    }
#endif
}


