// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class RotateComponent : MonoBehaviour {

	RotateHandleComponent handle;

	public UnityEvent onRotationDone = new UnityEvent();
	public UnityEvent onRotationStart = new UnityEvent();
	public UnityEvent onCanRotateChange = new UnityEvent();

	Vector3 startDir;
	bool _isRotating = false;

	bool isRotating {
		get{ return _isRotating; }
		set{
			_isRotating = value;
			Utility.canPlayerMove = ! _isRotating;
		}
	}

	public bool canRotate = true;

	void Start(){

		// Set points as rotatable
		var containerComponents = GetComponentsInChildren<PointsContainerComponent>();
		foreach( var containerComponent in containerComponents ){
			var container = containerComponent.pathContainer;

			setPointsAsRotatable(container);

			container.onGeneratePathPointsDone.AddListener( () => {
				setPointsAsRotatable(container);
			});

			// Update points before/after rotation
			onRotationDone.AddListener(container.ResetPoints);
			onRotationStart.AddListener(container.onRotationStart);
		}

		handle = GetComponentsInChildren<RotateHandleComponent>()[0];
		handle.onMouseDown.AddListener(OnHandleMouseDown);

		var player = Object.FindObjectOfType< PlayerComponent >();

        if (player == null) return;

		// Enable/disable rotation during player movement.
		player.onTargetReached.AddListener( () => {
			canRotate = true;
			onCanRotateChange.Invoke();
		});
		player.onStartMoving.AddListener( () => {
			canRotate = false;
			onCanRotateChange.Invoke();
		});
	}

	void setPointsAsRotatable( PathContainer container ){
		foreach( var point in container.points ) {
			point.canMove = true;
		}
	}

	void Update(){

		if ( canRotate ) CheckInput();
	}

	// public void Rotate(){
	// 	isRotated = !isRotated;
	// 	if( isRotated ){
	// 		transform.eulerAngles = new Vector3( 0, 0, 90 );
	// 	}else{
	// 		transform.eulerAngles = Vector3.zero;
	// 	}
	// 	onRotationDone.Invoke();
	// }

	bool touchStart = false;
	bool mouseTouchStart = false;

	void OnHandleMouseDown(){
		touchStart = true;
	}


	void CheckInput(){

		if( touchStart ) {
			if (Input.touchCount == 1) {

				var touch = Input.GetTouch (0);

				OnTouch (
				touch.position,
				touch.phase == TouchPhase.Began
				);

			} else if (Input.GetMouseButton (0)) {

				OnTouch (
				Input.mousePosition,
				!mouseTouchStart
				);
				mouseTouchStart = true;
			} else {

				touchStart = false;
				mouseTouchStart = false;
			}
		} else {

			if( isRotating ) SnapRotation();

		}

	}

	Quaternion snapRotation;
	Vector3 targetForward;
	bool isRotationTargetSet = false;
	float rotationSpeed = 12;
	void SnapRotation(){

		if( ! isRotationTargetSet ) {
			var euler = transform.eulerAngles;

			for (var i = 0; i < 3; i++ ) {
				euler[i] = Mathf.Round(euler[i] / 90.0f ) * 90.0f;
			}
			snapRotation = Quaternion.identity;
			snapRotation.eulerAngles = euler;
			targetForward = snapRotation * Vector3.forward;
			isRotationTargetSet = true;
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, snapRotation, 0.5f*rotationSpeed*Time.deltaTime);

		if( Vector3.Dot(transform.forward, targetForward) > 0.999999f ) {
			transform.rotation = snapRotation;
			isRotating = false;
			isRotationTargetSet = false;
			onRotationDone.Invoke();
			Debug.Log("done");
		}

	}

    float currAngle = 0;

    void OnTouch(Vector3 screenTouchPosition, bool startPhase = false){

		var camera = Camera.main;

		var handleScreenPos = camera.WorldToScreenPoint( handle.transform.position );
		handleScreenPos.z = 0;

		var endDir = screenTouchPosition - handleScreenPos;
		endDir.z = 0;

		if ( startPhase ) {
            currAngle = 0;
            startDir = endDir;
			isRotating =  true;
			isRotationTargetSet = false;
			onRotationStart.Invoke();
			return;
		}

        var angle = Utility.SignedAngle(startDir, endDir, Vector3.forward);

        var newAngle = Mathf.LerpAngle(currAngle, -angle, rotationSpeed * Time.deltaTime);

        transform.Rotate(handle.transform.up, newAngle - currAngle, Space.World);
        currAngle = newAngle;
    }

		Vector3 GetTouchPosition(){
			if (Input.touchCount == 1) {
				var touch = Input.GetTouch (0);
				return touch.position;
			} else if (Input.GetMouseButton (0)) {
				return Input.mousePosition;
			}
			return Vector3.zero;
		}
}
