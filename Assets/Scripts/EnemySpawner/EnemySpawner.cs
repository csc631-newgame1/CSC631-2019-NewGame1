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
    private MapManager mapManager;
    // A list of all accepted Spawn Zones
    private List<SpawnZone> spawnZones;
    private bool showEnemySpawnZones;
    private MapConfiguration mapConfiguration;

    [Header("Enemy Spawn Zone Settings")]
    [Tooltip("Increase slightly to increase distance between zones.")]
    public float distanceBetweenZonesScale = 0.1f;
    [Tooltip("Smallest size a zone can be.")]
    public float lowerRadius = 2f;
    [Tooltip("Largest size a zone can be.")]
    public float upperRadius = 4f;
    [Tooltip("Smallest number of spawnable tiles a zone can contain.")]
    public int minimumNumberOfTilesInSpawnZone = 1;
    [Tooltip("Largest number of spawnable tiles a zone can contain.")]
    public int maximumNumberOfTilesInSpawnZone = 50;

    public int maxNumberOfSpawnZones = 50;

    // Initializes map data
    public void Init(MapManager mapManager, MapConfiguration mapConfiguration)
    {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        this.width = config.width;
        this.height = config.height;
        regionSize = new Vector2(width, height);
        this.cell_size = config.cell_size;
        this.radius = cell_size * Mathf.Sqrt(2);
        this.offset = config.GetOffset();
        this.mapManager = mapManager;
        this.mapConfiguration = mapConfiguration;

        spawnZones = new List<SpawnZone>();
    }

    // Call this to spawn enemies on the Map Manager
    public void SpawnEnemies(GameObject enemyPrefab) {
		
        GenerateSpawnZones();
        TrimSpawnZones();
		
        // Create random TestEnemies
        List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
        // Create Enemy Groups
        for (int groupIndex = 0; groupIndex < maxNumberOfSpawnZones; groupIndex++) {
            List<EnemyGroupDescription> enemyGroupDescriptions = new List<EnemyGroupDescription>();

            // Create Enemies inside of the Enemy Group
            for (int enemyPropertyIndex = 0; enemyPropertyIndex < Random.Range(1, 3); enemyPropertyIndex++) {
                enemyGroupDescriptions.Add(new EnemyGroupDescription(new GameAgentStats(Random.Range(CharacterClasses.Orc, CharacterClasses.Skeleton), 7f, 35f, 1f, 6f),
                                                                 Random.Range(1, 2), 0.1f, 0.1f, 0f, 0f));
            }

            enemyGroups.Add(new EnemyGroup(enemyGroupDescriptions, Distribution.Balanaced));
        }

        EnemyGroupManager enemyGroupManager = new EnemyGroupManager(enemyGroups, spawnZones);
        List<EnemyToSpawn> enemies = enemyGroupManager.GetEnemiesToSpawn();

		foreach (EnemyToSpawn enemy in enemies) {
			GameObject enemySpawn = mapManager.instantiate(enemyPrefab, enemy.gridPosition, enemy.stats);
		}
    }

    // Creates a list of Spawn Zones of varrying sizes in the map
    private void GenerateSpawnZones(int numSamplesBeforeRejection = 50) {

        int[,] grid = new int[width, height];
        // A list of the remaining Spawn Zones to randomly generate new Spawn Zones
        List<SpawnZone> remainingSpawnZones = new List<SpawnZone>();

        remainingSpawnZones.Add(new SpawnZone(new Pos((int)(regionSize / 2).x, (int)(regionSize / 2).y), radius));

        // Attempts to create Spawn Zones of random size based on parameters
        // then checks if the Spawn Zone is acceptable
        while (remainingSpawnZones.Count > 0) {
            int spawnIndex = Random.Range(0, remainingSpawnZones.Count);
            SpawnZone spawnCenter = remainingSpawnZones[spawnIndex];
            bool candidateAccepted = false;

            for (int i=0; i<numSamplesBeforeRejection; i++) {
                float angle = Random.value * Mathf.PI * 2;
                float spawnZoneRadius = Random.Range(lowerRadius/cell_size, upperRadius/cell_size);
                Vector3 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                float distanceBetweenZones = Mathf.Max(1, i * distanceBetweenZonesScale);
                // Creates a potential position for the center of the Spawn Zone
                Vector3 candidateVector = new Vector3(spawnCenter.GetPosition().x, spawnCenter.GetPosition().y) + dir * spawnZoneRadius * distanceBetweenZones;
                Pos candidate = new Pos(Mathf.CeilToInt(candidateVector.x), Mathf.CeilToInt(candidateVector.y));

                if (IsValid(candidate, spawnZones, grid, spawnZoneRadius)) {
                    SpawnZone spawnZone = CreateSpawnZone(candidate, spawnZoneRadius);

                    // Checks if the number of zone tiles is acceptable
                    if (spawnZone.GetNumberOfUnpopulatedTilesInZone() >= minimumNumberOfTilesInSpawnZone
                        && spawnZone.GetNumberOfUnpopulatedTilesInZone() <= maximumNumberOfTilesInSpawnZone) {

                        // Spawn Zone is accepted and added to the list
                        spawnZones.Add(spawnZone);
                        remainingSpawnZones.Add(spawnZone);
                        grid[(int)(candidate.x / cell_size), (int)(candidate.y / cell_size)] = spawnZones.Count;
                        candidateAccepted = true;
                        break;
                    }
                }
            }

            if (!candidateAccepted) {
                remainingSpawnZones.RemoveAt(spawnIndex);
            }
        }
    }

    // Randomly removes spawn zones until it is within the max number of spawn zones
    private void TrimSpawnZones() {
        if (spawnZones.Count > maxNumberOfSpawnZones) {
            int numOfZonesToRemove = spawnZones.Count - maxNumberOfSpawnZones;
            for (int i = 0; i < numOfZonesToRemove; i++) {
                int randomIndex = Random.Range(0, spawnZones.Count - 1);
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
        SpawnZone spawnZone = new SpawnZone(candidate, spawnZoneRadius);
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

    public void ShowEnemySpawnZones(bool showEnemySpawnZones) {
        this.showEnemySpawnZones = showEnemySpawnZones;
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
