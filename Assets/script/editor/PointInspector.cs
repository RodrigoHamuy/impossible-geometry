using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(PathPointComponent))]
public class PointInspector : Editor {

	bool isDebugDone = false;
	PathPointComponent lastPoint;

	List<Vector3> lineSegments = new List<Vector3>();

	void OnSceneGUI() {

		var targetObject = (PathPointComponent) target;

		var tex2 = new Texture2D(1, 1);
		var fillColor = new Color( 1, 1, 1, .5f );
		var fillColorArray =  tex2.GetPixels();
		for(var i = 0; i < fillColorArray.Length; ++i) {
			fillColorArray[i] = fillColor;
		}
		tex2.SetPixels( fillColorArray );
		tex2.Apply();

		GUIStyle style = new GUIStyle();
		// style.normal.textColor = Color.green;
		style.normal.background = tex2;

		Handles.BeginGUI();
		Handles.Label(
			targetObject.transform.position,
			targetObject.transform.position.ToString(),
			style
		);
		// Handles.DrawLines(lineSegments.ToArray());
		Handles.EndGUI();

		// CheckRay(targetObject);

	}

	void CleanAll(){
		var allPoints = Object.FindObjectsOfType<PathPointComponent>();
		lineSegments.Clear();
		foreach( var point in allPoints ){
			setColor(point, new Color(.4f, .0f, .8f) );
		}
	}

	public void CheckRay(PathPointComponent pointComponent){

		if( lastPoint != pointComponent ) {

			CleanAll();
			isDebugDone = false;
			lastPoint = pointComponent;
		}

		if (isDebugDone) return;
		isDebugDone = true;

		var point = pointComponent.point;

		Vector3[] directions = {
			point.component.transform.transform.forward,
			point.component.transform.transform.right,
			- point.component.transform.transform.forward,
			- point.component.transform.transform.right
		};

		foreach( var dir in directions){

			var pos = point.position + dir;
			Debug.Log(pos);
			var nextPoints = getPointsAtWorldPos( pos );

			foreach( var nextPoint in nextPoints ) {
				setColor(nextPoint.component, new Color(1.0f, .2f, 0) );
			}

		}

	}

	List<PathPoint> getPointsAtWorldPos(Vector3 pos){
		var screenPos = Camera.main.WorldToScreenPoint(pos);
		var ray = Camera.main.ScreenPointToRay(screenPos);
		return getPointsAtRay(ray);
	}

	void AddDebugLine(Ray ray){
		var start = ray.origin;
		var end =  ray.origin + ray.direction * (lastPoint.transform.position-ray.origin).magnitude;
		lineSegments.Add(start);
		lineSegments.Add(end);
	}


	List<PathPoint> getPointsAtRay(Ray ray){

		List<PathPoint> points = new List<PathPoint>();

		AddDebugLine(ray);

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

	void setColor(PathPointComponent point, Color color){

		var rend = point.GetComponentsInChildren<Renderer>()[0];
		rend.material.color = color;
	}

	[MenuItem("MyMenu/Align camera")]
  static void AlignCamera() {

			var camera = Camera.main.transform;

			SceneView.lastActiveSceneView.pivot = camera.position;
			SceneView.lastActiveSceneView.rotation = camera.rotation;
      SceneView.lastActiveSceneView.Repaint();

  }

}
