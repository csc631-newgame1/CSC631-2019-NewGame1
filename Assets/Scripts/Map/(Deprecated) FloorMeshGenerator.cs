using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.Type;
using static MapUtils.Dir;
using static MapUtils.MapConstants;

public class FloorMeshGenerator : MonoBehaviour
{
    private class FloorNode
	{
		public Vector3 vertex;
		public Vector2 uv;
		public int index;
		
		public FloorNode(float x, float y, float cell_size) 
		{
			vertex = new Vector3(x * cell_size, 0.0f, y * cell_size);
			uv = new Vector2(x * cell_size, y * cell_size);
			index = -1;
		}
	}
	
	private class MasterFloorNode
	{
		public FloorNode upleft;
		public FloorNode middle;
		
		public MasterFloorNode(int x, int y, float cell_size)
		{
			upleft = new FloorNode(x, y, cell_size);
			middle = new FloorNode((x + 0.5f), (y + 0.5f), cell_size);
		}
	}
	
	private int[,] map;
	private float cell_size;
	private float wall_height;
	private List<List<Cmd>> all_wall_cmds;
	private int width;
	private int height;
	private Vector3 offset;
	
	private MasterFloorNode[,] node_map;
	private List<Vector3> vertices;
	private List<Vector2> uvs;
	private List<int> triangles;
	
	private MeshFilter mesh_target;
	private float texture_scale;
	
	void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.texture_scale = config.surface_texture_scale;
		this.cell_size = config.cell_size;
		this.mesh_target = GetComponent<MeshFilter>();
		this.wall_height = config.wall_height;
	}
	
	public void generate_floor_mesh(int[,] map, List<List<Cmd>> wall_cmds, Vector3 offset)
	{
		set_config_variables();
		this.map = map;
		this.all_wall_cmds = wall_cmds;
		this.width = map.GetLength(0);
		this.height = map.GetLength(1);
		this.offset = offset;
		this.offset.y = wall_height;
		
		this.vertices = new List<Vector3>();
		this.uvs = new List<Vector2>();
		this.triangles = new List<int>();
		
		generate_node_map();
		
		triangulate_solid_floors();
		triangulate_wall_floors();
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
		
		mesh_target.mesh = mesh;
	}
	
	void generate_node_map()
	{
		node_map = new MasterFloorNode[width+1, height+1];
		for (int x = 0; x < width+1; x++) {
			for (int y = 0; y < height+1; y++) {
				node_map[x,y] = new MasterFloorNode(x, y, cell_size);
			}
		}
	}
	
	void triangulate_solid_floors()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (map[x,y] == EMPTY || map[x,y] == BRIDGE) {
					FloorNode n00 = node_map[x + 0, y + 0].upleft;
					FloorNode n01 = node_map[x + 1, y + 0].upleft;
					FloorNode n10 = node_map[x + 0, y + 1].upleft;
					FloorNode n11 = node_map[x + 1, y + 1].upleft;
					
					index_vertices(n00, n01, n10, n11);
					
					triangles.Add(n10.index);
					triangles.Add(n11.index);
					triangles.Add(n01.index);
					
					triangles.Add(n00.index);
					triangles.Add(n10.index);
					triangles.Add(n01.index);
				}
			}
		}
	}
	
	void triangulate_wall_floors()
	{
		foreach(List<Cmd> wall_cmds in all_wall_cmds) {
			foreach(Cmd cmd in wall_cmds) {
				MasterFloorNode n00 = node_map[cmd.pos.x + 0, cmd.pos.y + 0];
				MasterFloorNode n01 = node_map[cmd.pos.x + 1, cmd.pos.y + 0];
				MasterFloorNode n10 = node_map[cmd.pos.x + 0, cmd.pos.y + 1];
				MasterFloorNode n11 = node_map[cmd.pos.x + 1, cmd.pos.y + 1];
				if (cmd.type == CORNER) {
					switch (cmd.dir) {
						// commands are 180* turns from the surface mesh generator
						case RIGHT:
							triangulate_corner(n01.upleft, n11.upleft, n10.upleft); break;
						case DOWN:
							triangulate_corner(n11.upleft, n10.upleft, n00.upleft); break;
						case LEFT:
							triangulate_corner(n10.upleft, n00.upleft, n01.upleft); break;
						case UP:
							triangulate_corner(n00.upleft, n01.upleft, n11.upleft); break;
					}
				}
				if (cmd.type == ALLEY) {
					// Instead of alleys, corners with midpoints are created to counterbalance
					switch (cmd.dir) {
						case LEFT:
							triangulate_corner(n00.upleft, n00.middle, n10.upleft); break;
						case UP:
							triangulate_corner(n01.upleft, n00.middle, n00.upleft); break;
						case RIGHT:
							triangulate_corner(n11.upleft, n00.middle, n01.upleft); break;
						case DOWN:
							triangulate_corner(n10.upleft, n00.middle, n11.upleft); break;
					}
				}
			}
		}
	}
	
	void triangulate_corner(FloorNode n0, FloorNode n1, FloorNode n2)
	{
		index_vertices(n0, n1, n2);
		
		triangles.Add(n2.index);
		triangles.Add(n1.index);
		triangles.Add(n0.index);
	}
	
	void index_vertices(params FloorNode[] nodes)
	{
		foreach(FloorNode node in nodes) {
			if (node.index == -1) {
				node.index = vertices.Count;
				vertices.Add(node.vertex - offset);
				uvs.Add(node.uv / texture_scale);
			}
		}
	}
}
