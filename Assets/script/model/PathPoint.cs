using System.Collections.Generic;
using UnityEngine;

public class PathPoint {

	public Vector3 position;
	public List<Vector3> normals = new List<Vector3>();
	public List<Vector3> connections = new List<Vector3>();

	public PathPoint( Vector3 pos,  Vector3 normal){

		position = pos;
		normals.Add(normal);

		Vector3[] crossOptions = {
			Vector3.up,
			Vector3.down,
			Vector3.left,
			Vector3.right,
			Vector3.forward,
			Vector3.back
		};

		for (var i = 0; i < crossOptions.Length; i++) {

			var crossOption = crossOptions[i];
			if(Vector3.Dot(crossOption, normal) == 0) {

				var connection = pos + crossOption * .5f;
				connections.Add(connection);

			}

		}

	}

}
