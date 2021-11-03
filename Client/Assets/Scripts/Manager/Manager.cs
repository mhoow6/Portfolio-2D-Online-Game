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
        Pool = new GameObject("Pool");
    }
    #endregion

    #region Content
    public static MapManager Map { get => Instance._map; }
    MapManager _map = new MapManager();
    public static SpawnManager Spawner { get => Instance._spawner; }
    SpawnManager _spawner = new SpawnManager();
    public GameObject Pool { get; private set; } 
    public static NetworkManager Network { get => Instance._network; }
    NetworkManager _network = new NetworkManager();
    #endregion

    private void Start()
    {
        // Network Init
        // _network.Init();

        DontDestroyOnLoad(Pool);
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        // _network.Update();
    }

    private void OnDestroy()
    {
        // _network.Dispose();
    }
}
