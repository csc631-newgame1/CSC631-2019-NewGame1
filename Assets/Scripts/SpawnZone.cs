using System.Collections.Generic;
using UnityEngine;

// A number of tiles which enemies are able to spawn in
public class SpawnZone {
    // Center of the Spawn Zone
    private Vector3 position;
    // Reach of the Spawn Zone
    private float radius;
    // Traversable tiles within the Spawn Zone
    private List<Vector3> zoneTiles;

    public SpawnZone(Vector3 position, float radius) {
        this.position = position;
        this.radius = radius;
        zoneTiles = new List<Vector3>();
    }

    // Returns the center of the Spawn Zone
    public Vector3 GetPosition() {
        return position;
    }

    // Returns the radius of the Spawn Zone
    public float GetRadius() {
        return radius;
    }

    // Sets the traversable tiles within the radius of the Spawn Zone
    public void SetZoneTiles(List<Vector3> zoneTiles) {
        this.zoneTiles = zoneTiles;
    }

    // Returns the number of traversable tiles within the radius
    // of the Spawn Zone
    public int GetNumberOfTilesInZone() {
        return zoneTiles.Count;
    }
}
