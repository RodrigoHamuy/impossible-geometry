using System.Collections.Generic;
using UnityEngine;

public class PathFinder {

	List<PathPoint> _points;

	public void GenerateGrid(){

		_points = new List<PathPoint>();

		var points = Object.FindObjectsOfType<PathPointComponent>();

	}

	public void OnTap(Vector3 tapPos){

		Ray ray = Camera.main.ScreenPointToRay(tapPos);

		var layerMask = LayerMask.GetMask("Path");

    var hits = Physics.RaycastAll(ray, 100, layerMask);

		for (int i = 0; i < hits.Length; i++) {
      var hit = hits[i];
      Renderer rend = hit.transform.GetComponent<Renderer>();
			if (rend) {
          Color tempColor = rend.material.color;
          tempColor.a = 1;
          rend.material.color = tempColor;
      }
		}

	}


}
