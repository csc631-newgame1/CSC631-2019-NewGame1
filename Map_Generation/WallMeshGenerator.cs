using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.Type;
using static MapUtils.Dir;

public class WallMeshGenerator : MonoBehaviour
{
	private class WallNode
	{
		public Vector3 upper;
		public Vector3 lower;
		public Vector2 uv_upper;
		public Vector2 uv_lower;
		public int indx_upper;
		public int indx_lower;
		
		public WallNode(float x, float y, float cell_size, float length, float depth)
		{
			upper = new Vector3(x * cell_size, 0.0f, y * cell_size);
			lower = new Vector3(x * cell_size, -depth, y * cell_size);
			uv_upper = new Vector2(0.0f, length);
			uv_lower = new Vector2(depth, length);
			indx_upper = -1;
			indx_lower = -1;
		}
	}
	
	private List<Vector3> vertices;
	private List<Vector2> uvs;
	private List<int> triangles;
	private List<List<WallNode>> all_nodes;
	private List<List<Cmd>> all_wall_cmds;
	private float cell_size;
	private static float sqrt2 = Mathf.Sqrt(2);
	private Vector3 offset;
	
	public MeshFilter mesh_target;
	[Range(-10, 0)]
	public float depth;
	[Range(10, 100)]
	public float texture_scale;
	
    public void generate_wall_mesh(List<List<Cmd>> wall_cmds, float cell_size, Vector3 offset)
	{
		this.all_wall_cmds = wall_cmds;
		this.cell_size = cell_size;
		this.offset = offset;
		
		this.vertices = new List<Vector3>();
		this.uvs = new List<Vector2>();
		this.triangles = new List<int>();
		
		gen_nodes_from_cmds();
		gen_verts_from_nodes();
		
		Mesh mesh = new Mesh();
		mesh_target.mesh = mesh;
		
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
	
	void gen_nodes_from_cmds()
	{
		all_nodes = new List<List<WallNode>>();
		foreach (List<Cmd> wall_cmds in all_wall_cmds) {
			List<WallNode> nodes = new List<WallNode>();
			float length = 0;
			foreach(Cmd cmd in wall_cmds) {
				int x = cmd.pos.x;
				int y = cmd.pos.y;
				if (cmd.type == LINE) {
					length += 1 * cell_size;
				}
				else {
					length += sqrt2 * cell_size;
				}
				WallNode endpt = new WallNode(x, y, cell_size, length, depth); // compiler gets upset if endpt is unassigned
				switch (cmd.dir)
				{
					case LEFT :
						endpt = new WallNode((x + 0), (y + 1), cell_size, length, depth); break;
					case UP :
						endpt = new WallNode((x + 0), (y + 0), cell_size, length, depth); break;
					case RIGHT :
						endpt = new WallNode((x + 1), (y + 0), cell_size, length, depth); break;
					case DOWN :
						endpt = new WallNode((x + 1), (y + 1), cell_size, length, depth); break;
				}
				switch (cmd.type)
				{
					case LINE :
						nodes.Add(endpt);
						break;
					case CORNER :
						nodes.Add(endpt);
						break;
					case ALLEY :
						WallNode midpt = new WallNode((x + 0.5f), (y + 0.5f), cell_size, length - sqrt2 / 2.0f, depth);
						nodes.Add(endpt);
						nodes.Add(midpt);
						break;
				}
			}
			all_nodes.Add(nodes);
		}
	}
	
	void gen_verts_from_nodes()
	{
		foreach(List<WallNode> nodes in all_nodes) {
			index_vert(nodes[0]);
			for(int i = 1; i < nodes.Count; i++) {
				index_vert(nodes[i]);
				create_wall_triangles(nodes[i], nodes[i-1]);
			}
			create_wall_triangles(nodes[0], nodes[nodes.Count-1]);
		}
	}
	
	void index_vert(WallNode node)
	{
		node.indx_upper = vertices.Count;
		vertices.Add(node.upper - offset);
		uvs.Add(node.uv_upper / texture_scale);
		node.indx_lower = vertices.Count;
		vertices.Add(node.lower - offset);
		uvs.Add(node.uv_lower / texture_scale);
	}
	
	void create_wall_triangles(WallNode n0, WallNode n1)
	{
		triangles.Add(n0.indx_upper);
		triangles.Add(n1.indx_lower);
		triangles.Add(n0.indx_lower);
		
		triangles.Add(n0.indx_upper);
		triangles.Add(n1.indx_upper);
		triangles.Add(n1.indx_lower);
	}
}
