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

		var newTarget = Utility.GetCloser(
			Utility.GetPointsOnTapPos(tapPos, normal),
			tapPos
		);

		if( newTarget == null || newTarget == target
		) return false;

		target = newTarget;

		ResetAll();

		var start = Utility.getPointsAtWorldPos(player, normal)
		.OrderBy( point => {
			return (point.position - player).sqrMagnitude;
		})
		.ElementAt(0);

		Utility.SetPointColor(start.component, new Color(1, 1, 1) );
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
			Utility.SetPointColor(pointComponent, new Color(0, 0, 1) );
		}
	}

	public static List<PathPoint> findNextPoints(PathPoint point, Vector3 normal){

		List<PathPoint> nextPoints = new List<PathPoint>();

		var newNextPoints =  Utility.getNextPoints( point, normal, true );

		// A* Logic
		foreach( var nextPoint in newNextPoints ){

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

		}

		nextPoints = nextPoints
		.OrderBy( (PathPoint nextPoint) => {
			return nextPoint.estimatedCost;
		})
		.ThenBy( (PathPoint nextPoint) =>{
			return ( point.position - nextPoint.position ).sqrMagnitude;
		})
		.ToList();

		return nextPoints;
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
