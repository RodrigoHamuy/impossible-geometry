// using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

	public static bool canPlayerMove = true;

	public static float SignedAngle(Vector3 a, Vector3 b, Vector3 axis ) {
		var angle = Vector3.Angle(a, b);
		var cross = Vector3.Cross(a.normalized, b.normalized);
		var dot = Vector3.Dot(cross, axis);
		if (dot < 0) {
			angle = -angle;
		}
		return angle;
	}

	public static Vector2 getTouchEnd(){
		Vector2 tapPos = Vector2.zero;

		if (
			(Input.touchCount > 0) &&
			(Input.GetTouch(0).phase == TouchPhase.Ended)
		) {

			var touch = Input.GetTouch(0);
			tapPos = touch.position;

		} else if( Input.mousePresent ){

			tapPos = Input.mousePosition;

		}

		return tapPos;
	}

	public static List<PathPoint> findNextPoints(PathPoint point, Vector3 normal){

		normal = PathPoint.CleanNormal(normal);

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

				// default return
				return false;
			});

			foreach( var nextPoint in newNextPoints ){
				nextPoints.Add(nextPoint);
			}

		}

		return nextPoints;
	}

	static List<PathPoint> getPointsAtScreenPos(Vector3 pos, Vector3 normal){
		var ray = Camera.main.ScreenPointToRay(pos);
		if ( IsBehind(normal) ) {
			ray.origin = ray.origin + ray.direction*100;
			ray.direction = - ray.direction;

		}
		return getPointsAtRay(ray, normal);
	}

	public static List<PathPoint> getPointsAtWorldPos(Vector3 pos, Vector3 normal) {
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

	public static Vector3 GetTouchPosition(){
		if (Input.touchCount == 1) {
			var touch = Input.GetTouch (0);
			return touch.position;
		} else if ( Input.mousePresent ) {
			return Input.mousePosition;
		}
		return Vector3.zero;
	}


}
