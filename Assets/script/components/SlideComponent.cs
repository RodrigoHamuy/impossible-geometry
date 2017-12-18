// ﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class SlideComponent : MonoBehaviour {


	public UnityEvent onMoveDone = new UnityEvent();

	public float snapDistance = 1.0f;
	public float maxMove = 5.0f;
	public Vector3 dir = Vector3.up;

	float currMove = 0.0f;
	float speed = 15.0f;

	PointsContainerComponent block;

	bool _isMoving = false;

	bool isMoving {
		get { return _isMoving; }
		set {
			_isMoving = value;
			Utility.canPlayerMove = !_isMoving;
		}
	}
	bool isTouching = false;

	float startMove = 0.0f;

	Vector3 startTouchPos;
	Vector3 startPos;

	bool canSlide = true;

	// Use this for initialization
	void Start () {

		var player = Object.FindObjectsOfType< PlayerComponent >()[0];

		// Enable/disable slide movement during player movement.
		player.onTargetReached.AddListener( () => {
			canSlide = true;
		});
		player.onStartMoving.AddListener( () => {
			canSlide = false;
		});

		startPos = transform.position;

		var containerComponents = new List<PointsContainerComponent>(
			GetComponentsInChildren<PointsContainerComponent>()
		);

		containerComponents.RemoveAll( c => {
			return c.gameObject.tag != "slide";
		});

		foreach( var containerComponent in containerComponents ) {
			var container = containerComponent.pathContainer;

			SetContainerAsMovable(container);

			containerComponent.onMouseDown.AddListener( StartMove );

			onMoveDone.AddListener(() => {
				container.ResetPoints();
				SetContainerAsMovable(container);
			});

		}

	}

	void SetContainerAsMovable( PathContainer container ) {
		foreach( var point in container.points ) {
			point.canMove = true;
		}
	}

	void StartMove(){
		isTouching = true;
		startMove = currMove;
		startTouchPos = GetTouchPosition();
	}

	void Update () {

		if( !isMoving && isTouching ) CheckIfIsMoving();
		if( isMoving && isTouching ) Move();
		if( isMoving && !isTouching ) MoveToSnap();
	}

	void MoveToSnap(){

		var roundCurrMove = Mathf.Round(currMove/snapDistance) * snapDistance;

		var diff = Mathf.Abs( currMove - roundCurrMove );

		if ( diff > 0.0001f ) {
			currMove = Mathf.Lerp( currMove, roundCurrMove, Time.fixedDeltaTime*speed );
		} else {
			currMove = roundCurrMove;
			isMoving = false;
			onMoveDone.Invoke();
		}
		transform.position = startPos + dir * currMove;
	}

	void CheckIfIsMoving(){

		if( ! canSlide ) return;

		var camera = Camera.main;

		var currTouchPos = GetTouchPosition();

		if( currTouchPos == Vector3.zero ) {
			isTouching = false;
			isMoving = false;
			return;
		}

		var startWorldPos = camera.ScreenToWorldPoint( startTouchPos );
		var currWorldPos = camera.ScreenToWorldPoint( currTouchPos );

		var currDiff = currWorldPos - startWorldPos;
		isMoving = currDiff.sqrMagnitude > 0.01f;
	}

	void Move(){

		var camera = Camera.main;

		var currTouchPos = GetTouchPosition();

		if (currTouchPos == Vector3.zero ) {
			isTouching = false;
			MoveToSnap();
			return;
		}

		var startWorldPos = camera.ScreenToWorldPoint( startTouchPos );
		var currWorldPos = camera.ScreenToWorldPoint( currTouchPos );

		var currDiff = currWorldPos - startWorldPos;

		var dot = Vector3.Dot( currDiff, dir);

		float target = -1.0f;

		if ( dot > 0 && currMove < maxMove) {
			target = Mathf.Min(
				currDiff.magnitude + startMove,
				maxMove
			);
		} else if (dot < 0 && currMove > 0.0f ) {
			target = startMove - Mathf.Min(
				currDiff.magnitude,
				startMove
			);
		}

		if( target == -1.0f) return;

		currMove = Mathf.Lerp(
			currMove,
			target,
			Time.fixedDeltaTime*speed
		);

		transform.position = startPos + dir * currMove;

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
