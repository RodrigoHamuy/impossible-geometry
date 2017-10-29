using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathContainer{

	public UnityEvent unityEvent = new UnityEvent();

	Vector3[][] triangles;

	public List<PathPoint> points = new List<PathPoint>();

	public void setTriangles(Transform gameObject){

		var mesh = gameObject.GetComponent<MeshFilter>().mesh;
		var t = mesh.triangles;
		var vertices = mesh.vertices;

		setTriangles(t, vertices, gameObject.localToWorldMatrix );

	}

	void setTriangles(int[] t, Vector3[] vertices, Matrix4x4 matrix){

		triangles = new Vector3[ t.Length/3 ][];

		for (var i = 0; i < t.Length/3; i++) {

			triangles[i] = new Vector3[4];
			triangles[i][0] = matrix.MultiplyVector( vertices [t[i*3]] );
			triangles[i][1] = matrix.MultiplyVector( vertices [t[i*3+1]] );
			triangles[i][2] = matrix.MultiplyVector( vertices [t[i*3+2]] );

			var a = triangles[i][0];
			var b = triangles[i][1];
			var c = triangles[i][2];

			var sideAB = (a - b).normalized;
			var sideAC = (a - c).normalized;
			var sideBC = (b - c).normalized;

			Vector3 cross;

			if (Vector3.Dot(sideAB, sideAC) == 0) {

				cross = Vector3.Cross(sideAB, sideAC);

			}else if (Vector3.Dot(sideAB, sideBC) == 0){

				cross = Vector3.Cross(sideAB, sideBC);

			} else { // if (Vector3.Dot(sideAC, sideBC) == 0){

				cross = Vector3.Cross(sideAC, sideBC);

			}

			triangles[i][3] = cross;

		}

	}

	public void GeneratePathPoints(){

		for (var i = 0; i < triangles.Length; i++) {

			var triangle = triangles[i];
				_AddPoint(triangle[0], triangle[1], triangle[2], triangle[3]);
				_AddPoint(triangle[0], triangle[2], triangle[1], triangle[3]);
				_AddPoint(triangle[1], triangle[2], triangle[0], triangle[3]);

		}

		unityEvent.Invoke();

	}

	void _AddPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 up){

		Vector3 pos;

		if(
			(
				a.x == b.x &&
				a.y == b.y
			) || (
				a.z == b.z &&
				a.y == b.y
			) || (
				a.x == b.x &&
				a.z == b.z
			)
		){
			var sideAB = a - b;
			var distance = sideAB.magnitude;
			var dir = sideAB.normalized;

			for( var i = 0; i < distance; i++ ) {

				pos = a - dir * (i + .5f);
				_AddPoint(pos, up);

			}

		} else {
			var sideAC = a - c;
			var sideBC = b - c;
			var dirAC = sideAC.normalized;
			var dirBC = sideBC.normalized;
			var distanceAC = sideAC.magnitude;
			var distanceBC = sideBC.magnitude;

			for( var i = 0; i < distanceAC; i++ ) {

				pos = c + dirBC * .5f + dirAC * i;
				_AddPoint(pos, up);

			}
		}

	}

	void _AddPoint(Vector3 pos, Vector3 up){

		var index = points.FindIndex( (PathPoint point) => {
			return point.position == pos;
		});

		if( index > -1 ) {

			var normals = points[index].normals;

			var normalExists = normals.Exists( (Vector3 normal) => {
				return normal == up;
			});

			if( ! normalExists ) {
				normals.Add(up);
			}


		} else {

			var pathPoint = new PathPoint(pos, up);
			points.Add(pathPoint);

		}

	}

}