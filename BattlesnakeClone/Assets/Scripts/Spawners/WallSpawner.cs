using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : AbstractSpawner
{
    protected override void Awake()
    {
        this.spawnMin = 30f;
        this.spawnMax = 60f;
        base.Awake();
    }
    
    protected override bool SpawnCondition()
    {
        return lastSpawnObject == null;
    }
}
