using UnityEngine;

public class PointsContainerComponent : MonoBehaviour {

	public Transform pointPrefab;
	public Transform vertexPrefab;

	PathContainer pathContainer = new PathContainer();

	void Start () {

		pathContainer.unityEvent.AddListener(AddPointGameObject);

		pathContainer.setTriangles(transform);

		pathContainer.GeneratePathPoints();

	}

	void AddPointGameObject(){

		pathContainer.points.ForEach(delegate( PathPoint point ){

			var pointComponent = Instantiate(
				pointPrefab,
				point.position,
				Quaternion.identity
			).GetComponent<PathPointComponent>();

			pointComponent.point = point;

		});

	}
}
