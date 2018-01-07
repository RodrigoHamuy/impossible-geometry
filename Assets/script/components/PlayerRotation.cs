using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour {

	Transform body;

	PlayerComponent player;

	Vector3 prevForward;
	Vector3 currForward;
	Vector3 nextForward;

	float currAngle;
	float targetAngle;
	float rotationAmount;

	bool isRotating = false;
	
	void Start () {

		body = transform.Find( "animation" );

		player = GetComponent<PlayerComponent>();
		player.onNodeHalfWay.AddListener( UpdateForward );

		prevForward = transform.forward;
		currForward = prevForward;
		// player.onStartMoving.AddListener( UpdateForwardOnStartMoving );
		
	}

	void Update () {

		if ( ! isRotating ) return;

		// var rotationStep = player.speed * Time.deltaTime;
		// rotationAmount = Mathf.Min( rotationAmount + rotationStep, 1.0f) ;
		rotationAmount = Mathf.Min( rotationAmount + Time.deltaTime, 1.0f) ;

		var newAngle = Mathf.LerpAngle( currAngle, targetAngle, rotationAmount); 

		// body.rotation = transform.rotation;

		// body.forward = prevForward;

		// var rotation = body.rotation;

		body.Rotate( body.up, newAngle - currAngle );
		currAngle = newAngle;

		if( rotationAmount == 1.0f ) isRotating = false;

	}

	// void UpdateForwardOnStartMoving() {
	// }

	void UpdateForward() {

		if( player.path.Count < 2) return;

		var nextNode = player.path[0];
		var nextNextNode = player.path[1];

		if( nextNode.stairConn != null && nextNode.stairConn == nextNextNode.stairConn ) return;

		nextForward = Utility.getDirFromScreenView( nextNode.position, nextNextNode.position );		

		targetAngle = Vector3.Angle( prevForward, nextForward);

		if( targetAngle != 0.0f ) {
			rotationAmount = 0;
			isRotating = true;
		}

	}
}
