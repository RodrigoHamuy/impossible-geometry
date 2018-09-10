// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PointsContainerComponent))]
public class PointContainerInspector : Editor {

	PointsContainerComponent component;

	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		component = (PointsContainerComponent) target;

		if(GUILayout.Button("Reset path points")) {
			ResetPoints();
    }

		if(GUILayout.Button("Convert to prism")) {
			ConvertToPrism();
    }
	}

	void ResetPoints() {
		Debug.Log("Reset points");
		var pointsContainer = component.gameObject.GetComponent<PointsContainerComponent>();
		pointsContainer.ResetPoints();
	}

	void ConvertToPrism() {
		Debug.Log("convert to prism");
		var meshFilter = component.gameObject.GetComponent<MeshFilter>();

		Undo.RecordObject (meshFilter, "Convert to prism");

		var mesh = new Mesh();
		meshFilter.mesh = mesh;

		var center = 0.5f;

		mesh.vertices = new Vector3[]{
			// up
			new Vector3( center,  center, -center),
			new Vector3(-center,  center, -center),
			new Vector3(-center,  center,  center),
			new Vector3( center,  center,  center),
			// back
			new Vector3( center,  center, -center),
			new Vector3( center,  center,  center),
			new Vector3( center, -center,  center),
			new Vector3( center, -center, -center),
			// left
			new Vector3( center, -center, -center),
			new Vector3(-center,  center, -center),
			new Vector3( center,  center, -center),
			new Vector3(-center, -center, -center),
			// right
			new Vector3( center,  center,  center),
			new Vector3(-center,  center,  center),
			new Vector3( center, -center,  center),
		};
		mesh.triangles = new int[]{
			 0, 1, 2, // up
			 0, 2, 3,
			 4, 5, 6, // back
			 7, 4, 6,
			 8, 9,10, // left
			 8,11, 9,
			12,13,14, // right
		};
		mesh.normals = new Vector3[]{
			// up
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up,
			// back
			Vector3.right,
			Vector3.right,
			Vector3.right,
			Vector3.right,
			// left
			Vector3.back,
			Vector3.back,
			Vector3.back,
			Vector3.back,
			// right
			Vector3.forward,
			Vector3.forward,
			Vector3.forward,
		};
		mesh.uv = new Vector2[]{
			// up
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(0,1),
			new Vector2(1,1),
			// back
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(0,1),
			new Vector2(1,1),
			// left
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(1,1),
			new Vector2(0,1),
			// right
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(1,1),
		};

		// mesh.RecalculateNormals();


	}
}
