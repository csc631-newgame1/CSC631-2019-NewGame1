using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MapUtils.MapConstants;

public class BridgeMeshGenerator : MonoBehaviour
{
    private int[,] map;
	private int width;
	private int height;
	private float cell_size;
	private float wall_height;
	
	private List<Vector3> vertices;
	private List<Vector2> uvs;
	private List<int> triangles;
	private Vector3 offset;
	
	private MeshFilter target_mesh;
	
	void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.cell_size = config.cell_size;
		this.wall_height = config.wall_height;
		this.target_mesh = GetComponent<MeshFilter>();
	}
	
	public void generate_bridge_mesh(int[,] map, Vector3 offset)
	{
		set_config_variables();
		this.map = map;
		this.width = map.GetLength(0);
		this.height = map.GetLength(1);
		this.offset = offset;
		this.offset.y -= 0.005f;
		
		vertices = new List<Vector3>();
		uvs = new List<Vector2>();
		triangles = new List<int>();
		
		triangulate_all_bridges();
		
		Mesh mesh = target_mesh.mesh;
		
		mesh.triangles = null;
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
	
	void triangulate_all_bridges()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (map[x, y] == BRIDGE) {
					triangulate_bridge(x, y);
				}
			}
		}
	}
	
	void triangulate_bridge(int x, int y) 
	{
		triangulate_platform(x, y, cell_size / 4);
		
		triangulate_leg((x * cell_size + 0.0625f), (y * cell_size + 0.0625f), 0.0625f, wall_height);
		triangulate_leg((x * cell_size + 0.9375f), (y * cell_size + 0.0625f), 0.0625f, wall_height);
		triangulate_leg((x * cell_size + 0.0625f), (y * cell_size + 0.9375f), 0.0625f, wall_height);
		triangulate_leg((x * cell_size + 0.9375f), (y * cell_size + 0.9375f), 0.0625f, wall_height);
	}
	
	void triangulate_platform(float x, float y, float height)
	{
		triangle_fan(
			new Vector3[] {
				new Vector3((x + 0), 0f, (y + 0) ), new Vector3((x + 0), 0f, (y + 1) ),
				new Vector3((x + 1), 0f, (y + 1) ), new Vector3((x + 1), 0f, (y + 0) ) },
			new Vector2[] {
				new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, height), new Vector2(0f, height) });
		triangulate_leg((x + (cell_size / 2f)), y + (cell_size / 2f), cell_size / 2f, height);
	}
	
	void triangulate_leg(float x, float y, float thickness, float height)
	{
		triangle_fan(
			new Vector3[] {
				new Vector3((x - thickness), 0f			 , (y - thickness)), new Vector3((x + thickness), 0f		  , (y - thickness)),
				new Vector3((x + thickness), -height	 , (y - thickness)), new Vector3((x - thickness), -height	  , (y - thickness)) },
			new Vector2[] {
				new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, height), new Vector2(0f, height) });
		triangle_fan(
			new Vector3[] {
				new Vector3((x + thickness), 0f			 , (y - thickness)), new Vector3((x + thickness), 0f		  , (y + thickness)),
				new Vector3((x + thickness), -height	 , (y + thickness)), new Vector3((x + thickness), -height	  , (y - thickness)) },
			new Vector2[] {
				new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, height), new Vector2(0f, height) });
		triangle_fan(
			new Vector3[] {
				new Vector3((x + thickness), 0f			 , (y + thickness)), new Vector3((x - thickness), 0f		  , (y + thickness)),
				new Vector3((x - thickness), -height	 , (y + thickness)), new Vector3((x + thickness), -height	  , (y + thickness)) },
			new Vector2[] {
				new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, height), new Vector2(0f, height) });
		triangle_fan(
			new Vector3[] {
				new Vector3((x - thickness), 0f			 , (y + thickness)), new Vector3((x - thickness), 0f		  , (y - thickness)),
				new Vector3((x - thickness), -height	 , (y - thickness)), new Vector3((x - thickness), -height	  , (y + thickness)) },
			new Vector2[] {
				new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, height), new Vector2(0f, height) });
	}
	
	void triangle_fan(Vector3[] verts, Vector2[] uv)
	{
		write_triangles((verts.Length - 2) * 3);
		for (int i = 0; i < verts.Length - 2; i ++) {
			vertices.Add(verts[0] - offset);
			vertices.Add(verts[i + 1] - offset);
			vertices.Add(verts[i + 2] - offset);
		}
		for (int i = 0; i < uv.Length - 2; i++) {
			uvs.Add(uv[0]);
			uvs.Add(uv[i + 1]);
			uvs.Add(uv[i + 2]);
		}
	}
	
	void write_triangles(int amt)
	{
		for (int i = 0; i < amt; i++) {
			triangles.Add(vertices.Count + i);
		}
	}
	
}
