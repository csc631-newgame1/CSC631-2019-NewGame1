using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
	public float wave_height = 0.5f;
	public float wave_length = 0.75f;
	public float wave_frequency = 0.5f;
	public int width_segments = 15;
	public int height_segments = 15;
	
	private List<float> distances;
	private List<Vector3> vertices;
	private List<Vector2> uvs;
	private List<int> triangles;
	private Vector3 base_offset;
	private Vector3 wave_offset;
	private Vector3 floor_offset;
	private Vector3 origin;
	private int width;
	private int height;
	private float cell_size;
	private float wall_height;
	
	MeshFilter mesh_target;
	Mesh mesh;
	
	void Awake()
	{
		set_config_variables();
	}
	
	void set_config_variables()
	{
		mesh_target = GetComponent<MeshFilter>();
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.width = config.width;
		this.height = config.height;
		this.cell_size = config.cell_size;
		this.origin = new Vector3(0.5f, 0f, 0.5f);
		this.wall_height = config.wall_height;
		this.base_offset = config.GetOffset();
	}
	
	void Update()
	{
		if (mesh != null) {
			update_waves();
		}
	}
	
    public void generate_water_mesh()
	{
		set_config_variables();
		
		this.wave_offset = base_offset;
		this.floor_offset = base_offset;
		this.wave_offset.y  = (wall_height / 2f);
		this.floor_offset.y = (wall_height);
		
		vertices = new List<Vector3>();
		distances = new List<float>();
		triangles = new List<int>();
		
		float tile_x = ((float) width * cell_size) / (float) width_segments;
		float tile_y = ((float) height * cell_size) / (float) height_segments;
		
		for (int x = 0; x < width_segments; x++)
			for (int y = 0; y < height_segments; y++)
				write_two_surface_triangles(x, y, tile_x, tile_y);
		
		for (int x = width_segments; x > 0; x--)
			write_two_wall_triangles(x, height_segments, -1, 0, tile_x, tile_y);
		for (int x = 0; x < width_segments; x++)
			write_two_wall_triangles(x, 0, 1, 0, tile_x, tile_y);
		for (int y = height_segments; y > 0; y--)
			write_two_wall_triangles(0, y, 0, -1, tile_x, tile_y);
		for (int y = 0; y < height_segments; y++)
			write_two_wall_triangles(width_segments, y, 0, 1, tile_x, tile_y);
			
		for (int i = 0; i < vertices.Count; i++) {
			float distance = Vector3.Distance(vertices[i], origin);
			distance = (distance % wave_length) / wave_length;
			distances.Add(distance);
		}
		
		mesh = mesh_target.mesh;
		mesh.triangles = null;
		mesh.vertices = vertices.ToArray();
		//mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
	
	// Water mesh is generated in a similar way to the wall mesh, i.e vertices are not shared between triangles
	void write_two_surface_triangles(int x, int y, float xscale, float yscale)
	{
		int ind = vertices.Count;
		int[] indices = { ind + 0, ind + 1, ind + 2, ind + 3, ind + 4, ind + 5 };
		triangles.AddRange(indices);
		
		vertices.Add(new Vector3((x + 1) * xscale, 0f, (y + 1) * yscale) - wave_offset);
		vertices.Add(new Vector3((x + 1) * xscale, 0f, (y + 0) * yscale) - wave_offset);
		vertices.Add(new Vector3((x + 0) * xscale, 0f, (y + 0) * yscale) - wave_offset);
		
		vertices.Add(new Vector3((x + 0) * xscale, 0f, (y + 1) * yscale) - wave_offset);
		vertices.Add(new Vector3((x + 1) * xscale, 0f, (y + 1) * yscale) - wave_offset);
		vertices.Add(new Vector3((x + 0) * xscale, 0f, (y + 0) * yscale) - wave_offset);
	}
	
	// walls surround the border of the map, facing outwards
	void write_two_wall_triangles(int x, int y, int xi, int yi, float xscale, float yscale)
	{
		int ind = vertices.Count;
		int[] indices = { ind + 0, ind + 1, ind + 2, ind + 3, ind + 4, ind + 5 };
		triangles.AddRange(indices);

		Vector3 v1 = new Vector3((x +  0) * xscale, 0f, (y +  0) * yscale);
		Vector3 v2 = new Vector3((x + xi) * xscale, 0f, (y + yi) * yscale);
		
		vertices.Add(v1 - wave_offset);
		vertices.Add(v2 - wave_offset);
		vertices.Add(v2 - floor_offset);
		
		vertices.Add(v1 - wave_offset);
		vertices.Add(v2 - floor_offset);
		vertices.Add(v1 - floor_offset);
	}
	
	void update_waves()
	{	
		for(int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			
			if (vertex.y < -((wall_height / 1.9f) + wave_height)) // don't modify bottom wall vertices
				continue;
			
			//Oscilate the wave height via sine to create a wave effect
			vertex.y = wave_height * Mathf.Sin(Time.time * Mathf.PI * 2.0f * wave_frequency
			+ (Mathf.PI * 2.0f * distances[i])) - wave_offset.y;
			
			vertices[i] = vertex;
		}
		
		mesh.MarkDynamic();
		mesh.vertices = vertices.ToArray();
		mesh.RecalculateNormals();
	}
}
