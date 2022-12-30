using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float SpawnRate {get; set;} = 5f;

    protected GameObject lastSpawnObject = null;
    protected long lastSpawnTime = 0;

    protected float spawnMin = 10f ;
    protected float spawnMax = 30f;
    protected bool checkDelete = true;

    protected virtual void Awake()
    {
        this.SpawnRate = UnityEngine.Random.Range(spawnMin, spawnMax);
    }

    protected virtual void Start()
    {
        InvokeRepeating(nameof(Spawn), SpawnRate, SpawnRate);
        if (checkDelete)
        {
            InvokeRepeating(nameof(CheckDeletingFood), SpawnRate, 1.0f);
        }            
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

    private void CheckDeletingFood()
    {
        if (lastSpawnObject != null)
        {
            var time = System.DateTime.Now.Ticks;
            System.TimeSpan elapsedSpan = new System.TimeSpan(time - lastSpawnTime);
            if (elapsedSpan.TotalSeconds > 10)
            {
                Destroy(lastSpawnObject);
                // Cooldown spawning and restart later
                this.CancelInvoke(nameof(Spawn));
                this.Awake();
                this.Start();
            }
        }
    }

    protected virtual bool SpawnCondition()
    {
        return true;
    }
}
