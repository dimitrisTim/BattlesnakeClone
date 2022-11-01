using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 1f;
    public int width;
    public int height;

    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    private void Spawn()
    {   
        Vector2Int gridPosition = new Vector2Int(Random.Range(-width, width), Random.Range(-height, height));  
        GameObject food = Instantiate(prefab, new Vector3(gridPosition.x, gridPosition.y), Quaternion.identity);   
    }
}
