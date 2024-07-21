using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadSpawningPool : SpawningPool
{
    public override void Init()
    {
        GameManager.Instance.Spawn.OnSpawnEvent -= AddBreadCounter;
        GameManager.Instance.Spawn.OnSpawnEvent += AddBreadCounter;

    }

    public void AddBreadCounter(int value) { _objCount += value; } 
    

}
