using MapUtils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
	
    private int width;
    private int height;
    private float cell_size;
    private Vector3 offset;
    private Vector3 regionSize;
    private float radius;
    private string seed;
    private int seedHashed;
    private MapManager mapManager;
    private System.Random rng;
    // A list of all accepted Spawn Zones
    private List<SpawnZone> spawnZones;
    public bool showEnemySpawnZones;
    private MapConfiguration mapConfiguration;
	
	public GameObject enemyPrefab;

    [Header("Enemy Spawn Zone Settings")]
    [Tooltip("Increase slightly to increase distance between zones.")]
    public float distanceBetweenZones = 1f;
    [Tooltip("Smallest size a zone can be.")]
    public float lowerRadius = 2f;
    [Tooltip("Largest size a zone can be.")]
    public float upperRadius = 6f;
    [Tooltip("Smallest number of spawnable tiles a zone can contain.")]
    public int minimumNumberOfTilesInSpawnZone = 3;
    [Tooltip("Largest number of spawnable tiles a zone can contain.")]
    public int maximumNumberOfTilesInSpawnZone = 100;

    public int maxNumberOfSpawnZones = 100;

    // Initializes map data
    public void Init(MapManager mapManager)
    {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        width = config.width;
        height = config.height;
        regionSize = new Vector2(width, height);
        cell_size = config.cell_size;
        radius = cell_size * Mathf.Sqrt(2);
        offset = config.GetOffset();
        this.mapManager = mapManager;
        mapConfiguration = config;
        seed = config.seed;
        rng = config.GetRNG();

        spawnZones = new List<SpawnZone>();
		SpawnEnemies();
    }

    // Call this to spawn enemies on the Map Manager
    public void SpawnEnemies() {
		
        GenerateSpawnZones();
        TrimSpawnZones();

        EnemyGroupManager enemyGroupManager = new EnemyGroupManager(spawnZones);
        List<EnemyToSpawn> enemies = enemyGroupManager.GetEnemiesToSpawn();

		foreach (EnemyToSpawn enemy in enemies) {
			mapManager.instantiate(enemyPrefab, enemy.gridPosition, enemy.stats);
		}
    }

    // Creates a list of Spawn Zones of varrying sizes in the map
    private void GenerateSpawnZones(int numSamplesBeforeRejection = 100) {

        int[,] grid = new int[width, height];
        // A list of the remaining Spawn Zones to randomly generate new Spawn Zones
        Pos zonePoint = new Pos(0, 0);
        while (!mapManager.IsWalkable(zonePoint)) {
            if (zonePoint.x < width) {
                zonePoint = new Pos(zonePoint.x + 1, zonePoint.y);
            } else {
                zonePoint.x = 0;
                zonePoint.y++;
            }
        }

        // Attempts to create Spawn Zones of random size based on parameters
        // then checks if the Spawn Zone is acceptable
        int attempts = 0;
        bool end = false;
        int distanceBetweenZones = Mathf.RoundToInt(this.distanceBetweenZones);
        int spawnZoneRadius = 0;
        int oldRadiusAndDistance = 0;

        while (attempts < 1000 && !end) {
            // retain old radius + distance
            oldRadiusAndDistance = distanceBetweenZones + spawnZoneRadius;
            // set new values
            distanceBetweenZones = rng.Next(Mathf.Max(1, Mathf.RoundToInt(this.distanceBetweenZones / 2f)), Mathf.RoundToInt(this.distanceBetweenZones));
            spawnZoneRadius = rng.Next(Mathf.RoundToInt(lowerRadius), Mathf.RoundToInt(upperRadius + 1));

            // Reached the end
            if (zonePoint.x > (width - upperRadius - this.distanceBetweenZones) && zonePoint.y > (height - upperRadius - this.distanceBetweenZones)) {
                end = true;
            }

            bool valid = true;
            Pos temp = new Pos(zonePoint.x, zonePoint.y);
            while (!IsValid(temp, spawnZones, grid, spawnZoneRadius)) {
                // Move the point to compensate for the smaller radius
                if (temp.x - 1 < 0) {
                    temp.x = 0;
                    if (temp.y - 1 < 0) {
                        temp.y = 0;
                    } else {
                        temp.y--;
                    }
                } else {
                    temp.x--;
                }

                // If smallest radius doesn't work, then it isn't a valid spawn point
                if (spawnZoneRadius == lowerRadius) {
                    valid = false;
                    break;
                } else {
                    // Try a smaller radius
                    spawnZoneRadius--;
                }
            }

            if (valid) {
                zonePoint = temp;
                SpawnZone spawnZone = CreateSpawnZone(zonePoint, spawnZoneRadius);
                // Checks if the number of zone tiles is acceptable
                if (spawnZone.GetNumberOfUnpopulatedTilesInZone() >= minimumNumberOfTilesInSpawnZone
                    && spawnZone.GetNumberOfUnpopulatedTilesInZone() <= maximumNumberOfTilesInSpawnZone) {

                    // Spawn Zone is accepted and added to the list
                    spawnZones.Add(spawnZone);
                    grid[zonePoint.x, zonePoint.y] = spawnZones.Count;
                }
            }

            // Move point to next position
            if (zonePoint.x + oldRadiusAndDistance + spawnZoneRadius >= width) {
                zonePoint.x = 0;
                if (zonePoint.y + spawnZoneRadius + oldRadiusAndDistance >= height) {
                    zonePoint.y = 0;
                } else {
                    zonePoint.y += spawnZoneRadius + oldRadiusAndDistance;
                }
            } else {
                zonePoint.x += spawnZoneRadius + oldRadiusAndDistance;
            }

            attempts++;
            if (attempts == 999) {
                Debug.Log("ended by attempts");
            }
        }
    }

    // Randomly removes spawn zones until it is within the max number of spawn zones
    private void TrimSpawnZones() {
        if (spawnZones.Count > maxNumberOfSpawnZones) {
            int numOfZonesToRemove = spawnZones.Count - maxNumberOfSpawnZones;
            for (int i = 0; i < numOfZonesToRemove; i++) {
                int randomIndex = rng.Next(0, spawnZones.Count - 1);
                spawnZones.Remove(spawnZones[randomIndex]);
            }
        }
    }

    // Checks if the center of the Spawn Zone (candidate) will create a valid Spawn Zone
    // Checks if the center is traversable, and if other Spawn Zones fall within this Spawn Zone
    bool IsValid(Pos candidate, List<SpawnZone> points, int[,] grid, float spawnZoneRadius) {
        // Check if center is traversable
        if (!mapManager.IsTraversable(new Pos((int)candidate.x, (int)candidate.y))) {
            return false;
        }

        // Check if the surrounding cells are within the radius of another Spawn Zone already created
        if (candidate.x >=0 && candidate.x < regionSize.x && candidate.y >= 0 && candidate.y < regionSize.y) {
            int cellX = candidate.x;
            int cellY = candidate.y;
            int numOfCellsToScan = Mathf.CeilToInt(upperRadius);

            // Determines number of cells to search around the center
            int searchStartX = Mathf.Max(0, cellX - numOfCellsToScan);
            int searchEndX = Mathf.Min(cellX + numOfCellsToScan, width - 1);
            int searchStartY = Mathf.Max(0, cellY - numOfCellsToScan);
            int searchEndY = Mathf.Min(cellY + numOfCellsToScan, height - 1);

            for (int x = searchStartX; x <= searchEndX; x++) {
                for (int y = searchStartY; y <= searchEndY; y++) {
                    int pointIndex = grid[x, y] - 1;
                    // non -1 means there is a Spawn Zone
                    if (pointIndex != -1) {
                        float dst = (new Vector3(candidate.x, candidate.y) - new Vector3(points[pointIndex].GetPosition().x, points[pointIndex].GetPosition().y)).magnitude;
                        if (dst <= (spawnZoneRadius + points[pointIndex].GetRadius())) {
                            // Candidate too close to another Spawn Zone
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }

    // Creates the Spawn Zone and populates its traversable zone tile list
    // by checking all the tiles within its radius
    SpawnZone CreateSpawnZone(Pos candidate, float spawnZoneRadius) {
        SpawnZone spawnZone = new SpawnZone(candidate, spawnZoneRadius, mapManager.GetTileRegion(candidate));
        List<Pos> zoneTiles = new List<Pos>();

        int cellX = candidate.x;
        int cellY = candidate.y;
        int numOfCellsToScan = Mathf.CeilToInt(spawnZoneRadius);

        int searchStartX = Mathf.Max(0, cellX - numOfCellsToScan);
        int searchEndX = Mathf.Min(cellX + numOfCellsToScan, width - 1);
        int searchStartY = Mathf.Max(0, cellY - numOfCellsToScan);
        int searchEndY = Mathf.Min(cellY + numOfCellsToScan, height - 1);

        for (int x = searchStartX; x <= searchEndX; x++) {
            for (int y = searchStartY; y <= searchEndY; y++) {
                Pos tile = new Pos(x, y);
                if (mapManager.IsTraversable(tile) && !mapManager.IsOccupied(tile)) {
                    int a = cellX - x;
                    int b = cellY - y;
                    if (Mathf.Sqrt(a*a + b*b) <= spawnZoneRadius) {
                        zoneTiles.Add(tile);
                    }
                }
            }
        }
        spawnZone.SetZoneTiles(zoneTiles);

        return spawnZone;
    }

    public List<SpawnZone> GetSpawnZones() {
        return spawnZones;
    }

    void OnDrawGizmos() {
        if (showEnemySpawnZones) {
            List<Color> gizColors = new List<Color> { Color.red, Color.yellow, Color.blue, Color.cyan, Color.green, Color.white, Color.grey };

            if (spawnZones.Count > 0) {
                for (int i = 0; i < spawnZones.Count; i++) {

                    if (spawnZones[i].IsPopulated()) {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(mapManager.grid_to_world(new Pos((int)spawnZones[i].GetPosition().x, (int)spawnZones[i].GetPosition().y)), spawnZones[i].GetRadius());
                        List<Pos> zoneTiles = spawnZones[i].GetUnpopulatedZoneTiles();
                        foreach (Pos tile in zoneTiles) {
                            Gizmos.color = gizColors[i % gizColors.Count];
                            Gizmos.DrawWireCube(mapManager.grid_to_world(new Pos((int)tile.x, (int)tile.y)), new Vector3(mapConfiguration.cell_size, 0, mapConfiguration.cell_size));
                        }
                    }
                }
            }
        }
    }
}
