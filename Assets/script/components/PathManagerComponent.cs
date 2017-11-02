using UnityEngine;

public class PathManagerComponent : MonoBehaviour {

	public PlayerComponent player;

	PathFinder pathFinder = new PathFinder();

	void Update () {

		Vector3 tapPos;

		if (
			(Input.touchCount > 0) &&
			(Input.GetTouch(0).phase == TouchPhase.Began)
		) {

			var touch = Input.GetTouch(0);
			tapPos = touch.position;

		} else if ( Input.GetMouseButton(0) ) {

			tapPos = Input.mousePosition;

		} else {
			return;
		}

		// if( player.isMoving ) return;

		player.isMoving = true;

		var a = pathFinder.MovePlayerTo(
			player.transform.position, tapPos
		);

		if( a ) {
			Debug.Log("Path found: " + a);
		}
		
	}
}
