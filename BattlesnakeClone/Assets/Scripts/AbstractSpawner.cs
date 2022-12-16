using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float SpawnRate {get; set;} = 5f;

    protected GameObject lastSpawnObject = null;
    protected long lastSpawnTime = 0;

    protected virtual void Start()
    {
        InvokeRepeating(nameof(Spawn), SpawnRate, SpawnRate);
    }

    protected void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    protected virtual void Spawn()
    {   
        var width = GameAssets.i.Width;
        var height = GameAssets.i.Height;
        while(SpawnCondition())
        {
            var randPos = new Vector2Int(Random.Range(-width, width), Random.Range(-height, height));
            var randPos3 = new Vector3(randPos.x, randPos.y);
            // If no other object in this area
            if (Physics2D.OverlapCircle(randPos, 0.1f) == null)
            {
                lastSpawnObject = Instantiate(prefab, randPos3, Quaternion.identity);
                lastSpawnTime = System.DateTime.Now.Ticks;
                break;
            } 
        }        
    }

    protected virtual bool SpawnCondition()
    {
        return true;
    }
}
