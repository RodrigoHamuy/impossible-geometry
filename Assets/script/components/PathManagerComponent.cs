// using System.Collections;
using UnityEngine;

public class PathManagerComponent : MonoBehaviour {

	public PlayerComponent player;

	PathFinder pathFinder = new PathFinder();

	void Start() {
		var allBlocks = Object.FindObjectsOfType<PointsContainerComponent>();
		foreach( var block in allBlocks ) {
			block.onMouseDown.AddListener(OnBlockMouseDown);
		}
	}
	void OnBlockMouseDown () {

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

		if( player.isMoving ) return;
		var a = pathFinder.MovePlayerTo(
			player.transform.position,
			tapPos,
			PathPoint.CleanNormal( player.transform.up )
		);

		if( a ) {
			player.Walk( pathFinder.path );
			Debug.Log("Path found: " + pathFinder.path.Count + " steps.");
			foreach( var point in pathFinder.path){
				PathFinder.setColor(point.component, new Color(1, .8f, 0));
			}
		} else {
			Debug.Log("Path not found");
		}
	}
}
