using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder {

	PathPoint target;
	PathPoint start;

	// List<PathPoint> openPoints;

	public bool MovePlayerTo(Vector3 player, Vector3 tapPos){

		var newTarget = getPointAtScreenPos(tapPos);

		if(
			newTarget == null || target == newTarget
		) return false;

		target = newTarget;

		ResetAll();

		start = getPointAtWorldPos(player);

		return Search(start, target);
	}

	void ResetAll(){

		// openPoints.Clear();

		var allPointComponents = Object.FindObjectsOfType<PathPointComponent>();

		foreach(var pointComponent in allPointComponents) {
			pointComponent.point.Reset(target);
			setColor(pointComponent, new Color(0, 0, 1) );
		}

	}

	PathPoint getPointAtScreenPos(Vector3 pos){
		var ray = Camera.main.ScreenPointToRay(pos);
		return getPointAtRay(ray);
	}

	PathPoint getPointAtWorldPos(Vector3 pos){

		var dir = pos - Camera.main.transform.position;
		var ray = new Ray {
			direction = dir,
			origin = Camera.main.transform.position
		};
		return getPointAtRay(ray);
	}

	PathPoint getPointAtRay(Ray ray){

		var layerMask = LayerMask.GetMask("Debug.Point");

		var hits = Physics.RaycastAll(ray, 100.0f, layerMask);

		foreach( var hit in hits ) {

			var point = hit.collider.transform.parent.GetComponent<PathPointComponent>().point;

			// TODO: Replace Vector3.up with dynamic up

			if( point.normal == Vector3.up ){
				return point;
			}

		}

		return null;
	}

	List<PathPoint> findNextPoints(PathPoint point){

		List<PathPoint> nextPoints = new List<PathPoint>();

		Vector3[] directions = {
			point.component.transform.transform.forward,
			point.component.transform.transform.right,
			- point.component.transform.transform.forward,
			- point.component.transform.transform.right
		};

		foreach( var dir in directions){

			var pos = point.position + dir;
			var nextPoint = getPointAtWorldPos( pos );

			if( nextPoint == null || nextPoint == point.prev)
			continue;

			if( nextPoint.state == PathPoint.State.Open ){
				if( ! nextPoint.isCloser(point) ) {
					continue;
				}
			} else {
				nextPoint.state = PathPoint.State.Open;
			}
			nextPoint.setPrev(point);

			nextPoint.state = PathPoint.State.Open;

			nextPoints.Add(nextPoint);
			setColor(nextPoint.component, new Color(1.0f, .2f, 0) );

		}

		nextPoints = nextPoints.OrderBy( (PathPoint nextPoint) => {
			return nextPoint.estimatedCost;
		}).ToList();

		if( nextPoints.Count > 0 ) {
			setColor(nextPoints[0].component, new Color(1, .8f, 0) );
		}


		return nextPoints;
	}

	void setColor(PathPointComponent point, Color color){

		var rend = point.GetComponentsInChildren<Renderer>()[0];
		rend.material.color = color;
	}

	bool Search(PathPoint current, PathPoint target){

		if( current == target) return true;

		current.target = target;
		current.state = PathPoint.State.Closed;

		List<PathPoint> nexts = findNextPoints(current);

		setColor(current.component, new Color(.6f, .13f, .86f) );
		setColor(target.component, new Color(.2f, .8f, 0) );

		foreach( var next in nexts ) {
			if( Search(next, target) ){
				return true;
			}
		}

		return false;
	}


}
