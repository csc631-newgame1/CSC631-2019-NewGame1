﻿	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class MapMeshGenerator : MonoBehaviour
{	
	private const int HMAP_FILLED = -1;
	private const int HMAP_EMPTY = 0;
	private const int HMAP_EDGE = 1;
	
	private int width;
	private int height;
	private float cell_size;
	private float wall_height;
	private Vector3 offset;
	public Gradient gradient;
	
	void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.width = config.width;
		this.height = config.height;
		this.cell_size = config.cell_size;
		this.wall_height = config.wall_height;
		this.offset = config.GetOffset();
	}
	
	public void generate_map_mesh(int[,] map)
	{
		/* STEP 1: INIT COMPONENTS
		 * Intermediate and height map arrays are constructed 
		 * the intermediate map holds only the data that is relevant to height map construction
		 */
		
			set_config_variables();
			
			int[,] intermediate_map = new int[width, height];
			float[,] height_map = new float[width, height];
			
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					// exclude bridges and platforms, their mesh is generated separately
					if (traversable(map[x, y]) && map[x, y] != BRIDGE && map[x, y] != PLATFORM) {
						
						if (x - 1 >= 0 && !traversable(map[x - 1, y]))
							intermediate_map[x - 1, y] = HMAP_EDGE;
						
						if (x + 1 < width && !traversable(map[x + 1, y]))
							intermediate_map[x + 1, y] = HMAP_EDGE;
						
						if (y - 1 >= 0 && !traversable(map[x, y - 1]))
							intermediate_map[x, y - 1] = HMAP_EDGE;
						
						if (y + 1 < height && !traversable(map[x, y + 1]))
							intermediate_map[x, y + 1] = HMAP_EDGE;
						
						intermediate_map[x, y] = HMAP_FILLED;
					}
				
		/* STEP 2: GENERATE HEIGHT MAP
		 * Intermediate map is iterated over multiple times to determine each cell's distance to a walkable cell
		 * These distances are recorded in the height map, which is used in the next step
		 */
				
			bool finished = false;
			int curr_iteration = 1;
			
			while (!finished) {
				
				finished = true;
				int[,] temp = intermediate_map.Clone() as int[,];
				
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						
						if (intermediate_map[x, y] == HMAP_EDGE) {
							
							finished = false;
							height_map[x, y] = height_func(curr_iteration, x, y);
							
							if (x > 0 && intermediate_map[x - 1, y] == HMAP_EMPTY)
								temp[x - 1, y] = HMAP_EDGE;
							
							if (x < width - 1 && intermediate_map[x + 1, y] == HMAP_EMPTY)
								temp[x + 1, y] = HMAP_EDGE;
							
							if (y > 0 && intermediate_map[x, y - 1] == HMAP_EMPTY)
								temp[x, y - 1] = HMAP_EDGE;
							
							if (y < height - 1 && intermediate_map[x, y + 1] == HMAP_EMPTY)
								temp[x, y + 1] = HMAP_EDGE;
							
							temp[x, y] = HMAP_FILLED;
						}
					}
				}
				intermediate_map = temp.Clone() as int[,];
				curr_iteration++;
			}
		
		/* STEP 3: CREATE MESH NODES
		 * Now, a surface mesh is created according to these generated height values
		 * Optionally, a function may be applied to each height value, to amplify or minimize terrain in some fashion 
		 */
		
			Vector3[,] vertex_map = new Vector3[width * 2 + 1, height * 2 + 1];
			
			// initialize vertex_map with vector x and z values
			for(int x = 0; x < width * 2 + 1; x++)
				for(int y = 0; y < height * 2 + 1; y++)
					vertex_map[x, y] = new Vector3((float) x / 2f, 0f, (float) y / 2f) - offset;
			
			// Generate vertex_map height values based on height map
			// vertex heights are the average heights of all the tiles they touch
			// e.g: the lower right vertex of a tile actually lies on the intersection of four tiles, so it is the average of their heights
			for (int x = 1; x < width * 2 + 1; x+=2) {
				for (int y = 1; y < height * 2 + 1; y+=2) {
					
					float center_tile, lower_tile, right_tile, lower_right_tile;
					center_tile = height_map[(x - 1) / 2, (y - 1) / 2];
					lower_tile = right_tile = lower_right_tile = center_tile;
					
					if (y < height * 2 - 1)
						lower_tile = height_map[(x - 1) / 2, (y + 1) / 2];
					if (x < width * 2 - 1)
						right_tile = height_map[(x + 1) / 2, (y - 1) / 2];
					if (y < height * 2 - 1 && x < width * 2 - 1)
						lower_right_tile = height_map[(x + 1) / 2, (y + 1) / 2];
					
					vertex_map[x + 0, y + 0].y = (center_tile) / 1f;
					vertex_map[x + 1, y + 0].y = (center_tile + right_tile) / 2f;
					vertex_map[x + 0, y + 1].y = (center_tile + lower_tile) / 2f;
					vertex_map[x + 1, y + 1].y = (center_tile + right_tile + lower_tile + lower_right_tile) / 4f;
				}
			}
			
			// finish up around the edges
			vertex_map[0, 0].y = height_map[0, 0];
			
			for(int x = 1; x < width * 2 + 1; x+=2) {
				
				float center_tile, right_tile;
				right_tile = center_tile = height_map[(x - 1) / 2, 0];
				
				if (x < width * 2 - 1)
					right_tile  = height_map[(x + 1) / 2, 0];
				
				vertex_map[x, 0].y		= (center_tile) / 1f;
				vertex_map[x + 1, 0].y 	= (center_tile + right_tile) / 2f;
			}
			
			for(int y = 1; y < height * 2 + 1; y+=2) {
				
				float center_tile, lower_tile;
				lower_tile = center_tile = height_map[0, (y - 1) / 2];
				
				if (y < height * 2 - 1)
					lower_tile  = height_map[0, (y + 1) / 2];
				
				vertex_map[0, y].y		= (center_tile) / 1f;
				vertex_map[0, y + 1].y 	= (center_tile + lower_tile) / 2f;
			}
			
			// anchor all vertices that touch walkable tiles to height 0
			for (int x = 1; x < width * 2 + 1; x+=2) {
				for (int y = 1; y < height * 2 + 1; y+=2) {
					// exclude bridges and platforms in this calculation, they are not part of the terrain
					if (traversable(map[(x - 1) / 2, (y - 1) / 2]) && map[(x - 1) / 2, (y - 1) / 2] != BRIDGE && map[(x - 1) / 2, (y - 1) / 2] != PLATFORM) {
						vertex_map[x - 1, y - 1].y = 0;
						vertex_map[x - 0, y - 1].y = 0;
						vertex_map[x + 1, y - 1].y = 0;
						
						vertex_map[x - 1, y - 0].y = 0;
						vertex_map[x - 0, y - 0].y = 0;
						vertex_map[x + 1, y - 0].y = 0;
						
						vertex_map[x - 1, y + 1].y = 0;
						vertex_map[x - 0, y + 1].y = 0;
						vertex_map[x + 1, y + 1].y = 0;
					}
					// if the tile is an edge, just anchor the center vertex
					else if (map[(x - 1) / 2, (y - 1) / 2] == EDGE) {
						vertex_map[x + 0, y + 0].y = 0;
					}
				}
			}
		
		/* STEP 4: CREATE TRIANGLES & FINALIZE MESH
		 * Once the mesh nodes are created, an array of triangles, representing the map surface, is constructed from it 
		 */

		Vector3[] vertices = new Vector3[width * height * 4 * 2 * 3]; // width * height tiles, 4 faces per tile, 2 triangles per face, 3 verts/triangle, 
		Vector2[] uvs = new Vector2[width * height * 4 * 2 * 3];
		Vector2[] uv2s = new Vector2[width * height * 4 * 2 * 3];
		//Color[] colors = new Color[width * height * 4 * 2 * 3];
		int[] triangles = new int[width * height * 4 * 2 * 3];
		
		int i = 0; // keeps track of triangles index
		
		for (int x = 0; x < width * 2; x++) {
			for (int y = 0; y < height * 2; y++) {
				
				Color col = gradient.Evaluate(Mathf.Clamp01(Mathf.Abs(height_map[x / 2, y / 2]) / wall_height));
				
				Vector3 v0 = vertex_map[x + 0, y + 0];
				Vector3 v1 = vertex_map[x + 1, y + 0];
				Vector3 v2 = vertex_map[x + 0, y + 1];
				Vector3 v3 = vertex_map[x + 1, y + 1];

				// orientation of triangles changes in a regular pattern according to which corner of each tile it occupies
				// a final tile face will be triangulated something like this (excuse the bad text drawing):
				/*
				 * -----------
				 * | \  |   /|  8 triangles, 4 faces per tile, 2 triangles per face
				 * |  \ | /  |
				 * |---------|  All hypotenuse aim towards the center of the tile
				 * |  / | \  |
				 * |/   |   \|
				 * |-----------
				 * 
				 * This creates a repeating pattern in the tile geometry, so that it is not biased towards any particular orientation
				 */   
				 
				if ((x % 2) == 1) {
					SWAPv(ref v0, ref v1);
					SWAPv(ref v2, ref v3);
				}
				
				if ((y % 2) == 1) {
					SWAPv(ref v0, ref v2);
					SWAPv(ref v1, ref v3);
				}
					// because of this re-ordering of vertices, the winding order of the triangles needs to be taken into account
					// With an odd number of swaps, the winding order for the triangles is reversed
					if (((x + y) % 2) == 1) {
						vertices[i + 0] = v0;
						vertices[i + 1] = v1;
						vertices[i + 2] = v3;
						
						vertices[i + 3] = v2;
						vertices[i + 4] = v0;
						vertices[i + 5] = v3;
						
						uvs[i + 0] = new Vector2(Mathf.Abs(v0.y / wall_height), 0);
						uvs[i + 1] = new Vector2(Mathf.Abs(v1.y / wall_height), 0);
						uvs[i + 2] = new Vector2(Mathf.Abs(v3.y / wall_height), 0);
						
						uvs[i + 3] = new Vector2(Mathf.Abs(v2.y / wall_height), 0);
						uvs[i + 4] = new Vector2(Mathf.Abs(v0.y / wall_height), 0);
						uvs[i + 5] = new Vector2(Mathf.Abs(v3.y / wall_height), 0);
						
						uv2s[i + 0] = new Vector2(v0.x, v0.z);
						uv2s[i + 1] = new Vector2(v1.x, v1.z);
						uv2s[i + 2] = new Vector2(v3.x, v3.z);
						
						uv2s[i + 3] = new Vector2(v2.x, v2.z);
						uv2s[i + 4] = new Vector2(v0.x, v0.z);
						uv2s[i + 5] = new Vector2(v3.x, v3.z);
						
						/*
						colors[i + 0] = gradient.Evaluate(Mathf.Abs(v0.y / wall_height));
						colors[i + 1] = gradient.Evaluate(Mathf.Abs(v1.y / wall_height));
						colors[i + 2] = gradient.Evaluate(Mathf.Abs(v3.y / wall_height));
						
						colors[i + 3] = gradient.Evaluate(Mathf.Abs(v2.y / wall_height));
						colors[i + 4] = gradient.Evaluate(Mathf.Abs(v0.y / wall_height));
						colors[i + 5] = gradient.Evaluate(Mathf.Abs(v3.y / wall_height));*/
					}
					else {
						vertices[i + 0] = v3;
						vertices[i + 1] = v1;
						vertices[i + 2] = v0;
						
						vertices[i + 3] = v3;
						vertices[i + 4] = v0;
						vertices[i + 5] = v2;
						
						uvs[i + 0] = new Vector2(Mathf.Abs(v3.y / wall_height), 0);
						uvs[i + 1] = new Vector2(Mathf.Abs(v1.y / wall_height), 0);
						uvs[i + 2] = new Vector2(Mathf.Abs(v0.y / wall_height), 0);
						
						uvs[i + 3] = new Vector2(Mathf.Abs(v3.y / wall_height), 0);
						uvs[i + 4] = new Vector2(Mathf.Abs(v0.y / wall_height), 0);
						uvs[i + 5] = new Vector2(Mathf.Abs(v2.y / wall_height), 0);
						
						uv2s[i + 0] = new Vector2(v3.x, v3.z);
						uv2s[i + 1] = new Vector2(v1.x, v1.z);
						uv2s[i + 2] = new Vector2(v0.x, v0.z);
						
						uv2s[i + 3] = new Vector2(v3.x, v3.z);
						uv2s[i + 4] = new Vector2(v0.x, v0.z);
						uv2s[i + 5] = new Vector2(v2.x, v2.z);
						
						/*colors[i + 0] = gradient.Evaluate(Mathf.Abs(v3.y / wall_height));
						colors[i + 1] = gradient.Evaluate(Mathf.Abs(v1.y / wall_height));
						colors[i + 2] = gradient.Evaluate(Mathf.Abs(v0.y / wall_height));
						
						colors[i + 3] = gradient.Evaluate(Mathf.Abs(v3.y / wall_height));
						colors[i + 4] = gradient.Evaluate(Mathf.Abs(v0.y / wall_height));
						colors[i + 5] = gradient.Evaluate(Mathf.Abs(v2.y / wall_height));*/
					}
				
					triangles[i + 0] = i;
					triangles[i + 1] = i + 1;
					triangles[i + 2] = i + 2;
					
					triangles[i + 3] = i + 3;
					triangles[i + 4] = i + 4;
					triangles[i + 5] = i + 5;
				
				i += 6;
			}
		}
		
		MeshFilter mf = GetComponent<MeshFilter>();
		
		// unless indexFormat Uint32 is specified, mesh index goes up to only 65536 (16-bit)
		mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mf.mesh.triangles = null;
		
		mf.mesh.vertices = vertices;
		mf.mesh.uv = uvs;
		mf.mesh.uv2 = uv2s;
		//mf.mesh.colors = colors;
		mf.mesh.triangles = triangles;
		mf.mesh.RecalculateNormals();
	}
	
	private void SWAPv(ref Vector3 a, ref Vector3 b)
	{
		Vector3 tmp = a;
		a = b;
		b = tmp;
	}
	
	private void SWAPt(ref Vector2 a, ref Vector2 b)
	{
		Vector2 tmp = a;
		a = b;
		b = tmp;
	}
	
	private float height_func(int hval, int x, int y)
	{
		return -wall_height * ((sigmoid(hval / 2f) - 0.5f) * 2);
	}
	
	private float sigmoid(float x)
	{
		return 1f / (1 + Mathf.Exp(-x));
	}
	
	private char intermediate_char(int int_val)
	{
		switch(int_val)
		{
			case -1 : return ' ';
			case 0  : return '#';
			case 1  : return 'E';
			default : return ' ';
		}
	}
	
	private char height_char(int height_val)
	{
		switch(height_val)
		{
			case 0 : return ' ';
			case 1 : return '/';
			case 2 : return '%';
			case 3 : return '@';
			case 4 : return '&';
			default: return '#';
		}
	}
	
	// code jail:
	/*int adj_tiles = 0;
				int surf_tiles = 0;
				float height_avg = 0;
				
				if (x < width && y < height) {
					height_avg += height_map[x, y];
					surf_tiles += height_map[x, y] == 0 ? 1 : 0;
					adj_tiles++;
				}
				if (x - 1 >= 0 && y < height) {
					height_avg += height_map[x - 1, y];
					surf_tiles += height_map[x - 1, y] == 0 ? 1 : 0;
					adj_tiles++;
				}
				if (x < width && y - 1 >= 0) {
					height_avg += height_map[x, y - 1];
					surf_tiles += height_map[x, y - 1] == 0 ? 1 : 0;
					adj_tiles++;
				}
				if (x - 1 >= 0 && y - 1 >= 0) {
					height_avg += height_map[x - 1, y - 1];
					surf_tiles += height_map[x - 1, y - 1] == 0 ? 1 : 0;
					adj_tiles++;
				}
				
				height_avg /= (float) adj_tiles;

				if (surf_tiles > 0)
					height_avg = 0;
				
				vertex_map[x, y] = new Vector3(x * cell_size, -height_avg, y * cell_size) - offset;*/
				
	/*GradientColorKey[] color_key = new GradientColorKey[6];
		color_key[0].color = new Color(0.9f, 0.1f, 0.1f);
		color_key[0].time = 1f;
		color_key[1].color = new Color(168f / 255f, 0f, 0f);
		color_key[1].time = 4f / 5f;
		color_key[2].color = new Color(0.4f, 0.4f, 0.4f);
		color_key[2].time = 3f / 5f;
		color_key[3].color = new Color(238f / 255f, 1f, 0f);
		color_key[3].time = 2f / 5f;
		color_key[4].color = new Color(140f / 255f, 196f / 255f, 0f);
		color_key[4].time = 1f / 5f;
		color_key[5].color = new Color(84f / 255f, 117f / 255f, 0f);
		color_key[5].time = 0f;*/
}