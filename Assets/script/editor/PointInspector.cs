// using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(PathPointComponent))]
public class PointInspector : Editor {

	bool isDebugDone = false;
	PathPointComponent lastPoint;

	void OnSceneGUI() {

		var targetObject = (PathPointComponent) target;

		GUIStyle style = new GUIStyle();
		// style.normal.textColor = Color.green;

		Handles.BeginGUI();
		Handles.Label(
			targetObject.transform.position,
			targetObject.transform.position.ToString(),
			style
		);
		Handles.EndGUI();

		CheckRay(targetObject);

	}

	void CleanAll(){
		var allPoints = Object.FindObjectsOfType<PathPointComponent>();
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
			var nextPoint = getPointAtWorldPos( pos );

			if( nextPoint == null)
			continue;

			setColor(nextPoint.component, new Color(1.0f, .2f, 0) );

		}

	}

	PathPoint getPointAtWorldPos(Vector3 pos){

		var camera = Camera.main.transform;

		var dir = pos - camera.position;

		Debug.DrawRay(camera.position, dir, Color.green, 25.0f);

		var ray = new Ray {
			direction = dir,
			origin = camera.position
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
