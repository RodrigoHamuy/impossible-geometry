// using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour {

	public Player controller;

	public bool isMoving = false;

	public float toVel = 2.5f;
	public float maxVel = 5.0f;
	public float maxForce = 5.0f;
	public float gain = 5f;


	Rigidbody rigidBody;
	List<PathPoint> path;

	Vector3 targetPos;
	PathPoint targetPoint;
	PathPoint prevPoint;

	void Start() {
		controller = new Player(this);
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.detectCollisions = false;
		rigidBody.freezeRotation = true;
	}

	void FixedUpdate(){

		if( !isMoving ) return;

		Vector3 dist = targetPos - transform.position;
		dist.y = 0; // ignore height differences
		// calc a target vel proportional to distance (clamped to maxVel)
		Vector3 tgtVel = Vector3.ClampMagnitude(toVel * dist, maxVel);
		// calculate the velocity error
		Vector3 error = tgtVel - rigidBody.velocity;
		// calc a force proportional to the error (clamped to maxForce)
		Vector3 force = Vector3.ClampMagnitude(gain * error, maxForce);
		rigidBody.AddForce(force);

		if(
			(rigidBody.position - targetPos).sqrMagnitude < 0.001f &&
			error.sqrMagnitude < 0.001f &&
			rigidBody.velocity.sqrMagnitude < 0.001f
		) {
			if( path.Count > 1) {
				rigidBody.position = targetPoint.position;
				rigidBody.velocity = Vector3.zero;
				MoveToNextPoint();
			} else{
				Debug.Log("Target reached");
				path.Clear();
				isMoving = false;
				rigidBody.velocity = Vector3.zero;
				rigidBody.position = targetPoint.position;
			}
		}

	}

	public void Walk(List<PathPoint> path){
		this.path = path;
		MoveToNextPoint();
		isMoving = true;
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
			rigidBody.position = targetPoint.position - dir;
			targetPos = targetPoint.position;
		} else {
			targetPos = prevPoint.position + dir;
		}



	}

}
