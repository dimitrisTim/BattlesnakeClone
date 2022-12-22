using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : AbstractSpawner
{
    protected override void Awake()
    {
        this.spawnMin = 1f;
        this.spawnMax = 1f;
        base.Awake();
    }

    protected override void Spawn()
    {
        base.Spawn();
        if (lastSpawnObject != null)
        {
            var pos = lastSpawnObject.transform.position;
            pos.z = -0.5f;
            lastSpawnObject.transform.position = pos;
        }
    }
    
    protected override bool SpawnCondition()
    {
        return lastSpawnObject == null;
    }
}
