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
        Debug.Log("Update for SimulationObjects");
        allSimuObjects.ForEach(DestroyImmediate);
        allSimuObjects.Clear();

        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Player"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Body"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Scoring"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Speed"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("Wall"));
        CreateSpawnedObjectCopies(GameObject.FindGameObjectsWithTag("BrickSphere"));
    }

    private void CreateSpawnedObjectCopies(GameObject[] spawnedObjects)
    {
        foreach (var spawnedObject in spawnedObjects)
        {   
            if (spawnedObject.CompareTag("Player") && spawnedObject.GetComponent<Snake>().IsSimulation)
            {
                continue;
            }
            var simuSpawnedGoodie = Instantiate(spawnedObject);
            if (spawnedObject.CompareTag("Player"))
            {
                simuSpawnedGoodie.GetComponent<Snake>().IsSimulation = true;
            }
            simuSpawnedGoodie.layer = this.SimLayerID;
            allSimuObjects.Add(simuSpawnedGoodie);
        }
    }        
}
