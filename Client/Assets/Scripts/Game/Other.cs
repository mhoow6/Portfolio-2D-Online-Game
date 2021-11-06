using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Other : Player
{
    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        V_UpdateObject();
    }


}
