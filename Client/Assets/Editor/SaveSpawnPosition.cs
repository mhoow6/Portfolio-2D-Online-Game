using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;
using static Define;
using Google.Protobuf.Protocol;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class SaveSpawnPosition
{
#if UNITY_EDITOR
    // % (Ctrl), # (Shift), & (Alt)
    [MenuItem("Tools/Save Spawn Position")]
    public static void Save()
    {
        if (EditorUtility.DisplayDialog("2D Map Spawn Position Saver", "Save Position?", "Yes", "No"))
        {
            if (!Directory.Exists($"{ResourcePaths.Map_Collision_Save}"))
            {
                Directory.CreateDirectory($"{ResourcePaths.Map_Collision_Save}");
                throw new DirectoryNotFoundException("디렉토리가 없어서 파일 생성에 실패했습니다. 디렉토리를 만들었으니 다시 시도해주세요.");
            }

            GameObject[] maps = Resources.LoadAll<GameObject>(ResourcePaths.Map_Prefabs);

            for (int i = 0; i < maps.Length; i++)
            {
                // 플레이어 스폰 위치 저장
                Tilemap ptm = Util.FindChild<Tilemap>(maps[i], "Tilemap_PlayerSpawn", true);
                // 몬스터 스폰 위치 저장
                Tilemap mtm = Util.FindChild<Tilemap>(maps[i], "Tilemap_MonsterSpawn", true);
                if (ptm != null && mtm != null)
                {
                    using (StreamWriter sw = File.CreateText($"{ResourcePaths.Map_SpawnPos_Save}/{maps[i].name}.txt"))
                    {
                        sw.WriteLine("objectcode,x,y");

                        {
                            int xMin = ptm.cellBounds.xMin;
                            int xMax = ptm.cellBounds.xMax;
                            int yMin = ptm.cellBounds.yMin;
                            int yMax = ptm.cellBounds.yMax;

                            for (int x = xMin; x < xMax; x++)
                            {
                                for (int y = yMin; y < yMax; y++)
                                {
                                    TileBase tile = ptm.GetTile(new Vector3Int(x, y, 0));
                                    if (tile != null)
                                    {
                                        sw.WriteLine($"{ObjectCode.Player},{x},{y}");
                                    }
                                }
                            }
                        }

                        {
                            int xMin = mtm.cellBounds.xMin;
                            int xMax = mtm.cellBounds.xMax;
                            int yMin = mtm.cellBounds.yMin;
                            int yMax = mtm.cellBounds.yMax;

                            for (int x = xMin; x < xMax; x++)
                            {
                                for (int y = yMin; y < yMax; y++)
                                {
                                    TileBase tile = mtm.GetTile(new Vector3Int(x, y, 0));
                                    if (tile != null)
                                    {
                                        sw.WriteLine($"{ObjectCode.Monster},{x},{y}");
                                    }
                                }
                            }
                        }

                    }
                    Debug.Log($"{maps[i].name} Save Completed");
                }
            }
        }
    }
#endif
}

