using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationObjects : ScriptableObject
{
    public List<GameObject> allSimuObjects = new List<GameObject>();

    public int SimLayerID {get; set;}

    public void Init(int layerID)
    {
        this.SimLayerID = layerID;
    }

    public void SetSimuObjects()
    {
        allSimuObjects.ForEach(DestroyImmediate);
        allSimuObjects.Clear();

        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Player"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Body"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Scoring"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Speed"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Wall"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("BrickSphere"));
    }

    public List<Snake> GetSnakes()
    {
        return allSimuObjects.Where(x => x.CompareTag("Player")).Select(x => x.GetComponent<Snake>()).ToList();
    }

    private void CreateSpawnedObjectCopies(GameObject[] spawnedObjects)
    {
        foreach (var spawnedObject in spawnedObjects)
        {   
            if (spawnedObject.CompareTag("Player") && spawnedObject.GetComponent<Snake>().IsSimulation)
            {
                continue;
            }
            var simuSpawnedObj = Instantiate(spawnedObject);
            if (spawnedObject.CompareTag("Player"))
            {
                simuSpawnedObj.GetComponent<Snake>().IsSimulation = true;
                simuSpawnedObj.GetComponent<BoxCollider2D>().enabled = false; 
            }
            simuSpawnedObj.layer = this.SimLayerID;
            allSimuObjects.Add(simuSpawnedObj);
        }
    }        
}
