using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(PathPointComponent))]
public class PointInspector : Editor {

	void OnSceneGUI() {

		var targetObject = (PathPointComponent) target;

		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.green;

		Handles.BeginGUI();
		Handles.Label(
			targetObject.transform.position,
			targetObject.transform.position.ToString(),
			style
		);
		Handles.EndGUI();

	}

}
