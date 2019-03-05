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

    public float spawnZoneSize = 2.5f;

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

    public List<SpawnZone> GeneratePoints(int numSamplesBeforeRejection = 40) {
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
                float spawnZoneRadius = Random.Range(radius, spawnZoneSize * radius);
                Vector3 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                float distanceIncrease = Mathf.Max(1, i / 10);
                Vector3 candidate = spawnCenter.GetPosition() + dir * spawnZoneRadius * distanceIncrease;

                if(IsValid(candidate, points, grid, spawnZoneRadius)) {
                    SpawnZone newSpawnZone = new SpawnZone(candidate, spawnZoneRadius);
                    points.Add(newSpawnZone);
                    spawnPoints.Add(newSpawnZone);
                    grid[(int)(candidate.x / cell_size), (int)(candidate.y / cell_size)] = points.Count;
                    candidateAccepted = true;
                    break;
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
            int numOfCellsToScan = Mathf.CeilToInt(spawnZoneRadius / cell_size);

            // Search around the candidate cell
            int searchStartX = Mathf.Max(0, cellX - numOfCellsToScan);
            int searchEndX = Mathf.Min(cellX + numOfCellsToScan, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - numOfCellsToScan);
            int searchEndY = Mathf.Min(cellY + numOfCellsToScan, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++) {
                for (int y = searchStartY; y <= searchEndY; y++) {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1) {
                        float dst = (candidate - points[pointIndex].GetPosition()).magnitude;
                        if (dst < (spawnZoneRadius + points[pointIndex].GetRadius())) {
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
}
