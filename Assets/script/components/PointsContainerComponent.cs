using System.Collections.Generic;
using UnityEngine;

public class PointsContainerComponent : MonoBehaviour {

	public Transform connectorPrefab;
	public Transform pointPrefab;
	public Transform vertexPrefab;

	Transform[] vertices = new Transform[3];
	List<PathPointComponent> connectors = new List<PathPointComponent>();

	PathContainer pathContainer = new PathContainer();

	int i = 0;

	void Start () {

		pathContainer.unityEvent.AddListener(UpdatePointsGameObject);

		pathContainer.setTriangles(transform);

		// pathContainer.GeneratePathPoints();

	}

	public void AddPoint(){

		if( i> 0){
			for (var i2 = 0; i2<vertices.Length; i2++) {
				Destroy(vertices[i2].gameObject);
			}
		}

		var triangle = pathContainer.triangles[i];
		vertices[0] = Instantiate(
			vertexPrefab,
			triangle[0],
			Quaternion.identity
		);
		vertices[1] = Instantiate(
			vertexPrefab,
			triangle[1],
			Quaternion.identity
		);
		vertices[2] = Instantiate(
			vertexPrefab,
			triangle[2],
			Quaternion.identity
		);
		pathContainer.GeneratePathPoints(i);
		i++;
	}

	void UpdatePointsGameObject(){

		connectors.ForEach( (PathPointComponent connector) => {
			Destroy(connector.gameObject);
		});

		connectors.Clear();

		var points = pathContainer.points;

		points.ForEach( ( PathPoint point ) => {

			Vector3 forward = Vector3.zero;

			if( point.normals[0] == Vector3.forward) {
				forward = Vector3.up;
			} else  if( point.normals[0] == Vector3.back) {
				forward = Vector3.down;
			} else  if( point.normals[0] == Vector3.up) {
				forward = Vector3.left;
			} else  if( point.normals[0] == Vector3.down) {
				forward = Vector3.right;
			} else  if( point.normals[0] == Vector3.left) {
				forward = Vector3.forward;
			} else  if( point.normals[0] == Vector3.right) {
				forward = Vector3.back;
			}

			var pointComponent = Instantiate(
				pointPrefab,
				point.position,
				Quaternion.LookRotation( forward, point.normals[0] )
			).GetComponent<PathPointComponent>();

			point.connections.ForEach( (Vector3 connPos) => {
				Instantiate(
					connectorPrefab,
					connPos,
					Quaternion.LookRotation( forward, point.normals[0] )
				);
			});

			pointComponent.point = point;
			connectors.Add(pointComponent);

		});

	}
}
