using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private Vector3 start, end;
	private bool initialized = false;
	private MeshRenderer projectileRender;
	
	public bool reachedDestination = false;
	
    public void Init(Vector3 start, Vector3 end) { 
		this.start = start;
		this.end = end;
		initialized = true;
		transform.LookAt(end);
		projectileRender = GetComponent<MeshRenderer>();
	}
	
	private float progress = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
		else if (progress >= 1f) {
			reachedDestination = true;
			initialized = false;
			projectileRender.enabled = false;
			Destroy(this.gameObject, 3.0f); // applies slight delay to destruction so that reachedDestination variable can be read
		}
		
		transform.position = Vector3.Lerp(start, end, progress) + Vector3.up;
		progress += 0.05f;
    }
}
