using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    #region Mono-SingleTon
    public static Manager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Content
    public static MapManager Map { get => Instance._map; }
    MapManager _map = new MapManager();
    public static SpawnManager Spawner { get => Instance._spawner; }
    public GameObject Pool { get; private set; }
    SpawnManager _spawner = new SpawnManager();
    #endregion

    private void Start()
    {
        Pool = new GameObject("Pool");
        DontDestroyOnLoad(Pool);

        DontDestroyOnLoad(this.gameObject);
    }
}
