using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFoodSpawner : AbstractSpawner
{
    private void Awake()
    {
        this.SpawnRate = UnityEngine.Random.Range(10f, 30f);
    }

    protected override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(CheckDeletingFood), SpawnRate, 1.0f);
    }

    private void CheckDeletingFood()
    {
        if (lastSpawnObject != null)
        {
            var time = System.DateTime.Now.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(time - lastSpawnTime);
            if (elapsedSpan.TotalSeconds > 10)
            {
                Destroy(lastSpawnObject);
                // Cooldown spawning and restart later
                this.CancelInvoke(nameof(Spawn));
                this.Awake();
                base.Start();
            }
        }
    }
    
    protected override bool SpawnCondition()
    {
        return lastSpawnObject == null;
    }
}
