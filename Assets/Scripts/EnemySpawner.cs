using MapUtils;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    private int width;
    private int height;
    private float cell_size;
    private Vector3 offset;
    private Vector3 regionSize;
    private float radius;
    private MapManager mapManager;

    public float distanceBetweenZonesScale = 0.1f;
    public float lowerRadius = 2f;
    public float upperRadius = 4f;
    public int minimumNumberOfTilesInSpawnZone = 1;
    public int maximumNumberOfTilesInSpawnZone = 50;

    public void Init(MapManager mapManager)
    {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        this.width = config.width;
        this.height = config.height;
        regionSize = new Vector2(width, height);
        this.cell_size = config.cell_size;
        this.radius = cell_size * Mathf.Sqrt(2);
        this.offset = config.GetOffset();
        this.mapManager = mapManager;
    }

    public List<SpawnZone> GeneratePoints(int numSamplesBeforeRejection = 50) {
        int[,] grid = new int[width, height];
        List<SpawnZone> points = new List<SpawnZone>();
        List<SpawnZone> spawnPoints = new List<SpawnZone>();

        spawnPoints.Add(new SpawnZone(regionSize / 2, radius));
        while (spawnPoints.Count > 0) {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            SpawnZone spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i=0; i<numSamplesBeforeRejection; i++) {
                float angle = Random.value * Mathf.PI * 2;
                float spawnZoneRadius = Random.Range(lowerRadius/cell_size, upperRadius/cell_size);
                Vector3 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                float distanceBetweenZones = Mathf.Max(1, i * distanceBetweenZonesScale);
                Vector3 candidate = spawnCenter.GetPosition() + dir * spawnZoneRadius * distanceBetweenZones;

                if(IsValid(candidate, points, grid, spawnZoneRadius)) {
                    SpawnZone spawnZone = CreateSpawnZone(candidate, spawnZoneRadius);

                    if (spawnZone.GetNumberOfTilesInZone() >= minimumNumberOfTilesInSpawnZone
                        && spawnZone.GetNumberOfTilesInZone() <= maximumNumberOfTilesInSpawnZone) {
                        points.Add(spawnZone);
                        spawnPoints.Add(spawnZone);
                        grid[(int)(candidate.x / cell_size), (int)(candidate.y / cell_size)] = points.Count;
                        candidateAccepted = true;
                        break;
                    }
                }
            }

            if (!candidateAccepted) {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        return points;
    }

    
    bool IsValid(Vector3 candidate, List<SpawnZone> points, int[,] grid, float spawnZoneRadius) {
        if (!mapManager.IsTraversable(new Pos((int)candidate.x, (int)candidate.y))) {
            return false;
        }

        if (candidate.x >=0 && candidate.x < regionSize.x && candidate.y >= 0 && candidate.y < regionSize.y) {
            int cellX = (int)(candidate.x / cell_size);
            int cellY = (int)(candidate.y / cell_size);
            int numOfCellsToScan = Mathf.CeilToInt(upperRadius / cell_size);

            // Search around the candidate cell
            int searchStartX = Mathf.Max(0, cellX - numOfCellsToScan);
            int searchEndX = Mathf.Min(cellX + numOfCellsToScan, width - 1);
            int searchStartY = Mathf.Max(0, cellY - numOfCellsToScan);
            int searchEndY = Mathf.Min(cellY + numOfCellsToScan, height - 1);

            for (int x = searchStartX; x <= searchEndX; x++) {
                for (int y = searchStartY; y <= searchEndY; y++) {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1) {
                        float dst = (candidate - points[pointIndex].GetPosition()).magnitude;
                        if (dst <= (spawnZoneRadius + points[pointIndex].GetRadius())) {
                            // Candidate too close to the point
                            return false;
                        }
                    }
                    
                }
            }

            return true;
        }
        return false;
    }

    SpawnZone CreateSpawnZone(Vector3 candidate, float spawnZoneRadius) {
        SpawnZone spawnZone = new SpawnZone(candidate, spawnZoneRadius);
        List<Vector3> zoneTiles = new List<Vector3>();

        int cellX = (int)(candidate.x / cell_size);
        int cellY = (int)(candidate.y / cell_size);
        int numOfCellsToScan = Mathf.CeilToInt(spawnZoneRadius / cell_size);

        int searchStartX = Mathf.Max(0, cellX - numOfCellsToScan);
        int searchEndX = Mathf.Min(cellX + numOfCellsToScan, width - 1);
        int searchStartY = Mathf.Max(0, cellY - numOfCellsToScan);
        int searchEndY = Mathf.Min(cellY + numOfCellsToScan, height - 1);

        for (int x = searchStartX; x <= searchEndX; x++) {
            for (int y = searchStartY; y <= searchEndY; y++) {
                if (mapManager.IsTraversable(new Pos(x, y))) {
                    zoneTiles.Add(new Vector3(x, y));
                }
            }
        }
        spawnZone.SetZoneTiles(zoneTiles);

        return spawnZone;
    }
}
