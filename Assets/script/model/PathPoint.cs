using System.Collections.Generic;
using UnityEngine;

public class PathPoint {

	public Vector3 position;
	public List<Vector3> normals = new List<Vector3>();
	public List<Vector3> connections = new List<Vector3>();

	public PathPoint( Vector3 pos,  Vector3 normal){

		position = pos;
		normals.Add(normal);

	}

}
