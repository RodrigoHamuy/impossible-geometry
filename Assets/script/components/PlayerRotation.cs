using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour {

	Transform body;

	PlayerComponent player;

	Vector3 prevForward;

	float currAngle;
	float targetAngle;
	
	void Start () {

		body = transform.Find( "animation" );

		player = GetComponent<PlayerComponent>();
		player.onNodeHalfWay.AddListener( UpdateForwardHalfWay );
		player.onNodeReached.AddListener( UpdateForwardOnNodeReached );
		player.onStartMoving.AddListener( UpdateForwardOnStart );
		
	}

	void Update () {

		var newAngle = Mathf.LerpAngle( currAngle, targetAngle, Time.deltaTime * player.speed);

		body.transform.Rotate( Vector3.up, newAngle-currAngle);

		currAngle = newAngle;

		prevForward = transform.forward;

	}

	void UpdateForwardHalfWay() {

		if( player.path.Count < 2) return;

		var prevNode = player.path[0];
		var nextNode = player.path[1];

		UpdateForward( prevNode, nextNode);

	}

	void UpdateForwardOnStart() {
		var prevNode = player.prevPoint;
		var nextNode = player.targetPoint;
		UpdateForward( prevNode, nextNode);
	}

	void UpdateForwardOnNodeReached() {

		if( player.path.Count > 1 ) return;
		var prevNode = player.prevPoint;
		var nextNode = player.targetPoint;
		UpdateForward( prevNode, nextNode);

	}

	void UpdateForward( PathPoint prevNode, PathPoint nextNode ) {

		Vector3 nextForward;

		if( prevNode.stairConn != null && prevNode.stairConn == nextNode ) {
			nextForward = Utility.getDirFromScreenView( prevNode.position, nextNode.position, prevNode.normal );				
		}else {
			nextForward = Utility.getDirFromScreenView( prevNode.position, nextNode.position );	
		}

		targetAngle = Vector3.SignedAngle( prevForward, nextForward, transform.up);

	}


}
