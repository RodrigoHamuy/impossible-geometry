// using System.Collections;
using UnityEngine;

public class PathManagerComponent : MonoBehaviour {

	PlayerComponent player;

	PathFinder pathFinder = new PathFinder();

	void Start() {

		player = Object.FindObjectsOfType< PlayerComponent >()[0];

		// var allBlocks = Object.FindObjectsOfType<PointsContainerComponent>();
		// foreach( var block in allBlocks ) {
		// 	block.onMouseUp.AddListener(OnBlockMouseUp);
		// }
	}

	void Update(){
		CheckInput();
	}
	void CheckInput(){
		if (player.isMoving) return;
        if (!Utility.canPlayerMove) return;

        Vector2 tapPos = Utility.getTouchEnd();

        if (tapPos == Vector2.zero) return;

		OnBlockMouseUp( tapPos );
	}
	void OnBlockMouseUp ( Vector2 tapPos ) {		

		var a = pathFinder.MovePlayerTo(
			player.transform.position,
			tapPos,
			Utility.CleanNormal( player.transform.up )
		);

		if( a &&  pathFinder.path != null && pathFinder.path.Count > 0 ) {
			player.Walk( pathFinder.path );
			Debug.Log("Path found: " + pathFinder.path.Count + " steps.");
			foreach( var point in pathFinder.path){
				Utility.SetPointColor(point.component, new Color(1, .8f, 0));
			}
		} else {
			Debug.Log("Path not found");
		}
	}
}
