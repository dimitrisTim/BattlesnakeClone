using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : AbstractSpawner
{
    protected override void Awake()
    {
        this.checkDelete = false;
        this.spawnMin = 3f;
        this.spawnMax = 10f;
        base.Awake();
    }
}
