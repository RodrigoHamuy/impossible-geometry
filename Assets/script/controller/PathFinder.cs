using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder {

	PathPoint target;
	PathPoint start;

	public List<PathPoint> path = new List<PathPoint>();

	// List<PathPoint> openPoints;

	public bool MovePlayerTo(Vector3 player, Vector3 tapPos){

		// TODO: Find out the most suitable point target. Maybe the one
		// Maybe the closest to the player level.
		// Or maybe the closest to the previous point.
		// This would require to decide the target after the path is found.

		var newTarget = getPointsAtScreenPos(tapPos)[0];

		if(
			newTarget == null || target == newTarget
		) return false;

		target = newTarget;

		ResetAll();

		// TODO: Choose the closest point to the player.

		start = getPointsAtWorldPos(player)[0];
		setColor(start.component, new Color(1, 1, 1) );

		return Search(start, target);
	}

	void ResetAll(){

		path.Clear();

		var allPointComponents = Object.FindObjectsOfType<PathPointComponent>();

		foreach(var pointComponent in allPointComponents) {
			pointComponent.point.Reset(target);
			setColor(pointComponent, new Color(0, 0, 1) );
		}
	}

	List<PathPoint> getPointsAtScreenPos(Vector3 pos){
		var ray = Camera.main.ScreenPointToRay(pos);
		return getPointsAtRay(ray);
	}

	List<PathPoint> getPointsAtWorldPos(Vector3 pos){
		var screenPos = Camera.main.WorldToScreenPoint(pos);
		return getPointsAtScreenPos(screenPos);
	}

	List<PathPoint> getPointsAtRay(Ray ray){

		List<PathPoint> points = new List<PathPoint>();

		var layerMask = LayerMask.GetMask("Debug.Point");

		var hits = Physics.RaycastAll(ray, 100.0f, layerMask);

		foreach( var hit in hits ) {

			var point = hit.collider.transform.parent.GetComponent<PathPointComponent>().point;

			// TODO: Replace Vector3.up with dynamic up

			if( point.normal == Vector3.up ){
				points.Add(point);
			}

		}

		return points;
	}

	List<PathPoint> findNextPoints(PathPoint point){

		List<PathPoint> nextPoints = new List<PathPoint>();

		// var overlappingPoints = getPointsAtWorldPos(point.position);
		//
		// overlappingPoints.RemoveAll( overlappingPoint => {
		// });

		Vector3[] directions = {
			point.component.transform.transform.forward,
			point.component.transform.transform.right,
			- point.component.transform.transform.forward,
			- point.component.transform.transform.right
		};

		foreach( var dir in directions){

			var pos = point.position + dir;
			var newNextPoints = getPointsAtWorldPos( pos );

			// Remove if it is overlapped by another point bellow the same ray.
			newNextPoints.RemoveAll( nextPoint => {

				// Remove if the point has been check already.
				if (nextPoint.state == PathPoint.State.Closed ) return true;

				// TODO: Add a parameter to tell if the object is a block or a
				// plane, so this check is skipped on planes.

				// remove if this nextPoint is above the current point (from camera
				// perspective) and his block overlaps the current point.
				if(
					nextPoint.camPosition.y > point.camPosition.y &&
					nextPoint.position.y > point.position.y
				) return true;

				// remove if this block is bellow the current point (from camera
				// perspective) and is being overlapped by the current point.
				if(
					nextPoint.camPosition.y < point.camPosition.y &&
					nextPoint.position.y < point.position.y
				) return true;

				// remove if it is the previous point
				if ( nextPoint == point.prev ) return true;

				// TODO: What is this? Please comment.
				// if( nextPoint.camPosition.y > point.camPosition.y ){
				// 	if(
				// 		newNextPoints.Exists( overlappingNextPoint =>{
				// 			return overlappingNextPoint.position.y > nextPoint.position.y;
				// 		})
				// 	) return true;
				// }

				// default return
				return false;
			});

			foreach( var nextPoint in newNextPoints ){

				// Same, but for objects that are bellow
				// if(
				// 	nextPoint.position.y < point.position.y &&
				// 	nextPoint.camPosition.y < point.camPosition.y
				// ) {
				// 	continue;
				// }

				// Check if nextPoint is next to a point that is on top of point.
				// if(
				// 	overlappingPoints.Exists( (overlappingPoint) => {
				// 		return overlappingPoint.position.y == nextPoint.position.y;
				// 	})
				// ){
				// 	continue;
				// }

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
				// setColor(nextPoint.component, new Color(1.0f, .2f, 0) );

			}

		}

		nextPoints = nextPoints
		.OrderBy( (PathPoint nextPoint) => {
			return nextPoint.estimatedCost;
		})
		.ThenBy( (PathPoint nextPoint) =>{
			return ( point.position - nextPoint.position ).sqrMagnitude;
		})
		.ToList();

		if( nextPoints.Count > 0 ) {
			// setColor(nextPoints[0].component, new Color(1, .8f, 0) );
		}


		return nextPoints;
	}

	public static void setColor(PathPointComponent point, Color color){

		var rend = point.GetComponentsInChildren<Renderer>()[0];
		rend.material.color = color;
	}

	bool Search(PathPoint current, PathPoint target){

		if( current == target) {
			path.Add(target);
			return true;
		}

		current.target = target;
		current.state = PathPoint.State.Closed;

		List<PathPoint> nexts = findNextPoints(current);

		// setColor(current.component, new Color(.6f, .13f, .86f) );
		// setColor(target.component, new Color(.2f, .8f, 0) );

		foreach( var next in nexts ) {
			if( Search(next, target) ){
				path.Add(next);
				return true;
			}
		}

		return false;
	}


}
