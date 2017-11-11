using System.Collections.Generic;
using UnityEngine;

public class PathPoint {

	static public Dictionary<Vector3, string> layer = new Dictionary<Vector3, string>(){
		{
		  Vector3.up,
	    "Debug.up"
		}, {
		  Vector3.down,
			"Debug.down"
		}, {
		  Vector3.left,
		  "Debug.left"
		}, {
		  Vector3.right,
		  "Debug.right"
		}, {
		  Vector3.forward,
		  "Debug.forward"
		}, {
		  Vector3.back,
		  "Debug.back"
		},
	};

	public enum State{ New, Open, Closed };

	public Vector3 position;
	public Vector3 normal;

	public State state;
	public PathPoint prev;
	public PathPoint target;

	public PathPointComponent component;
	public PathContainer container;

	public bool isPrismSide = true;

	public float estimatedDistanceToTarget;
	public float distanceFromStart = 0;

	public List<Vector3> connections = new List<Vector3>();

	public Vector3 camPosition;
	public Vector3 realCamPosition;
	public Vector3 screenPosition;

	public bool rotating = false;
	public bool canMove = false;

	public PathPoint( Vector3 pos,  Vector3 normal, PathContainer container){

		this.container = container;

		position = pos;
		this.normal = normal;

		realCamPosition = Camera.main.WorldToScreenPoint(pos);
		screenPosition = realCamPosition;
		screenPosition.z = 0;

		// We actually get a world position, but where all the objects are at
		// the same distance from the Camera.
		camPosition = Camera.main.ScreenToWorldPoint(screenPosition);

		for (var i = 0; i < 3; i++) {
			camPosition[i] = Mathf.Round( camPosition[i] * 10f) * .5f;
		}


		Vector3[] crossOptions = {
			Vector3.up,
			Vector3.down,
			Vector3.left,
			Vector3.right,
			Vector3.forward,
			Vector3.back
		};

		for (var i = 0; i < crossOptions.Length; i++) {

			var crossOption = crossOptions[i];
			if(Vector3.Dot(crossOption, normal) == 0) {

				var connection = pos + crossOption * .5f;
				connections.Add(connection);

			}

		}
	}

	public void SetComponent( PathPointComponent c ){
		component = c;
		component.point = this;
		var quad = component.transform.GetChild(0);

		var layerName = PathPoint.layer[ normal ];

		quad.gameObject.layer = LayerMask.NameToLayer( layerName );

		component.gameObject.name = layerName;

	}

	public void Reset(PathPoint target){

		state = State.New;
		prev = null;
		this.target = target;
		estimatedDistanceToTarget = Mathf.Round( (target.camPosition - camPosition).sqrMagnitude );
		distanceFromStart = 0;

	}

	public void setPrev(PathPoint prev){
		this.prev = prev;
		distanceFromStart = Mathf.Round(
			prev.distanceFromStart + (prev.camPosition - camPosition).sqrMagnitude
		);
	}

	public bool isCloser(PathPoint point){

		var newDistanceFromStart = Mathf.Round(
			point.distanceFromStart + (point.camPosition - camPosition).sqrMagnitude
		);
		return newDistanceFromStart < distanceFromStart;

	}

	public float estimatedCost{
		get{
			return Mathf.Round( estimatedDistanceToTarget + distanceFromStart );
		}
	}

	static public Vector3 CleanNormal(Vector3 n){
		var vectors = new Vector3[]{
			Vector3.up,
			Vector3.down,
			Vector3.left,
			Vector3.right,
			Vector3.forward,
			Vector3.back
		};

		foreach( var vector in vectors ){
			if ( Vector3.Dot( vector, n ) > .9f ){
				return vector;
			}
		}

		Debug.LogError("This is not a perpendicular normal.");

		return n;

	}

}
