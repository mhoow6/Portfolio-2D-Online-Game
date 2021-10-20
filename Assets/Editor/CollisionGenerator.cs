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
                throw new DirectoryNotFoundException("���丮�� ��� �ݶ��̴� ������ �����߽��ϴ�. ���丮�� ��������� �ٽ� �õ����ּ���.");
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

                    sw.WriteLine(xMin); // ���� �ּ� x ��ǥ ����
                    sw.WriteLine(xMax); // ���� �ִ� x ��ǥ ����
                    sw.WriteLine(yMin); // ���� �ּ� y ��ǥ ����
                    sw.WriteLine(yMax); // ���� �ּ� y ��ǥ ����

                    // �� 1 1 2 3 4 5 6 ..
                    // �� 0 1 2 3 4 5 6 .. 
                    for (int y = yMax; y >= yMin; y--)
                    {
                        for (int x = xMin; x <= xMax; x++)
                        {
                            TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                            if (tile != null)
                            {
                                sw.Write("1"); // ������
                            }
                            else
                            {
                                sw.Write("0"); // �� ������
                            }
                        }
                        sw.WriteLine(); // ����ĭ���� �Ѱ�
                    }
                }
            }
            Debug.Log($"Save Completed");
        }
    }
#endif
}


