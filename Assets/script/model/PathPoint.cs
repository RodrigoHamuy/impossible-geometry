using System.Collections.Generic;
using UnityEngine;

public class PathPoint {

	public enum State{ New, Open, Closed };

	public Vector3 position;
	public Vector3 normal;

	public State state;
	public PathPoint prev;
	public PathPoint target;

	public PathPointComponent component;

	public float estimatedDistanceToTarget;
	public float distanceFromStart = 0;

	public List<Vector3> connections = new List<Vector3>();

	public Vector3 camPosition;

	public PathPoint( Vector3 pos,  Vector3 normal){

		position = pos;
		this.normal = normal;

		camPosition = Camera.main.WorldToScreenPoint(pos);

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

}
