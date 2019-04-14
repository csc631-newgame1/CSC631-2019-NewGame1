using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class MapConfiguration : MonoBehaviour
{
    public int width;
	public int height;
	[Range(0, 100)]
	public int fill_percent;
	[Range(0, 30)]
	public int smoothness;
	[Range(1, 100)]
	public int region_cull_threshold;
	
	[Range(0, 10)]
	public float cell_size;
	[Range(0, 10)]
	public float wall_height;
	public float wall_texture_scale;
	public float surface_texture_scale;
	public float bridge_texture_scale;
	public float floor_texture_scale;
    public float object_size_scale;
	
	public int seed { get { return Settings.Seed; } }

    public Vector3 GetOffset() {
        return new Vector3(width / (2f * cell_size), 0.0f, height / (2f * cell_size));
    }

    public System.Random GetRNG() {
        return new System.Random(seed);
    }
	
	public Vector3 gridToWorld(Pos pos) {
		return new Vector3(pos.x * cell_size + cell_size / 2f, 0f, pos.y * cell_size + cell_size / 2f) - GetOffset();
	}
}
