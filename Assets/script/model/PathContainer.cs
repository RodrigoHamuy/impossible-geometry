// using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathContainer{

	public UnityEvent unityEvent = new UnityEvent();

	public Vector3[][] triangles;

	public List<PathPoint> points = new List<PathPoint>();

	PointsContainerComponent component;

	public void ResetPoints(){
		Debug.Log("Reset points");
		foreach( var point in points ) {
			GameObject.Destroy(point.component.gameObject);
		}
		points.Clear();
		setTriangles();
		GeneratePathPoints();
	}

	public void onRotationStart() {
		foreach( var point in points ) {
			point.rotating = true;
		}
	}

	public void setTriangles(PointsContainerComponent component) {
		this.component = component;
		setTriangles();
	}

	public void setTriangles(){

		var transform = component.transform;

		var mesh = transform.GetComponent<MeshFilter>().mesh;
		var t = mesh.triangles;
		var vertices = mesh.vertices;

		var matrix = transform.localToWorldMatrix;

		// if( transform.parent != null ) {
		// 	matrix = matrix * transform.parent.localToWorldMatrix;
		// }

		setTriangles(
			t,
			vertices,
			matrix
		);

	}

	void setTriangles(int[] triIndexes, Vector3[] vertices, Matrix4x4 matrix){

		triangles = new Vector3[ triIndexes.Length/3 ][];

		for (var i = 0; i < triIndexes.Length/3; i++) {

			var triangle = new Vector3[3];

			triangle[0] = matrix.MultiplyPoint(vertices [ triIndexes [i*3]] );
			triangle[1] = matrix.MultiplyPoint(vertices [ triIndexes [i*3+1]] );
			triangle[2] = matrix.MultiplyPoint(vertices [ triIndexes [i*3+2]] );

			for (var i2 = 0; i2<triangle.Length; i2++) {

				triangle[i2] = RoundVertex(triangle[i2]);

			}

			triangles[i] = triangle;

		}

	}

	Vector3 RoundVertex(Vector3 v){

		v.x = Mathf.Round(v.x * 2f) * 0.5f;
		v.y = Mathf.Round(v.y * 2f) * 0.5f;
		v.z = Mathf.Round(v.z * 2f) * 0.5f;

		return v;

	}

	public void GeneratePathPoints(){

		for (var i = 0; i < triangles.Length; i++) {

			GeneratePathPoints(i, false);

		}

		FixPrism();

		unityEvent.Invoke();

	}

	void FixPrism(){

		var pos = Vector3.zero;
		foreach( var point in points ) {
			if( point.isPrismSide ) {
				pos = point.position - point.normal;
				continue;
			}
		}

		if( pos == Vector3.zero ) return;

		foreach( var point in points ) {
			if( point.position == pos ) {
				point.isPrismSide = true;
				return;
			}
		}

	}

	public void GeneratePathPoints(int i, bool invokeEvent = true) {

		_AddPoint(triangles[i]);

		if( invokeEvent ) {
			unityEvent.Invoke();
		}

	}

	void _AddPoint(Vector3[] triangle){

		var a = triangle[0];
		var b = triangle[1];
		var c = triangle[2];

		var up = Vector3.Cross(
			(b-a).normalized,
			(c-b).normalized
		).normalized;

		up = PathPoint.CleanNormal(up);

		// if ( up != Vector3.up ) return;

		List<Vector3> sides = new List<Vector3>{
			(b - a),
			(c - b),
			(a - c)
		};

		List<Vector3> origins = new List<Vector3>{
			a,
			b,
			c
		};

		int maxIndex = 0;

		for (var i = 0; i < sides.Count; i++) {

			if( sides[i].sqrMagnitude > sides[ maxIndex ].sqrMagnitude ) {
				maxIndex = i;
			}

		}

		sides.RemoveAt(maxIndex);
		origins.RemoveAt(maxIndex);

		var sideA = sides[0];
		var sideB = sides[1];

		var sideALength = sideA.magnitude;
		var sideBLength = sideB.magnitude;

		var sideADir = sideA.normalized;
		var sideBDir = sideB.normalized;

		for (var x = 0; x < sideALength; x++) {

			for (var y = 0; y < sideBLength; y++) {

				var pos = origins[ maxIndex % 2 ] + sideADir * (x + .5f) + sideBDir * (y+.5f);
				_AddPoint(pos, up);

			}

		}

	}

	void _AddPoint(Vector3 pos, Vector3 up){

		var i = points.FindIndex( (PathPoint point) => {
			return point.position == pos;
		});

		if( i == -1 ) {

			var pathPoint = new PathPoint(pos, up);
			points.Add(pathPoint);

		} else {
			points[ i ].isPrismSide = false;
		}

	}

}
