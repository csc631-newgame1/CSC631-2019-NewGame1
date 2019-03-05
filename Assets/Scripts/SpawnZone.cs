using System.Collections.Generic;
using UnityEngine;

public class SpawnZone {
    private Vector3 position;
    private float radius;
    private List<Vector3> zoneTiles;

    public SpawnZone(Vector3 position, float radius) {
        this.position = position;
        this.radius = radius;
        zoneTiles = new List<Vector3>();
    }

    public Vector3 GetPosition() {
        return position;
    }

    public float GetRadius() {
        return radius;
    }

    public void SetZoneTiles(List<Vector3> zoneTiles) {
        this.zoneTiles = zoneTiles;
    }

    public int GetNumberOfTilesInZone() {
        return zoneTiles.Count;
    }
}
