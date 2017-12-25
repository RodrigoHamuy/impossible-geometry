using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

	public static bool canPlayerMove = true;

	// Get Points

    static List<PathPoint> getPointsAtRay(Ray ray, Vector3 normal) {

        var layer = PathPoint.layer[normal];

        List<PathPoint> points = new List<PathPoint>();

        var layerMask = LayerMask.GetMask(layer);

        var hits = Physics.RaycastAll(ray, 100.0f, layerMask);

        foreach (var hit in hits)
        {

            var point = hit.collider.transform.parent.GetComponent<PathPointComponent>().point;

            // TODO: Normal should be decided from the last node,
            // as the player may be able to move on diff normals.

            // if (point.normal == normal)
            // {
            points.Add(point);
            // }

        }

        return points;
    }

    static List<PathPoint> getPointsAtScreenPos(Vector3 pos, Vector3 normal) {
        var ray = Camera.main.ScreenPointToRay(pos);
        if (IsBehind(normal))
        {
            ray.origin = ray.origin + ray.direction * 100;
            ray.direction = -ray.direction;

        }
        return getPointsAtRay(ray, normal);
    }

    public static List<PathPoint> getPointsAtWorldPos(Vector3 pos, Vector3 normal) {
        var screenPos = Camera.main.WorldToScreenPoint(pos);
        return getPointsAtScreenPos(screenPos, normal);
    }

	public static List<PathPoint> GetPointsOnTapPos( Vector3 tapPos, Vector3 normal ) {

        var points = new List<PathPoint>();
		var ray = Camera.main.ScreenPointToRay(tapPos);
        var layerMask = LayerMask.GetMask("Debug");
		var hits = Physics.RaycastAll(ray, 100.0f, layerMask);

		foreach (var hit in hits)
        {

            var point = hit.collider.transform.GetComponent<PathPointComponent>().point;

            if (point.normal == normal) {
                points.Add(point);
            }

        }

        return points;
	}

    public static PathPoint GetCloser( List<PathPoint> points, Vector3 tapPos ) {

		if (points.Count == 0) return null;
		
		var closerPoint = points.OrderBy( p => {
			return ( tapPos - p.screenPosition).sqrMagnitude;
		})
		.ElementAt(0);

        return closerPoint;
    }

    public static List<PathPoint> getNextPoints(
        PathPoint point,
        Vector3 normal,
        bool pathFindEnabled = false
    ) {

        List<PathPoint> nextPoints = new List<PathPoint>();

        var overlappingPoints = getPointsAtWorldPos(point.position, normal);

        overlappingPoints.RemoveAll(overlappingPoint =>
        {
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

        foreach (var dir in directions)
        {

            var pos = point.position + dir;
            var axis = GetNormalAxis(normal);

            var newNextPoints = getPointsAtWorldPos(pos, normal);
            var newNextPointsCopy = new List<PathPoint>(newNextPoints);

            // var CamDirDot = Vector3.Dot(Camera.main.transform.forward, dir);

            var wallsNormals = new List<Vector3>{
                PathPoint.CleanNormal(dir), 
                PathPoint.CleanNormal(-dir)
            };

            var camForward = Camera.main.transform.forward;


            List<PathPoint> potentialWalls = new List<PathPoint>();

            foreach( var wallNormal in wallsNormals ) {

                if (Vector3.Dot(wallNormal, camForward) > 0) continue;

                potentialWalls.AddRange(
                    getPointsAtWorldPos(
                        point.position + dir * 0.25f,
                        wallNormal
                    )
                );
                potentialWalls.AddRange(
                    getPointsAtWorldPos(
                        point.position + dir * 0.75f,
                        wallNormal
                    )
                );
            }

            // Remove if it is overlapped by another point bellow the same ray.
            newNextPoints.RemoveAll(nextPoint =>
            {

                if (pathFindEnabled)
                {
                    // Remove if the point has been check already.
                    if (nextPoint.state == PathPoint.State.Closed) return true;

                    // Remove if the block is in the middle of a rotation.
                    if (nextPoint.rotating) return true;

                    // remove if it is the previous point
                    if (nextPoint == point.prev) return true;
                }

                // Remove if nextPoint is bellow another nextPoint that is at the same
                // height or bellow the current point.
                if (
                    newNextPointsCopy.Exists(nextPoint2 =>
                    {
                        if (
                            nextPoint2 != nextPoint &&
                            nextPoint2.position[axis] <= point.position[axis]
                        )
                        {
                            return nextPoint2.position[axis] > nextPoint.position[axis];
                        }
                        else return false;
                    })
                ) return true;

                // Check if nextPoint is next to a point that is on top of point.
                if (
                    overlappingPoints.Exists((overlappingPoint) =>
                    {
                        return (
                            overlappingPoint.position[axis] <= nextPoint.position[axis] &&
                            overlappingPoint.position[axis] > point.position[axis]
                        );
                    })
                ) return true;


                // Detect walls
                var realDir = nextPoint.position - point.position;
                var realDirDotNormal = Vector3.Dot( realDir, normal );
                if (
                    potentialWalls.Exists((overlappingPoint) =>
                    {
                        var wallToPoint = point.position - overlappingPoint.position;
                        var wallToNext = nextPoint.position - overlappingPoint.position;

                        var wallToPointDotNormal = Vector3.Dot(wallToPoint, normal);
                        var wallToNextDotNormal = Vector3.Dot(wallToNext, normal);

                        if ( 
                            (
                                wallToPointDotNormal > 0 &&
                                wallToNextDotNormal < 0
                            ) || (
                                wallToPointDotNormal < 0 &&
                                wallToNextDotNormal > 0
                            )
                        ){
                            return true;
                        }

                        if( 
                            (
                                Vector3.Dot(wallToPoint, camForward) > 0 &&
                                wallToPoint.sqrMagnitude < 1.0f &&
                                wallToPointDotNormal < 0
                            ) ||
                            (
                                Vector3.Dot(wallToNext, camForward) > 0 &&
                                wallToNext.sqrMagnitude < 1.0f &&
                                wallToNextDotNormal < 0
                            )
                        ){
                            return true;
                        }
                        //     if( realDirDotNormal < 0 ) {
                        //         return true;
                        //     }
                        //     else if( wallToPoint.sqrMagnitude < 1.0f  ){
                        //         return true;
                        //     }
                        // }

                        // if (
                        //     Vector3.Dot(wallToNext, normal) > 0 &&
                        //     wallToNext.sqrMagnitude < 1.0f
                        // )
                        // {
                        //     return true;
                        // }
                        return false;
                    })
                ) return true;

                // If this is an Ok point
                return false;
            });


            foreach (var nextPoint in newNextPoints)
            {
                nextPoints.Add(nextPoint);
            }

        }

        return nextPoints;
    }

	// Points utils

	public static bool IsBehind(Vector3 n){
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
	
	public static Vector3 getDirFromScreenView( Vector3 from, Vector3 to ){

		var cam = Camera.main;

		var dir = Vector3.ProjectOnPlane(
			to - from,
			- cam.transform.forward
		).normalized;
		for (var i = 0; i < 3; i++) {
			dir[i] = Mathf.Round(dir[i]);
		}
		return dir.normalized;
	}

	public static void SetPointColor(PathPointComponent point, Color color) {
        var rend = point.GetComponentsInChildren<Renderer>()[0];
        rend.material.color = color;
    }

    // Touch

    public static Vector2 getTouchEnd()
    {
        Vector2 tapPos = Vector2.zero;

        if (
            (Input.touchCount > 0) &&
            (Input.GetTouch(0).phase == TouchPhase.Ended)
        )
        {

            var touch = Input.GetTouch(0);
            tapPos = touch.position;

        }
        else if (Input.mousePresent && Input.GetMouseButtonUp(0))
        {

            tapPos = Input.mousePosition;

        }

        return tapPos;
    }


    // public static Vector3 GetTouchPosition(){
    // 	if (Input.touchCount == 1) {
    // 		var touch = Input.GetTouch (0);
    // 		return touch.position;
    // 	} else if ( Input.mousePresent ) {
    // 		return Input.mousePosition;
    // 	}
    // 	return Vector3.zero;
    // }


}
