using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	public Transform standing;
	public Transform leftStepPrefab;
	public Transform rightStepPrefab;

	public Transform head;

	PlayerComponent playerComponent;

	Transform leftStepModel;
	Transform rightStepModel;

	float animStepTime = 0.1f;
	IEnumerator coroutine;

	int currStep = 0;

	void Start(){


		var cam = Camera.main;

		var hidePos = cam.transform.position - cam.transform.forward*9999.0f;

		leftStepModel = Instantiate(
			leftStepPrefab,
			hidePos,
			Quaternion.identity,
			transform
		);

		rightStepModel = Instantiate(
			rightStepPrefab,
			hidePos,
			Quaternion.identity,
			transform
		);

		playerComponent = GetComponent<PlayerComponent>();

		playerComponent.onStartMoving.AddListener( startWalking );
		playerComponent.onTargetReached.AddListener( stopWalking );

	}

	Vector3 headUp;
	Vector3 previousForward;
	Vector3 currHeadForward;
	float headRotationSpeed = 10.0f;

	void Update(){

		if( playerComponent.isMoving ){

			var angle = 10.0f * Mathf.Sin( Time.time * ( 1 / animStepTime*2.0f ) );

			if ( Vector3.Dot(playerComponent.nextNodeDir, currHeadForward) < 0.99f ) {
				currHeadForward = Vector3.Slerp(
					currHeadForward,
					playerComponent.nextNodeDir,
					Time.deltaTime * headRotationSpeed
				).normalized;
			} else {
				currHeadForward = playerComponent.nextNodeDir;
			}

			var upRotation = Quaternion.AngleAxis( angle, currHeadForward );

			var currHeadUp = upRotation * headUp;

			head.rotation = Quaternion
				.LookRotation( currHeadForward, currHeadUp );

			// headOriginalRotation.SetLookRotation( playerComponent.nextNodeDir );

			// var rotation = headOriginalRotation;

			// rotation.Rotate( Vector3.forward, 25.0f );

			// head.Rotate( Vector3.forward, angle );

			standing.forward = playerComponent.nextNodeDir;
			leftStepModel.forward = playerComponent.nextNodeDir;
			rightStepModel.forward = playerComponent.nextNodeDir;

		} else {
			headUp = standing.up;
			head.rotation = standing.rotation;
			previousForward = standing.forward;
			currHeadForward = previousForward;
		}

	}

	public void startWalking(){

		standing.forward = playerComponent.nextNodeDir;
		leftStepModel.forward = playerComponent.nextNodeDir;
		rightStepModel.forward = playerComponent.nextNodeDir;

		switchAnimStepModel();

		coroutine = walk();
    StartCoroutine(coroutine);

	}

	public void stopWalking(){

		var cam = Camera.main;
		var hidePos = cam.transform.position - cam.transform.forward*9999.0f;
		standing.position = transform.position;
		leftStepModel.position = hidePos;
		rightStepModel.position = hidePos;

		StopCoroutine(coroutine);
	}

	IEnumerator  walk(){
		yield return new WaitForSeconds( animStepTime );
		switchAnimStepModel();
		coroutine = walk();
		StartCoroutine(coroutine);

	}

	void switchAnimStepModel(){

		var cam = Camera.main;
		var hidePos = cam.transform.position - cam.transform.forward*9999.0f;

		standing.position = hidePos;
		leftStepModel.position = hidePos;
		rightStepModel.position = hidePos;

		currStep = ( currStep + 1 ) % 2;

		standing.position = hidePos;
		leftStepModel.position = hidePos;
		rightStepModel.position = hidePos;

		switch (currStep){
			// case 0:
			// 	standing.position = transform.position;
			// 	standing.rotation = transform.rotation;
			// break;
			case 0:
				leftStepModel.position = transform.position;
				break;
			case 1:
				rightStepModel.position = transform.position;
			break;
		}
	}

}
