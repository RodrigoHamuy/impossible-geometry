// using System.Linq;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour {

	public UnityEvent onTargetReached = new UnityEvent();
	public UnityEvent onStartMoving = new UnityEvent();

	public Player controller;

	public bool isMoving = false;

	public float acceleration = 2.5f;
	public float maxSpeed = 5.0f;
	float _speed = 0;

	List<PathPoint> path;
	Vector3 targetPos;
	PathPoint targetPoint;
	PathPoint prevPoint;

	void Start() {
		controller = new Player(this);
	}

	void Update(){

		if( !isMoving ) return;

		var currTargetPos = targetPos;

		Vector3 dir = (currTargetPos - transform.position).normalized;

		_speed = Mathf.Min( _speed + acceleration, maxSpeed);

		transform.position = transform.position + dir*(_speed*Time.fixedDeltaTime);

		if(
			(transform.position - targetPos).sqrMagnitude < 0.01f
		) {
			if( path.Count > 1) {
				MoveToNextPoint();
			} else{
				Debug.Log("Target reached");
				path.Clear();
				isMoving = false;
				transform.position = targetPoint.position;
				onTargetReached.Invoke();
			}
		}

	}

	public void Walk(List<PathPoint> path){
		this.path = path;
		MoveToNextPoint();
		isMoving = true;
		onStartMoving.Invoke();
	}

	void MoveToNextPoint(){
		prevPoint = path[0];
		path.RemoveAt(0);
		targetPoint = path[0];
		var camera = Camera.main;

		var dir = Vector3.ProjectOnPlane(
			targetPoint.position - prevPoint.position,
			- camera.transform.forward
		).normalized;
		for (var i = 0; i < 3; i++) {
			dir[i] = Mathf.Round(dir[i]);
		}

		if( prevPoint.position.y < targetPoint.position.y ) {
			transform.position = targetPoint.position - dir;
			targetPos = targetPoint.position;
		} else {
			transform.position = prevPoint.position;
			targetPos = prevPoint.position + dir;
		}



	}

}
