using System.Linq;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour {

	public UnityEvent onTargetReached = new UnityEvent();
	public UnityEvent onStartMoving = new UnityEvent();

	public bool isMoving = false;

	public float acceleration = 2.5f;
	public float maxSpeed = 5.0f;
	float _speed = 0;

	List<PathPoint> path;
	Vector3 targetPos;
	PathPoint targetPoint;
	PathPoint prevPoint;

	void Start() {
		CheckParent();
	}

	void CheckParent(){
		var screenPos = Camera.main.WorldToScreenPoint( transform.position );
		var ray = Camera.main.ScreenPointToRay( screenPos );
		var layerMask = LayerMask.GetMask( "Block" );

		var hits = Physics.RaycastAll(ray, 100.0f, layerMask);

		if ( hits.Length == 0) return;

        var currBlock = hits
            .OrderBy( 
                (hit) => (transform.position - hit.point).sqrMagnitude 
            )
            .First()
            .collider.transform;

		if(
			currBlock.parent != null &&
			(
				currBlock.parent.GetComponent<RotateComponent>() != null ||
				currBlock.parent.GetComponent<SlideComponent>() != null
			)
		) {
			transform.parent = currBlock.parent;
		}

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
				if( targetPoint.canMove ) {
					transform.parent = targetPoint.container.component.transform.parent;
				} else {
					transform.parent = null;
				}
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
