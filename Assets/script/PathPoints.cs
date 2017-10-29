using UnityEngine;

public class PathPoints : MonoBehaviour {

	public Transform pointPrefab;
	public Transform vertexPrefab;
	public Transform upPrefab;

	Transform[] verticesGO = new Transform[3];

	Vector3[][] triangles;

	int index = 0;

	void Start () {

		Mesh mesh;
		mesh = GetComponent<MeshFilter>().mesh;
		var t = mesh.triangles;
		var vertices = mesh.vertices;
		triangles = new Vector3[ t.Length/3 ][];

		for (var i = 0; i < t.Length/3; i++) {

			triangles[i] = new Vector3[4];
			triangles[i][0] = transform.localToWorldMatrix.MultiplyVector( vertices [t[i*3]] );
			triangles[i][1] = transform.localToWorldMatrix.MultiplyVector( vertices [t[i*3+1]] );
			triangles[i][2] = transform.localToWorldMatrix.MultiplyVector( vertices [t[i*3+2]] );

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

		for (var i = 0; i < triangles.Length; i++) {

			var triangle = triangles[i];
				_AddPoint(triangle[0], triangle[1], triangle[2], triangle[3]);
				_AddPoint(triangle[0], triangle[2], triangle[1], triangle[3]);
				_AddPoint(triangle[1], triangle[2], triangle[0], triangle[3]);

		}

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
				// point
				Instantiate(pointPrefab, pos, Quaternion.identity);
				// normal
				Instantiate(upPrefab, pos + up*0.15f, Quaternion.identity);
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
				// point
				Instantiate(pointPrefab, pos, Quaternion.identity);
				// normal
				Instantiate(upPrefab, pos + up*0.15f, Quaternion.identity);
			}
		}

	}

	public void AddPoint(){

		var i = index;
		var triangle = triangles[i];

		var dot = Vector3.Dot( triangle[0], triangle[1] );
		dot = Vector3.Dot( triangle[0], triangle[2] );
		dot = Vector3.Dot( triangle[1], triangle[2] );

		if( i>0){
			Destroy(verticesGO[0].gameObject);
			Destroy(verticesGO[1].gameObject);
			Destroy(verticesGO[2].gameObject);
		}

		verticesGO[0] = Instantiate(vertexPrefab, triangle[0], Quaternion.identity);
		verticesGO[1] = Instantiate(vertexPrefab, triangle[1], Quaternion.identity);
		verticesGO[2] = Instantiate(vertexPrefab, triangle[2], Quaternion.identity);

		_AddPoint(triangle[0], triangle[1], triangle[2], triangle[3]);
		_AddPoint(triangle[0], triangle[2], triangle[1], triangle[3]);
		_AddPoint(triangle[1], triangle[2], triangle[0], triangle[3]);

		index++;

	}

	void Update () {

	}
}
