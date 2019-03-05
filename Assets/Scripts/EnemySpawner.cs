using MapUtils;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private int width;
    private int height;
    private float cell_size;
    private Vector3 offset;
    private Vector3 regionSize;
    private float radius;
    private MapManager mapManager;

    public void Init(MapManager _mapManager)
    {
        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        this.width = config.width;
        this.height = config.height;
        regionSize = new Vector2(width, height);
        this.cell_size = config.cell_size;
        this.radius = cell_size * Mathf.Sqrt(2);
        this.offset = config.GetOffset();
        this.mapManager = _mapManager;
    }

    public List<Vector3> GeneratePoints(int numSamplesBeforeRejection = 30) {
        int[,] grid = new int[width, height];
        List<Vector3> points = new List<Vector3>();
        List<Vector3> spawnPoints = new List<Vector3>();

        spawnPoints.Add(regionSize / 2);
        while (spawnPoints.Count > 0) {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector3 spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i=0; i<numSamplesBeforeRejection; i++) {
                float angle = Random.value * Mathf.PI * 2;
                Vector3 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector3 candidate = spawnCenter + dir * Random.Range(radius, 2 * radius);

                if(IsValid(candidate, points, grid)) {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
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

    
    bool IsValid(Vector3 candidate, List<Vector3> points, int[,] grid) {
        if (!mapManager.IsTraversable(new Pos((int)candidate.x, (int)candidate.y))) {
            return false;
        }

        if (candidate.x >=0 && candidate.x < regionSize.x && candidate.y >= 0 && candidate.y < regionSize.y) {
            int cellX = (int)(candidate.x / cell_size);
            int cellY = (int)(candidate.y / cell_size);

            // Search around 5 cells
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++) {
                for (int y = searchStartY; y <= searchEndY; y++) {
                    

                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1) {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDst < radius * radius) {
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
