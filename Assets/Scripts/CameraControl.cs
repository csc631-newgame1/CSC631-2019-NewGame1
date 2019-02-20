using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.Time;
using static UnityEngine.Mathf;

public class CameraControl : MonoBehaviour
{
    public Vector3 camera_abs_position;
	[Range(0, 5)]
	public float speed;

    // Update is called once per frame
    void Update()
    {
		transform.position = 
			new Vector3(
				 Sin(time * speed) * camera_abs_position.x,
				camera_abs_position.y,
				-Cos(time * speed) * camera_abs_position.z );
        transform.LookAt(Vector3.zero);
    }
}
