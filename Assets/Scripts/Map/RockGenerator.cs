using UnityEngine;
using static MapUtils.MapConstants;

public class RockGenerator : MonoBehaviour {

    private int[,] map;
    private float cell_size;
    private int width;
    private int height;
    private Vector3 offset;

    public Transform prefab;

    public void generateRocks(int[,] map, Vector3 offset, float objectSizeScale) {
        this.map = map;
        this.width = map.GetLength(0);
        this.height = map.GetLength(1);
        this.offset = offset;

        //for (int x = 0; x < width; x++) {
        //    for (int y = 0; y < height; y++) {
        //        if (map[x, y] >= FILLED) {
        //            Instantiate(prefab, (new Vector3(x, 0, y) - offset), Quaternion.identity);
        //        }
        //    }
        //}
        prefab.transform.localScale = new Vector3(transform.localScale.x/objectSizeScale, transform.localScale.y / objectSizeScale, transform.localScale.z / objectSizeScale);
        Instantiate(prefab, (new Vector3(1, 0, 1) - offset), Quaternion.identity);
    }
}