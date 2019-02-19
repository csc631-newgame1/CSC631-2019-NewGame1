using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
	private int width;
	private int height;
	private float cell_size;
	private float wall_height;
	private float texture_scale;
	
    public void generate_floor()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		width = config.width;
		height = config.height;
		cell_size = config.cell_size;
		texture_scale = config.floor_texture_scale;
		wall_height = config.wall_height;
		
		Vector3 offset = new Vector3(width / (2f * cell_size), 0.0f, height / (2f * cell_size));
		offset.y = wall_height;
		
		Vector3 v00 = new Vector3(0f, 0f, 0f) - offset;
		Vector3 v01 = new Vector3(width * cell_size, 0f, 0f) - offset;
		Vector3 v10 = new Vector3(0f, 0f, height * cell_size) - offset;
		Vector3 v11 = new Vector3(width * cell_size, 0f, height * cell_size) - offset;
		
		Vector2 n00 = new Vector2(0f, 0f);
		Vector2 n01 = new Vector2(width / texture_scale, 0f);
		Vector2 n10 = new Vector2(0f, height / texture_scale);
		Vector2 n11 = new Vector2(width / texture_scale, height / texture_scale);
		
		Vector3[] vertices = { v00, v01, v10, v11 };
		Vector2[] uvs = { n00, n01, n10, n11 };
		int[] triangles = { 3, 1, 0, 2, 3, 0 };
		
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh.vertices = vertices;
		mf.mesh.uv = uvs;
		mf.mesh.triangles = triangles;
		mf.mesh.RecalculateNormals();
	}
}
