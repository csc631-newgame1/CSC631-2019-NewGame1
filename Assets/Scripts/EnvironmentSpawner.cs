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

    // Environment object variables.
    public int environmentDensity = 50;
    public float traversableEnvironmentDensity = 0.10f;
    public float nonTraversableEnvironmentDensity = 0.0f;
    public GameObject[] traversableEnvironmentObject;
    public GameObject[] nonTraversableEnvironmentObject;
    #endregion

    public void Init(MapManager mapManager, MapConfiguration mapConfiguration)
    {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        this.width = config.width;
        this.height = config.height;
        this.cell_size = config.cell_size;
        this.mapManager = mapManager;
        this.mapConfiguration = mapConfiguration;
    }

    // spawnEnvironment(), spawnEnvironmentObject(Vector3 cellPosition), GameObject getRandomEnvironmentObject()
    #region Main methods
    public void spawnEnvironment()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Pos position = mapManager.world_to_grid(new Vector3(x, 0, y));
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
        if (random < environmentDensity * nonTraversableEnvironmentDensity) Instantiate(getRandomEnvironmentObject(false), clone.transform);
        else if (random < environmentDensity * traversableEnvironmentDensity) Instantiate(getRandomEnvironmentObject(true), clone.transform);
    }

    GameObject getRandomEnvironmentObject(bool traversable)
    {
        int index;
        if (traversable)
        {
            index = Random.Range(0, traversableEnvironmentObject.Length);
            return traversableEnvironmentObject[index];
        } else
        {
            index = Random.Range(0, nonTraversableEnvironmentObject.Length);
            return nonTraversableEnvironmentObject[index];
        }
    }
    #endregion

}
