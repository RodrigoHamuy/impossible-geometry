// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class RotateComponent : MonoBehaviour {

	RotateHandleComponent handle;

	public UnityEvent onRotationDone = new UnityEvent();
	public UnityEvent onRotationStart = new UnityEvent();
	public UnityEvent onCanRotateChange = new UnityEvent();

	bool isRotated = false;

	Vector3 startDir;
	Quaternion startRotation;
	bool isRotating = false;

	public bool canRotate = true;

	void Start(){
		handle = GetComponentsInChildren<RotateHandleComponent>()[0];
		handle.onMouseDown.AddListener(OnHandleMouseDown);

		var player = Object.FindObjectsOfType< PlayerComponent >()[0];

		player.onTargetReached.AddListener( () => {
			canRotate = true;
			onCanRotateChange.Invoke();
		});
		player.onStartMoving.AddListener( () => {
			canRotate = false;
			onCanRotateChange.Invoke();
		});
	}

	void Update(){

		if ( canRotate ) CheckInput();
	}

	public void Rotate(){
		isRotated = !isRotated;
		if( isRotated ){
			transform.eulerAngles = new Vector3( 0, 0, 90 );
		}else{
			transform.eulerAngles = Vector3.zero;
		}
		onRotationDone.Invoke();
	}

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

		transform.rotation = Quaternion.Slerp(transform.rotation, snapRotation, 4*Time.deltaTime);

		if( Vector3.Dot(transform.forward, targetForward) > 0.999999f ) {
			transform.rotation = snapRotation;
			isRotating = false;
			isRotationTargetSet = false;
			onRotationDone.Invoke();
			Debug.Log("done");
		}

	}
	void OnTouch(Vector3 screenTouchPosition, bool startPhase = false){

		var camera = Camera.main;

		var handleScreenPos = camera.WorldToScreenPoint( handle.transform.position );
		handleScreenPos.z = 0;

		var endDir = screenTouchPosition - handleScreenPos;
		endDir.z = 0;

		if ( startPhase ) {
			startDir = endDir;
			startRotation = transform.rotation;
			isRotating =  true;
			isRotationTargetSet = false;
			onRotationStart.Invoke();
			return;
		}

		var angle = Vector3.SignedAngle(startDir, endDir, Vector3.forward);
		var targetRotation = Quaternion.AngleAxis( - angle, transform.up);

		targetRotation = targetRotation * startRotation;

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 4*Time.deltaTime);
	}
}
