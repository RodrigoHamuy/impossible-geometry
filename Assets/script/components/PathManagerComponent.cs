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
			var b = "";
			Debug.Log("Path found: " + a);
			foreach( var point in pathFinder.path){

				// Debug.Log(point.position);
				PathFinder.setColor(point.component, new Color(1, .8f, 0));

				b+= "\n" + point.position.ToString();

			}
			Debug.Log(b);
		}

	}
}
