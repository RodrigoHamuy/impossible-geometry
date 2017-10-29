using UnityEngine;

public class PathManagerComponent : MonoBehaviour {

	PathFinder pathFinder = new PathFinder();

	void Update () {

		if (
			(Input.touchCount > 0) &&
			(Input.GetTouch(0).phase == TouchPhase.Began)
		) {

			var touch = Input.GetTouch(0);
			pathFinder.OnTap(touch.position);

		} else if ( Input.GetMouseButton(0) ) {

			pathFinder.OnTap( Input.mousePosition );

		}

	}
}
