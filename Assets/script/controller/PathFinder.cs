using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder {

	PathPoint _target;

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

		var targets = Utility.GetPointsOnTapPos(tapPos, normal);

		targets = targets.OrderBy(t => {
            return (tapPos - t.screenPosition).sqrMagnitude;
        }).ToList();

		var start = Utility.getPointsAtWorldPos(player, normal)
		.OrderBy(point =>
		{
			return (point.position - player).sqrMagnitude;
		})
		.ElementAt(0);

        Utility.SetPointColor(start.component, new Color(1, 1, 1));

		foreach( var target in targets){

			if( target == _target) continue;

			if (target == start) continue;

			ResetAll( target );            

            if (Search(start, target)) {
				_target = target;
                return true;
            }
		}
        return false;
		
	}

	void ResetAll(PathPoint target){

		path = null;
		possiblePaths.Clear();

		var allPointComponents = Object.FindObjectsOfType<PathPointComponent>();

		foreach(var pointComponent in allPointComponents) {
			pointComponent.point.Reset(target);
			Utility.SetPointColor(pointComponent, new Color(0, 0, 1) );
		}
	}

	static public List<PathPoint> findNextPoints(PathPoint point, Vector3 normal){

		List<PathPoint> nextPoints = new List<PathPoint>();

		var newNextPoints =  Utility.getNextPoints( point, normal, true );

		// A* Logic
		foreach( var nextPoint in newNextPoints ){

			AddToList(nextPoint, point, nextPoints);

		}

		// Add Stairs connections
        if (point.stairConn != null) {
            AddToList(point.stairConn, point, nextPoints);
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

	static void AddToList( PathPoint nextPoint, PathPoint point, List<PathPoint> points){

        // Remove if the point has been check already.
        if (nextPoint.state == PathPoint.State.Closed) return;

        // Remove if the block is in the middle of a rotation.
        if (nextPoint.rotating) return;

        // remove if it is the previous point
        if (nextPoint == point.prev) return;

        if (nextPoint.state == PathPoint.State.Open) {
            if (!nextPoint.isCloser(point)) {
                return;
            }
        } else {
            nextPoint.state = PathPoint.State.Open;
        }

        nextPoint.setPrev(point);

        nextPoint.state = PathPoint.State.Open;

        points.Add(nextPoint);


	}

	bool Search(PathPoint current, PathPoint target, List<PathPoint> currentList = null){

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

		return Search( target );
	}

	bool Search(PathPoint target){
		possiblePaths.OrderBy( eachList => eachList.Last().estimatedCost );
		var next = possiblePaths[0].Last();
		return Search( next, target, possiblePaths[0] );
	}


}
