using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder {

	PathPoint target;

	Vector3 normal;

	public List<PathPoint> path;

	List< List <PathPoint> > possiblePaths = new List< List <PathPoint> >();

	// List<PathPoint> openPoints;

	public bool MovePlayerTo(
		Vector3 player,
		Vector3 tapPos,
		Vector3 n = default(Vector3)
	){

		if( n == Vector3.zero ) n = Vector3.up;

		normal = n;
		// TODO: Find out the most suitable point target.
		// Maybe the closest to the player level.
		// Or maybe the closest to the previous point.
		// This would require to decide the target after the path is found.

		var newTarget = Utility.GetCloser(
			Utility.GetPointsAtPos(tapPos, normal),
			tapPos
		);

		if( newTarget == null || newTarget == target
		) return false;

		target = newTarget;

		ResetAll();

		var start = getPointsAtWorldPos(player, normal)
		.OrderBy( point => {
			return (point.position - player).sqrMagnitude;
		})
		.ElementAt(0);

		setColor(start.component, new Color(1, 1, 1) );
		if( Search(start) ) {
			return true;
		} else {
			target = null;
			return false;
		}
	}

	void ResetAll(){

		path = null;
		possiblePaths.Clear();

		var allPointComponents = Object.FindObjectsOfType<PathPointComponent>();

		foreach(var pointComponent in allPointComponents) {
			pointComponent.point.Reset(target);
			setColor(pointComponent, new Color(0, 0, 1) );
		}
	}

	static List<PathPoint> getPointsAtScreenPos(Vector3 pos, Vector3 normal){
		var ray = Camera.main.ScreenPointToRay(pos);
		if ( IsBehind(normal) ) {
			ray.origin = ray.origin + ray.direction*100;
			ray.direction = - ray.direction;

		}
		return getPointsAtRay(ray, normal);
	}

	static public List<PathPoint> getPointsAtWorldPos(Vector3 pos, Vector3 normal) {
		var screenPos = Camera.main.WorldToScreenPoint(pos);
		return getPointsAtScreenPos(screenPos, normal);
	}

	static List<PathPoint> getPointsAtRay(Ray ray, Vector3 normal){

		var layer = PathPoint.layer[ normal ];

		List<PathPoint> points = new List<PathPoint>();

		var layerMask = LayerMask.GetMask(layer);

		var hits = Physics.RaycastAll(ray, 100.0f, layerMask);

		foreach( var hit in hits ) {

			var point = hit.collider.transform.parent.GetComponent<PathPointComponent>().point;

			// TODO: Normal should be decided from the last node,
			// as the player may be able to move on diff normals.

			if( point.normal == normal ){
				points.Add(point);
			}

		}

		return points;
	}

	static bool IsBehind(Vector3 n){
		var angle = Vector3.Angle( Camera.main.transform.forward, n );
		return angle < 90 ;
	}

	static int GetNormalAxis(Vector3 n){
		for (var i = 0; i < 3; i++) {
			if( n[i] != 0 ) return i;
		}
		Debug.LogError("This is not a normal");
		return -1;
	}

	static List<PathPoint> GetCrossOverlaps( Vector3 pos, Vector3 normal, int axis ) {

		// This is to move the ray half way down, as otherwise
		// it wont hit a cross overllap.

		var axisDir = IsBehind(normal) ? -1 : 1;

		var halfDir = Vector3.zero;
		halfDir[ ( axis + axisDir ) % 3 ] = normal[axis] * 0.5f;

		pos += halfDir;

		var crossNormal = Vector3.zero;
		var newAxis = ( axis + axisDir + axisDir ) % 3;
		if( newAxis < 0) newAxis += 3;
		crossNormal[ newAxis ] = normal[axis];
		crossNormal = PathPoint.CleanNormal( crossNormal );

		var crossOverlaps = getPointsAtWorldPos( pos, crossNormal );

		return crossOverlaps;
	}

	public static List<PathPoint> findNextPoints(PathPoint point, Vector3 normal){

		List<PathPoint> nextPoints = new List<PathPoint>();

		var overlappingPoints = getPointsAtWorldPos(point.position, normal);

		overlappingPoints.RemoveAll( overlappingPoint => {
			return (
				overlappingPoint.position.y < point.position.y ||
				overlappingPoint == point
			);
		});

		Vector3[] directions = {
			point.component.transform.transform.forward,
			point.component.transform.transform.right,
			- point.component.transform.transform.forward,
			- point.component.transform.transform.right
		};

		foreach( var dir in directions){

			var pos = point.position + dir;
			var axis = GetNormalAxis(normal);

			var newNextPoints = getPointsAtWorldPos( pos, normal );
			var crossOverlaps = GetCrossOverlaps( pos, normal, axis );


			// Remove if it is overlapped by another point bellow the same ray.
			newNextPoints.RemoveAll( nextPoint => {

				// Remove if the point has been check already.
				if (nextPoint.state == PathPoint.State.Closed ) return true;

				// Remove if the block is in the middle of a rotation.
				if ( nextPoint.rotating ) return true;

				// remove if this nextPoint is above the current point (from camera
				// perspective) and his block overlaps the current point.
				if(
					! nextPoint.isPrismSide &&
					nextPoint.camPosition[axis] > point.camPosition[axis] &&
					nextPoint.position[axis] > point.position[axis]
				) return true;

				// remove if this block is bellow the current point (from camera
				// perspective) and is being overlapped by the current point.
				if(
					! point.isPrismSide &&
					nextPoint.camPosition[axis] < point.camPosition[axis] &&
					nextPoint.position[axis] < point.position[axis]
				) return true;

				// remove if it is the previous point
				if ( nextPoint == point.prev ) return true;

				// Remove if nextPoint is bellow another nextPoint that is at the same
				// height or bellow the current point.
				if( nextPoint.camPosition.y > point.camPosition.y ){
					if(
						newNextPoints.Exists( nextPoint2 =>{
							if( nextPoint2.position.y <= point.position.y ){
								return nextPoint2.position.y > nextPoint.position.y;
							} else return false;
						})
					) return true;
				}

				// Check if nextPoint is next to a point that is on top of point.
				if (
					overlappingPoints.Exists( (overlappingPoint) => {
						return (
							overlappingPoint.position.y <= nextPoint.position.y &&
							overlappingPoint.screenPosition.y > nextPoint.screenPosition.y
						);
					})
				) return true;


				// Cross overlaps
				if(
					crossOverlaps.Exists( (overlappingPoint) =>{
						return (
							overlappingPoint.realCamPosition.z < nextPoint.realCamPosition.z &&
							overlappingPoint.realCamPosition.z > point.realCamPosition.z
						);
					})
				) return true;

				// TODO: Maybe remove this? Not sure what it is
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
				// 		return overlappingPoint.position.y <= nextPoint.position.y;
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

	bool Search(PathPoint current, List<PathPoint> currentList = null){

		if( current == target) {
			path = currentList;
			return true;
		}

		if( currentList == null ) {
			currentList = new List<PathPoint>();
			possiblePaths.Add( currentList );
			currentList.Add(current);
		}

		current.target = target;
		current.state = PathPoint.State.Closed;

		List<PathPoint> nexts = findNextPoints(current, normal);

		// setColor(current.component, new Color(.6f, .13f, .86f) );
		// setColor(target.component, new Color(.2f, .8f, 0) );

		possiblePaths.Remove(currentList);

		foreach( var next in nexts ) {

			List<PathPoint> nextList = new List<PathPoint>();
			nextList.AddRange(currentList);
			possiblePaths.Add( nextList );
			nextList.Add(next);

		}

    if (possiblePaths.Count == 0) return false;

		return Search();
	}

	bool Search(){
		possiblePaths.OrderBy( eachList => eachList.Last().estimatedCost );
		var next = possiblePaths[0].Last();
		return Search( next, possiblePaths[0] );
	}


}
