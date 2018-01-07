using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

	public Transform animationContainer;

	public Transform standing;
	public Transform leftStepPrefab;
	public Transform rightStepPrefab;

	public Transform head;

	PlayerComponent player;

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

		player = GetComponent<PlayerComponent>();

		// playerComponent.onStartMoving.AddListener( startWalking );
		// playerComponent.onTargetReached.AddListener( stopWalking );
		player.onNodeReached.AddListener( UpdateForward );

	}

	void UpdateForward() {

		

	}

	Vector3 up;
	Vector3 previousForward;
	Vector3 currForward;
	float rotationSpeed = 10.0f;

	void Update(){

		if( player.isMoving ) {
			var newForward = player.nextNodeDir;

			animationContainer.rotation = Quaternion.LookRotation( newForward , transform.up );
		} else {
			previousForward = animationContainer.forward;
			currForward = animationContainer.forward;
		}


		// if( ! playerComponent.isMoving &&  head.rotation != standing.rotation){
		// if( false ) {

		// 	up = standing.up;
        //     head.rotation = standing.rotation;
        //     previousForward = standing.forward;
        //     currForward = previousForward;

		// 	return;
		// } 
		
		// TiltHead();

	}

	void TiltHead() {

        // var angle = 10.0f * Mathf.Sin(Time.time * (1 / animStepTime * 2.0f));

		// currForward = playerComponent.nextNodeDir;

		// print( playerComponent.nextNodeDir );

		/* 
        if (Vector3.Dot(playerComponent.nextNodeDir, currHeadForward) < 0.99f) {

            currHeadForward = Vector3.Slerp(
                currHeadForward,
                playerComponent.nextNodeDir,
                Time.deltaTime * headRotationSpeed
            ).normalized;

        } else {

            currHeadForward = playerComponent.nextNodeDir;
        }
		*/

        // var tiltRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // var currHeadUp = tiltRotation * transform.up;

		// head.rotation = Quaternion.LookRotation(currForward, transform.up) * tiltRotation;
		// head.up = transform.up;

        // head.rotation *= tiltRotation;//Quaternion.LookRotation(currHeadForward, currHeadUp);

        // headOriginalRotation.SetLookRotation( playerComponent.nextNodeDir );

        // var rotation = headOriginalRotation;

        // rotation.Rotate( Vector3.forward, 25.0f );

        // head.Rotate( Vector3.forward, angle );

        // standing.forward = playerComponent.nextNodeDir;
        // leftStepModel.forward = playerComponent.nextNodeDir;
        // rightStepModel.forward = playerComponent.nextNodeDir;

	}

	public void startWalking(){


		var forward = player.nextNodeDir;

		standing.forward = forward;
		leftStepModel.forward = forward;
		rightStepModel.forward = forward;

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
