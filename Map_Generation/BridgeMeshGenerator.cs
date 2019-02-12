using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MapUtils.MapConstants;

public class BridgeMeshGenerator : MonoBehaviour
{
	private class BridgeNode
	{
		public Vector3 vertex;
		public Vector2 uv;
		public int index;
		public BridgeNode(int x, int y, float cell_size)
		{
			this.vertex = new Vector3(x * cell_size, 0.02f, y * cell_size);
			this.uv = new Vector2((x % 2), (y % 2));
			this.index = -1;
		}
	}
	
    private int[,] map;
	private int width;
	private int height;
	private float cell_size;
	private BridgeNode[,] node_map;
	
	private List<Vector3> vertices;
	private List<Vector2> uvs;
	private List<int> triangles;
	private Vector3 offset;
	
	public MeshFilter target_mesh;
	
	public void generate_bridge_mesh(int[,] map, float cell_size, Vector3 offset)
	{
		this.map = map;
		this.width = map.GetLength(0);
		this.height = map.GetLength(1);
		this.cell_size = cell_size;
		this.offset = offset;
		
		vertices = new List<Vector3>();
		uvs = new List<Vector2>();
		triangles = new List<int>();
		
		generate_node_map();
		triangulate_all_bridges();
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();
		
		target_mesh.mesh = mesh;
	}
	
	void generate_node_map()
	{
		node_map = new BridgeNode[width+1, height+1];
		for (int x = 0; x < width+1; x++) {
			for (int y = 0; y < height+1; y++) {
				node_map[x, y] = new BridgeNode(x, y, cell_size);
			}
		}
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
		BridgeNode n00 = node_map[x, y];
		BridgeNode n01 = node_map[x+1, y];
		BridgeNode n10 = node_map[x, y+1];
		BridgeNode n11 = node_map[x+1, y+1];
		
		index_vertices(n00, n01, n10, n11);
		
		write_triangle(n10, n00, n01);
		write_triangle(n10, n01, n11);
	}
	
	void write_triangle(BridgeNode n0, BridgeNode n1, BridgeNode n2)
	{
		triangles.Add(n0.index);
		triangles.Add(n1.index);
		triangles.Add(n2.index);
	}
	
	void index_vertices(params BridgeNode[] nodes)
	{
		foreach (BridgeNode node in nodes) {
			if (node.index == -1) {
				node.index = vertices.Count;
				vertices.Add(node.vertex - offset);
				uvs.Add(node.uv);
			}
		}
	}
}
