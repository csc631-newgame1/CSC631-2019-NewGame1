using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapUtils;

public class EnvironmentSpawner : MonoBehaviour
{

    #region Variables
    // Referenced conponents.
    MapManager mapManager;
    MapConfiguration mapConfiguration;

    // Map variables.
    float cell_size;
    int width;
    int height;
    enum environmentType {traversableFolliage, nonTraversableFolliage, traversableObject, nonTraversableObject, traversableStructure, nonTraversableStructure, particle };

    // Environment object variables.
    public int environmentDensity = 50;
    public float traversableFolliagDensity = 0.10f;
    public float nonTraversableFolliageDensity = 0.10f;
    public float traversableObjectDensity = 0.10f;
    public float nonTraversableObjectDensity = 0.0f;
    public float traversableStructureDensity = 0.0f;
    public float nonTraversableStructureDensity = 0.0f;
    public float particleDensity = 0.0f;
    public GameObject[] traversableFolliageObject;
    public GameObject[] nonTraversableFolliageObject;
    public GameObject[] traversableObjectObject;
    public GameObject[] nonTraversableObjectObject;
    public GameObject[] traversableStructureObject;
    public GameObject[] nonTraversableStructureObject;
    public GameObject[] particleObject;
    List<GameObject> allEnvironmentObject = new List<GameObject>();
    #endregion

    public void Init(MapManager mapManager)
    {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        this.width = config.width;
        this.height = config.height;
        this.cell_size = config.cell_size;
        this.mapManager = mapManager;
        this.mapConfiguration = mapConfiguration;
		spawnEnvironment();
    }

    // spawnEnvironment(), spawnEnvironmentObject(Vector3 cellPosition), GameObject getRandomEnvironmentObject()
    #region Main methods
    public void spawnEnvironment()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Pos position = new Pos(x, y);
                if (mapManager.IsTraversable(position)) {
                    spawnEnvironmentObject(mapManager.grid_to_world(position));
                }
            }
        }
    }

    void spawnEnvironmentObject(Vector3 cellPosition)
    {
        float random;
        var clone = new GameObject();
        clone.transform.position = cellPosition;

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * traversableFolliagDensity)
        {
            allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.traversableFolliage), clone.transform) as GameObject);
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * traversableObjectDensity)
        {
            allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.traversableObject), clone.transform) as GameObject);
            return;
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * traversableStructureDensity)
        {
            allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.traversableStructure), clone.transform) as GameObject);
            return;
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * nonTraversableFolliageDensity)
        {
            allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.nonTraversableFolliage), clone.transform) as GameObject);
            return;
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * nonTraversableObjectDensity)
        {
            allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.nonTraversableObject), clone.transform) as GameObject);
            return;
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * nonTraversableStructureDensity)
        {
            allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.nonTraversableStructure), clone.transform) as GameObject);
            return;
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * particleDensity)
        {
            allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.particle), clone.transform) as GameObject);
            return;
        }
    }

    public void clearEnvironment()
    {
        for (int i = 0; i < allEnvironmentObject.Count; i++)
        {
            Destroy(allEnvironmentObject[i]);
        }
        allEnvironmentObject.Clear();
    }

    GameObject getRandomEnvironmentObject(environmentType type)
    {
        int index;
        switch (type)
        {
            case environmentType.traversableFolliage:
                index = Random.Range(0, traversableFolliageObject.Length);
                return traversableFolliageObject[index];
            case environmentType.nonTraversableFolliage:
                index = Random.Range(0, nonTraversableFolliageObject.Length);
                return nonTraversableFolliageObject[index];
            case environmentType.traversableObject:
                index = Random.Range(0, traversableObjectObject.Length);
                return traversableObjectObject[index];
            case environmentType.nonTraversableObject:
                index = Random.Range(0, nonTraversableObjectObject.Length);
                return nonTraversableObjectObject[index];
            case environmentType.traversableStructure:
                index = Random.Range(0, traversableStructureObject.Length);
                return traversableStructureObject[index];
            case environmentType.nonTraversableStructure:
                index = Random.Range(0, nonTraversableStructureObject.Length);
                return nonTraversableStructureObject[index];
            case environmentType.particle:
                index = Random.Range(0, particleObject.Length);
                return particleObject[index];
            default:
                Debug.Log("getRandomEnvironmentObject() error.");
                return null;
        }
    }

    
    #endregion

}
