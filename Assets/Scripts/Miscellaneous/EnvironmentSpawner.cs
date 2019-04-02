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
    enum environmentType {traversable, nonTraversable, particle};

    // Environment object variables.
    public int environmentDensity = 50;
    public float traversableEnvironmentDensity = 0.10f;
    public float nonTraversableEnvironmentDensity = 0.0f;
    public float particleEnvironmentDensity = 0.0f;
    public GameObject[] traversableEnvironmentObject;
    public GameObject[] nonTraversableEnvironmentObject;
    public GameObject[] particleEnvironmentObject;
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
        float random = Random.Range(0, environmentDensity);
        var clone = new GameObject();
        clone.transform.position = cellPosition;
        if (random < environmentDensity * particleEnvironmentDensity) allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.particle), clone.transform) as GameObject);
        else if (random < environmentDensity * nonTraversableEnvironmentDensity) allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.nonTraversable), clone.transform) as GameObject);
        else if (random < environmentDensity * traversableEnvironmentDensity) allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(environmentType.traversable), clone.transform) as GameObject);
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
            case environmentType.traversable:
                index = Random.Range(0, traversableEnvironmentObject.Length);
                return traversableEnvironmentObject[index];
            case environmentType.nonTraversable:
                index = Random.Range(0, nonTraversableEnvironmentObject.Length);
                return nonTraversableEnvironmentObject[index];
            case environmentType.particle:
                index = Random.Range(0, particleEnvironmentObject.Length);
                return particleEnvironmentObject[index];
            default:
                Debug.Log("getRandomEnvironmentObject() error.");
                return null;
        }
    }

    
    #endregion

}
