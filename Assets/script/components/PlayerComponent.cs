using System.Linq;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour {

	[HideInInspector]
	public UnityEvent onTargetReached = new UnityEvent();

	[HideInInspector]
	public UnityEvent onStartMoving = new UnityEvent();

	[HideInInspector]
	public UnityEvent onNodeReached = new UnityEvent();

	[HideInInspector]
	public UnityEvent onNodeHalfWay = new UnityEvent();

	bool _isMoving = false;

	bool _isOnStairs = false;

	public bool isMoving{
		get { return _isMoving; }
	}

	public float acceleration = 2.5f;
	public float maxSpeed = 5.0f;
	float _speed = 0;

	public float speed {
		get { return _speed; }
	}

	[HideInInspector]
	public List<PathPoint> path;

	Vector3 targetPos;
	PathPoint targetPoint;
	PathPoint prevPoint;

	[HideInInspector]
	public Vector3 nextNodeDir;

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
		
		var dir = (targetPos - transform.position).normalized;

		_speed = Mathf.Min( _speed + acceleration, maxSpeed);

		var step = dir * _speed * Time.deltaTime;
		var newPos = transform.position + step;
		var newDistance = ( targetPos - newPos ).sqrMagnitude;
		var prevDistance = ( targetPos - transform.position).sqrMagnitude;

		if( prevDistance >= 0.5f && newDistance <= 0.5f ) {
			onNodeHalfWay.Invoke();
			print("onNodeHalfWay");
		}

		transform.position = transform.position + step;

		if(
			(transform.position - targetPos).sqrMagnitude < 0.01f
		) {
			onNodeReached.Invoke();
			if( targetPoint.switchButton != null ){
				targetPoint.switchButton.Press();
			}
			if( path.Count > 1 || _isOnStairs) {
				MoveToNextPoint();
			} else{
				Debug.Log("Target reached");
				path.Clear();
				_isMoving = false;
				transform.position = targetPos;
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

		_isMoving = true;

		onStartMoving.Invoke();

	}

	bool StairsLogic() {
		if (targetPoint.stairConn == prevPoint) {
            _isOnStairs = true;
            Debug.Log("_isOnStairs");
            var axis = Utility.GetNormalAxis(Utility.CleanNormal(transform.up));


            if (targetPoint.stairPos == PathPoint.StairPos.top) {
                targetPos = prevPoint.position;
                targetPos[axis] = targetPoint.position[axis];
            } else {
                targetPos = targetPoint.position;
                targetPos[axis] = prevPoint.position[axis];
            }
            return true;
        }
		return false;
    }

	bool StairsLogicLastStep() {
		if (_isOnStairs && targetPos != targetPoint.position) {
            targetPos = targetPoint.position;
            _isOnStairs = false;
            return true;
        }
		return false;
	}

	bool StairsDiagonalLogic() {
		
		if (targetPoint.stairDiagonalConn != prevPoint) return false;

		targetPos = targetPoint.position;
		return true;
	}

	void MoveToNextPoint(){

		if( StairsLogicLastStep() ) return;

		prevPoint = path[0];
		path.RemoveAt(0);
		targetPoint = path[0];

		if ( StairsDiagonalLogic() ) return;

		if( StairsLogic() ) return;

		_isOnStairs = false;

		if( 
			targetPoint.door != null &&
			targetPoint.door.conn.point == prevPoint
		){
			targetPos = targetPoint.position;
			transform.position = targetPos;
			transform.up = targetPoint.normal;
			return;
		}
		var camera = Camera.main;

		// this dir is projected because the two points may be
		// at different depth from the camera perspective.
		nextNodeDir = Utility.getDirFromScreenView( prevPoint.position, targetPoint.position );

		// TODO: I think this is only working when Player normal is up.
		// Need to make it dynamic to support player walking on walls/roofs.

		Vector3 overlappingPos;

		if( isOverlapped( targetPoint, out overlappingPos ) ){

			var nextDir = Utility.getDirFromScreenView( targetPoint.position, overlappingPos );
			transform.position = overlappingPos - nextDir - nextNodeDir;
			targetPos = overlappingPos - nextDir;

		}else if ( isOverlapped( prevPoint, out overlappingPos ) ) {

			var nextDir = Utility.getDirFromScreenView( overlappingPos, prevPoint.position );
			transform.position = overlappingPos + nextDir;
			targetPos = overlappingPos + nextDir + nextNodeDir;

		} else if ( prevPoint.position.y < targetPoint.position.y ) {

			transform.position = targetPoint.position - nextNodeDir;
			targetPos = targetPoint.position;

		} else {

			transform.position = prevPoint.position;
			targetPos = prevPoint.position + nextNodeDir;

		}

	}

	bool isOverlapped( PathPoint point, out Vector3 overlappingPos ){

		overlappingPos = Vector3.zero;

		var neighbours = Utility.getNextPoints(
			 point, 
			 Utility.CleanNormal(transform.up)
		);

		var up = point.position + transform.up;

		neighbours.AddRange(
			Utility.getPointsAtWorldPos(
				up - Vector3.right * 0.1f,
				Utility.CleanNormal(-Vector3.right)
			)
		);
		neighbours.AddRange(
			Utility.getPointsAtWorldPos(
				up - Vector3.forward * 0.1f,
				Utility.CleanNormal(-Vector3.forward )
			)
		);


		// remove the ones that are bellow player,
		// as they won't overllap the player0
		neighbours.RemoveAll( p => {
			var dir = (p.camPosition - point.camPosition).normalized;
			return Vector3.Dot( Vector3.up, dir ) < 0;
		});

		if (neighbours.Count == 0 ) return false;

		var neighbour = neighbours.OrderBy( p => {
			return p.realCamPosition.z;
		})
		.ToList() [0];

		overlappingPos = neighbour.position;

		return neighbour.realCamPosition.z < point.realCamPosition.z;

	}

}
