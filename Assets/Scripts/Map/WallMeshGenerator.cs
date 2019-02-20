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
		
		public WallNode(float x, float y, float cell_size, float length, float depth, float tscale)
		{
			upper = new Vector3(x * cell_size, 0.0f, y * cell_size);
			lower = new Vector3(x * cell_size, -depth, y * cell_size);
			uv_upper = new Vector2(0.0f, length / tscale);
			uv_lower = new Vector2(depth / tscale, length / tscale);
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
	
	private MeshFilter mesh_target;
	private float depth;
	private float texture_scale;
	
	void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.cell_size = config.cell_size;
		this.depth = config.wall_height;
		this.texture_scale = config.wall_texture_scale;
		this.mesh_target = GetComponent<MeshFilter>();
	}
	
    public void generate_wall_mesh(List<List<Cmd>> wall_cmds, Vector3 offset)
	{
		set_config_variables();
		this.all_wall_cmds = wall_cmds;
		this.offset = offset;
		
		this.vertices = new List<Vector3>();
		this.uvs = new List<Vector2>();
		this.triangles = new List<int>();
		
		gen_nodes_from_cmds();
		gen_verts_from_nodes();
		
		Mesh mesh = mesh_target.mesh;
		
		mesh.triangles = null;
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
			wall_cmds.Add(wall_cmds[0]); // nodes need to wrap around so that final segment doesn't have warped uvs
			foreach(Cmd cmd in wall_cmds) {
				
				int x = cmd.pos.x;
				int y = cmd.pos.y;
				WallNode endpt = null;
				WallNode midpt = new WallNode((x + 0.5f), (y + 0.5f), cell_size, length + (sqrt2 / 2f), depth, texture_scale);
				
				switch (cmd.dir)
				{
					case LEFT :
						endpt = new WallNode((x + 0), (y + 1), cell_size, length, depth, texture_scale); break;
					case UP :
						endpt = new WallNode((x + 0), (y + 0), cell_size, length, depth, texture_scale); break;
					case RIGHT :
						endpt = new WallNode((x + 1), (y + 0), cell_size, length, depth, texture_scale); break;
					case DOWN :
						endpt = new WallNode((x + 1), (y + 1), cell_size, length, depth, texture_scale); break;
				}
				switch (cmd.type)
				{
					case LINE :
						length += cell_size; break;
					case CORNER :
						length += cell_size * sqrt2; break;
					case ALLEY :
						length += cell_size * sqrt2; break;
				}
				
				nodes.Add(endpt);
				if (cmd.type == ALLEY)
					nodes.Add(midpt);
			}
			wall_cmds.RemoveAt(wall_cmds.Count - 1); // remove the wrap-around command
			all_nodes.Add(nodes);
		}
	}
	
	/* Something to note about wall mesh generation -
	 * We don't want the wall segments to share vertices, otherwise vertex normals will be interpolated between during rendering
	 * This makes the wall come across as having smooth edges, instead of the flat & sharp edges that we desire
	 */
	void gen_verts_from_nodes()
	{
		foreach(List<WallNode> nodes in all_nodes) {
			for(int i = 1; i < nodes.Count; i++) {
				create_wall_triangles(nodes[i], nodes[i-1]);
			}
			create_wall_triangles(nodes[0], nodes[nodes.Count-1]);
		}
	}
	
	void create_wall_triangles(WallNode n0, WallNode n1)
	{
		int ind = vertices.Count;
		int[] indices = { ind + 0, ind + 1, ind + 2, ind + 3, ind + 4, ind + 5 };
		triangles.AddRange(indices);
		
		vertices.Add(n0.lower - offset);
		vertices.Add(n1.lower - offset);
		vertices.Add(n0.upper - offset);
		
		vertices.Add(n1.lower - offset);
		vertices.Add(n1.upper - offset);
		vertices.Add(n0.upper - offset);
		
		uvs.Add(n0.uv_lower);
		uvs.Add(n1.uv_lower);
		uvs.Add(n0.uv_upper);
		
		uvs.Add(n1.uv_lower);
		uvs.Add(n1.uv_upper);
		uvs.Add(n0.uv_upper);
	}
}
