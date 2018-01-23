using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveBlock : MonoBehaviour {
	
	void Start () {

		var bounds = GetComponent<MeshFilter>().mesh.bounds;

		var top = bounds.extents.y * Vector3.up + .5f * Vector3.left;

		var right = - top;
		
	}

	void Update () {
		
	}
}
