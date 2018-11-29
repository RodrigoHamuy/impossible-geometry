using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegs : MonoBehaviour {

	public Transform body;

	public Transform stand;

	Transform leftStep;
	Transform rightStep;


	public Transform leftStepPrefab;
	public Transform rightStepPrefab;

	float stepDuration = 0.15f;

	Transform currStep;

	bool isWalking = false;
	
	void Start () {

		currStep = stand;

		var player = GetComponent<PlayerComponent>();

		player.onStartMoving.AddListener( Walk );

		player.onTargetReached.AddListener( Stand );

		var cam = Camera.main.transform;

		var hidePos = cam.position - cam.forward * 100.0f;

		leftStep = Instantiate( leftStepPrefab, hidePos, body.rotation);
		rightStep = Instantiate( rightStepPrefab, hidePos, body.rotation);
	}

	void Walk() {

		isWalking = true;
		StartCoroutine( SwitchStep() );

	}

	IEnumerator SwitchStep(){

		yield return new WaitForSeconds( stepDuration  );

		if( isWalking ) {

			HideStep();

			NextStep();

			UpdateStepPosition();

			StartCoroutine( SwitchStep() );

		}

	}

	void Stand() {

		isWalking = false;

		HideStep();

		currStep = stand;
		
		UpdateStepPosition();

	}

	void UpdateStepPosition(){

		UpdateStepPosition( currStep );

	}

	void UpdateStepPosition( Transform step ) {
		step.position = transform.position;
		step.rotation = body.rotation;
		step.parent = body;
	}

	void HideStep() {
		var cam = Camera.main.transform;

		var hidePos = cam.position - cam.forward * 100.0f;

		currStep.position = hidePos;
		currStep.parent = null;
	}

	void NextStep() {

		if( currStep == stand || currStep == rightStep) {
			currStep = leftStep;
		} else if ( currStep == leftStep ) {
			currStep = rightStep;
		}
	}
}
