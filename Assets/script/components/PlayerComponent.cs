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

    bool _isOnTwistedBlock = false;

    bool _isOnArchBlock = false;

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
	
	PathPoint _targetPoint;

	public PathPoint targetPoint{
		get{ return _targetPoint; }
	}

	PathPoint _prevPoint;

	public PathPoint prevPoint{
		get{ return _prevPoint; }
	}

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

	void UpdateNormal() {
		var totalDist = (targetPos - prevPoint.position).sqrMagnitude;

		var currDist = (targetPos - transform.position).sqrMagnitude;

		if( currDist < 0.01f ) currDist = 0.0f;

		transform.up = Vector3.Slerp( prevPoint.normal, targetPoint.normal, (totalDist-currDist)/totalDist );
	}

	void UpdateArchPos() {


        var dir = (targetPos - transform.position).normalized;

        _speed = Mathf.Min(_speed + acceleration, maxSpeed);

        var step = dir * _speed * Time.deltaTime;
        var newPos = transform.position + step;

		// var angle = Vector3.Angle();\

		var middle = targetPos - targetPoint.normal + prevPoint.normal * 0.5f;

        transform.position = transform.position + step;

		

	}

	void UpdateStep() {


        var dir = (targetPos - transform.position).normalized;

        _speed = Mathf.Min(_speed + acceleration, maxSpeed);

        var step = dir * _speed * Time.deltaTime;
        transform.position = transform.position + step;

	}

	void Update(){

		if( !isMoving ) return;		

		var prevDistance = ( targetPos - transform.position).sqrMagnitude;

		if( _isOnArchBlock ) {

			UpdateArchPos();

		} else {

			UpdateStep();

		}

		var newDistance = (targetPos - transform.position).sqrMagnitude;
		
		if( prevDistance >= 0.5f && newDistance <= 0.5f ) {
			onNodeHalfWay.Invoke();
			print("onNodeHalfWay");
		}

		if( _isOnTwistedBlock ) {
			UpdateNormal();
		}

		if(
			(transform.position - targetPos).sqrMagnitude < 0.01f
		) {
			onNodeReached.Invoke();
			if( _targetPoint.switchButton != null ){
				_targetPoint.switchButton.Press();
			}
			if( path.Count > 1 || _isOnStairs) {
				MoveToNextPoint();
			} else{
				Debug.Log("Target reached");
				path.Clear();
				_isMoving = false;
				transform.position = targetPos;
				if( _targetPoint.canMove ) {
					transform.parent = _targetPoint.container.component.transform.parent;
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
		if (_targetPoint.stairConn == _prevPoint) {
            _isOnStairs = true;
            Debug.Log("_isOnStairs");
            var axis = Utility.GetNormalAxis(Utility.CleanNormal(transform.up));


            if (_targetPoint.stairPos == PathPoint.StairPos.top) {
                targetPos = _prevPoint.position;
                targetPos[axis] = _targetPoint.position[axis];
            } else {
                targetPos = _targetPoint.position;
                targetPos[axis] = _prevPoint.position[axis];
            }
            return true;
        }
		return false;
    }

	bool StairsLogicLastStep() {
		if (_isOnStairs && targetPos != _targetPoint.position) {
            targetPos = _targetPoint.position;
            _isOnStairs = false;
            return true;
        }
		return false;
	}

	bool StairsDiagonalLogic() {
		
		if (_targetPoint.stairDiagonalConn != _prevPoint) return false;

		targetPos = _targetPoint.position;
		return true;
	}

	bool TwistedBlockLogic() {
		if( 
			_targetPoint.twistedBlockConn != null && 
			_targetPoint.twistedBlockConn == prevPoint 
		) {
			targetPos = _targetPoint.position;
			_isOnTwistedBlock = true;

			return true;
		}
		_isOnTwistedBlock = false;
		return false;

	}	

	bool ArchBlockLogic() {
		if( 
			_targetPoint.archBlockConn != null && 
			_targetPoint.archBlockConn == prevPoint 
		) {
			targetPos = _targetPoint.position;
			_isOnArchBlock = true;

			return true;
		}
		_isOnArchBlock = false;
		return false;

	}

	void MoveToNextPoint(){

		if( StairsLogicLastStep() ) return;

		_prevPoint = path[0];
		path.RemoveAt(0);
		_targetPoint = path[0];

		if ( TwistedBlockLogic() ) return;

		if ( StairsDiagonalLogic() ) return;

		if( StairsLogic() ) return;

		if( ArchBlockLogic() ) return;

		_isOnStairs = false;

		if( 
			_targetPoint.door != null &&
			_targetPoint.door.conn.point == _prevPoint
		){
			targetPos = _targetPoint.position;
			transform.position = targetPos;
			transform.up = _targetPoint.normal;
			return;
		}
		var camera = Camera.main;

		// this dir is projected because the two points may be
		// at different depth from the camera perspective.
		nextNodeDir = Utility.getDirFromScreenView( _prevPoint.position, _targetPoint.position );

		// TODO: I think this is only working when Player normal is up.
		// Need to make it dynamic to support player walking on walls/roofs.

		Vector3 overlappingPos;

		if( isOverlapped( _targetPoint, out overlappingPos ) ){

			var nextDir = Utility.getDirFromScreenView( _targetPoint.position, overlappingPos );
			transform.position = overlappingPos - nextDir - nextNodeDir;
			targetPos = overlappingPos - nextDir;

		}else if ( isOverlapped( _prevPoint, out overlappingPos ) ) {

			var nextDir = Utility.getDirFromScreenView( overlappingPos, _prevPoint.position );
			transform.position = overlappingPos + nextDir;
			targetPos = overlappingPos + nextDir + nextNodeDir;

		} else if ( _prevPoint.position.y < _targetPoint.position.y ) {

			transform.position = _targetPoint.position - nextNodeDir;
			targetPos = _targetPoint.position;

		} else {

			transform.position = _prevPoint.position;
			targetPos = _prevPoint.position + nextNodeDir;

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
