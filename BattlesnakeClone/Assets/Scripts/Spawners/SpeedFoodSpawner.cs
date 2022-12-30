using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFoodSpawner : AbstractSpawner
{
    protected override void Awake()
    {
        this.checkDelete = false;
        this.spawnMin = 30f;
        this.spawnMax = 60f;
        base.Awake();
    }
    protected override bool SpawnCondition()
    {
        return lastSpawnObject == null;
    }
}
