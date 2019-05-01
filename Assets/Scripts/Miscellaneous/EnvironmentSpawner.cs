using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapUtils;
using RegionUtils;

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
    enum environmentType { traversableFolliage, nonTraversableFolliage, traversableObject, nonTraversableObject, traversableStructure, nonTraversableStructure, particle, portal };

    // Environment object variables.
    public int environmentDensity = 50;
    public float traversableFolliagDensity = 0.10f;
    public float nonTraversableFolliageDensity = 0.10f;
    public float traversableObjectDensity = 0.10f;
    public float nonTraversableObjectDensity = 0.0f;
    public float traversableStructureDensity = 0.0f;
    public float nonTraversableStructureDensity = 0.0f;
    public float particleDensity = 0.0f;

    [Header("Portal Settings")]
    public GameObject startPortal;
    public GameObject exitPortal;
    public float portalMargin = 0.50f;
    public int spawnAreaRadius = 3;
    public int reserveAreaRadius = 3;

    [Header("Portrait Settings")]
    List<Pos> paintedList = new List<Pos>();
    public int maxRadius = 5;
    public int minRadius = 1;
    public int minArea = 10;
    public int portraitMargin = 5;
    public float structureDensity;
    public float objectDensity;
    public float foliageDensity;

    [Header("Portrait Prefabs")]
    public GameObject[] folliageStructure;
    public GameObject[] folliageObject;
    public GameObject[] folliageRubble;
    public GameObject[] orcStructure;
    public GameObject[] orcObject;
    public GameObject[] orcRubble;
    public GameObject[] undeadStructure;
    public GameObject[] undeadObject;
    public GameObject[] undeadRubble;

    [Header("Environment Prefabs")]
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
                if (isValidPotraitSpawn(position))
                {
                    spawnPortrait(position);
                }
            }
        }
    }

    // Portrait
    void spawnPortrait(Pos position)
    {
        int radius = Random.Range(minRadius, maxRadius);

        Pos startPos = new Pos(position.x - radius, position.y - radius);
        Pos endPos = new Pos(position.x + radius, position.y + radius);
        List<Pos> validPosList = getListOfValidPositions(startPos, endPos);

        int targetStructureQuota = (int)(validPosList.Count * structureDensity);
        int targetObjectQuota = (int)(validPosList.Count * objectDensity);

        Pos spawnPos;
        int i, randomIndex;

        if (validPosList.Count < minArea) return;

        for (i = targetStructureQuota; i > 0; i--)
        {
            if (validPosList.Count == 0) break;
            randomIndex = Random.Range(1, validPosList.Count);
            spawnPos = validPosList[randomIndex];
            spawnStructure(spawnPos);
            validPosList.RemoveAt(randomIndex);
        }

        for (i = targetObjectQuota; i > 0; i--)
        {
            if (validPosList.Count == 0) break;
            randomIndex = Random.Range(1, validPosList.Count);
            spawnPos = validPosList[randomIndex];
            spawnObject(spawnPos);
            validPosList.RemoveAt(randomIndex);
        }

        updatePaintedList(position, radius + portraitMargin);
        Debug.Log(targetObjectQuota + ", " + targetStructureQuota + ", " + radius + ", " + validPosList.Count);
    }

    List<Pos> getListOfValidPositions (Pos startPos, Pos endPos) {
        List<Pos> tempPosList = new List<Pos>();
        Pos tempPos;
        int x, y;

        for (x = startPos.x; x < endPos.x; x++)
        {
            for (y = startPos.y; y < endPos.y; y++)
            {
                tempPos = new Pos(x, y);
                if (isValidPotraitSpawn(tempPos)) {
                    tempPosList.Add(tempPos);
                }
            }
        }

        return tempPosList;
    }

    bool isValidPotraitSpawn(Pos position)
    {
        return mapManager.IsWalkable(position) && !mapManager.IsReserved(position) && !paintedList.Contains(position);
    }

    void spawnStructure(Pos position)
    {
        int randomIndex = Random.Range(0, traversableFolliageObject.Length);
        GameObject randomStructure = structures[randomIndex];
        allEnvironmentObject.Add(mapManager.instantiate_environment(randomStructure, position, true));
    }

    void spawnObject(Pos position)
    {
        int randomIndex = Random.Range(0, traversableFolliageObject.Length);
        GameObject randomObject = objects[randomIndex];
        allEnvironmentObject.Add(mapManager.instantiate_environment(randomObject, position, true));
    }

    void updatePaintedList(Pos position, int margin)
    {
        List<Pos> tempPosList = new List<Pos>();
        Pos startPos = new Pos(position.x - margin, position.y - margin);
        Pos endPos = new Pos(position.x + margin, position.y + margin);
        Pos tempPos;
        int x, y;

        for (x = startPos.x; x < endPos.x; x++)
        {
            for (y = startPos.y; y < endPos.y; y++)
            {
                tempPos = new Pos(x, y);
                paintedList.Add(tempPos);
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
        Region[] furthestRegions = mapManager.findFurthestRegions(1);

        Region startRegion = furthestRegions[0];
        Pos startRegionPosition = startRegion.startpos;
        int startRegionMargin = (int)(System.Math.Sqrt(startRegion.count) / 2);

        Region endRegion = furthestRegions[1];
        Pos endRegionPosition = endRegion.startpos;
        int endRegionMargin = (int)(System.Math.Sqrt(endRegion.count) / 2);

        int randomX, randomY;
        Pos position;

        do
        {
            randomX = Random.Range(startRegionPosition.x - startRegionMargin, startRegionPosition.x + startRegionMargin);
            randomY = Random.Range(startRegionPosition.y - startRegionMargin, startRegionPosition.y + startRegionMargin);
            position = new Pos(randomX, randomY);
        } while (!mapManager.IsTraversable(position));
        spawnTraversableObject(startPortal, position);
        setSpawnAreaAroundPosition(position, spawnAreaRadius);
        reserveAreaAroundPoistion(position, reserveAreaRadius);

        do
        {
            randomX = Random.Range(endRegionPosition.x - endRegionMargin, endRegionPosition.x + endRegionMargin);
            randomY = Random.Range(endRegionPosition.y - endRegionMargin, endRegionPosition.y + endRegionMargin);
            position = new Pos(randomX, randomY);
        } while (!mapManager.IsTraversable(position));
        spawnTraversableObject(exitPortal, position);
    }

    void reserveAreaAroundPoistion(Pos position, int radius)
    {
        int diameter = radius * 2;
        for (int x = position.x - radius; x < position.x + diameter; x++)
        {
            for (int y = position.y - radius; y < position.y + diameter; y++)
            {
                Pos currentPos = new Pos(x, y);
                if (mapManager.IsTraversable(currentPos))
                {
                    mapManager.setTileReserved(currentPos);
                }
            }
        }
    }

    void setSpawnAreaAroundPosition(Pos position, int radius)
    {
        int diameter = radius * 2;
        for (int x = position.x - radius; x < position.x + diameter; x++)
        {
            for (int y = position.y - radius; y < position.y + diameter; y++)
            {
                Pos currentPos = new Pos(x, y);
                if (mapManager.IsTraversable(currentPos))
                {
                    mapManager.setTileSpawn(currentPos);
                }
            }
        }
    }

    void spawnRandomEnvironmentObject(Pos position)
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

    void spawnRandomTraversableObject(environmentType type, Pos position)
    {
        allEnvironmentObject.Add(mapManager.instantiate_environment(getRandomEnvironmentObject(type), position, true));
    }

    void spawnRandomNonTraversableObject(environmentType type, Pos position)
    {
        allEnvironmentObject.Add(mapManager.instantiate_environment(getRandomEnvironmentObject(type), position, false));
        return;
    }

    void spawnTraversableObject(GameObject type, Pos position)
    {
        allEnvironmentObject.Add(mapManager.instantiate_environment(type, position, true));
        return;
    }

    void spawnNonTraversableObject(GameObject type, Pos position)
    {
        allEnvironmentObject.Add(mapManager.instantiate_environment(type, position, false));
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