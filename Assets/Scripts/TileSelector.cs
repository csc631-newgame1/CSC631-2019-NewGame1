using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class TileSelector : MonoBehaviour
{
	private int width;
	private int height;
	private float cell_size;
	private Vector3 offset;
	
	private Transform select_square;
	
	private int[,] map;
	
	public Vector3 hover_position;
	public Pos grid_position;
	
	public void init_tile_selector(int[,] map)
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		cell_size = config.cell_size;
		width = config.width;
		height = config.height;
		offset = new Vector3(width / (2f * cell_size), 0f, height / (2f * cell_size));
		
		this.map = map;
		
		Vector3 collider_size = new Vector3(width * cell_size, 1f, height * cell_size);
		BoxCollider selection_collider = GetComponent<BoxCollider>();
		selection_collider.size = collider_size;
		
		select_square = transform.Find("Selector");
	}
	
	void Update()
	{
		if (map != null) {
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)) {
				Vector3 diff = hit.point + offset;
				int hitx = (int) (diff.x / cell_size);
				int hity = (int) (diff.z / cell_size);
				// subtract one half of a cell's length so that the square will be centered
				if (hitx >= 0 && hitx < width && hity >= 0 && hity < height) {
					if (map[hitx, hity] != EMPTY && map[hitx, hity] != EDGE) {
						grid_position = new Pos(hitx, hity);
						hover_position = new Vector3(hitx + cell_size / 2f, 0f, hity + cell_size / 2f) - offset;
						select_square.gameObject.SetActive(true);
						select_square.position = hover_position;
					}
					else {
						select_square.gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
