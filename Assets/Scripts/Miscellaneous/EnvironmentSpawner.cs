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
    enum environmentType {traversableFolliage, nonTraversableFolliage, traversableObject, nonTraversableObject, traversableStructure, nonTraversableStructure, particle, portal};

    // Environment object variables.
    public int environmentDensity = 50;
    public float traversableFolliagDensity = 0.10f;
    public float nonTraversableFolliageDensity = 0.10f;
    public float traversableObjectDensity = 0.10f;
    public float nonTraversableObjectDensity = 0.0f;
    public float traversableStructureDensity = 0.0f;
    public float nonTraversableStructureDensity = 0.0f;
    public float particleDensity = 0.0f;
    public float portalMargin = 0.30f;
    public GameObject portal;
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
        spawnPortals();
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
                    spawnEnvironmentObject(position);
                }
            }
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

    void spawnPortals()
    {
        int randomX, randomY;
        Pos position;
        do
        {
            randomX = Random.Range(0, (int)(width * portalMargin));
            randomY = Random.Range(0, (int)(height * portalMargin));
            position = new Pos(randomX, randomY);
        } while (!mapManager.IsTraversable(position));
        spawNonTraversableObject(portal, position);
        do
        {
            randomX = Random.Range((int)(width - width * portalMargin), width);
            randomY = Random.Range((int)(width - width * portalMargin), height);
            position = new Pos(randomX, randomY);
        } while (!mapManager.IsTraversable(position));
        spawNonTraversableObject(portal, position);
    }

    void spawnEnvironmentObject(Pos position)
    {
        float random;

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * traversableFolliagDensity)
        {
            spawnRandomTraversableObject(environmentType.traversableFolliage, position);
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * particleDensity)
        {
            spawnRandomTraversableObject(environmentType.particle, position);
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * traversableObjectDensity)
        {
            spawnRandomTraversableObject(environmentType.traversableObject, position);
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * traversableStructureDensity)
        {
            spawnRandomTraversableObject(environmentType.traversableStructure, position);
            return;
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * nonTraversableFolliageDensity)
        {
            spawnRandomNonTraversableObject(environmentType.nonTraversableFolliage, position);
            return;
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * nonTraversableObjectDensity)
        {
            spawnRandomNonTraversableObject(environmentType.nonTraversableObject, position);
            return;
        }

        random = Random.Range(0, environmentDensity);
        if (random < environmentDensity * nonTraversableStructureDensity)
        {
            spawnRandomNonTraversableObject(environmentType.nonTraversableStructure, position);
            return;
        }
    }

    void spawnRandomTraversableObject (environmentType type, Pos position)
    {
        allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(type), mapManager.grid_to_world(position), Quaternion.identity) as GameObject);
    }

    void spawnRandomNonTraversableObject(environmentType type, Pos position)
    {
        allEnvironmentObject.Add(Instantiate(getRandomEnvironmentObject(type), mapManager.grid_to_world(position), Quaternion.identity) as GameObject);
        mapManager.removeTraversableTile(position);
        return;
    }

    void spawNonTraversableObject(GameObject type, Pos position)
    {
        allEnvironmentObject.Add(Instantiate(type, mapManager.grid_to_world(position), Quaternion.identity) as GameObject);
        mapManager.removeTraversableTile(position);
        return;
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
