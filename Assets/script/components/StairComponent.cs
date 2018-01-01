using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class StairComponent : MonoBehaviour {
	
	void Start () {

        var cube = transform.Find("Cube");

		var bounds = cube.GetComponent<MeshFilter>().mesh.bounds;

        var scale = cube.localScale;

		var top = transform.position 
			+ transform.up * bounds.extents.y * scale.y;

		var bottom = transform.position
            - transform.up * bounds.extents.y * scale.y
            + transform.forward;

        var pointTop = Utility.getPointsAtWorldPos(
            top,
            Utility.CleanNormal(transform.up)
        ).OrderBy(p => {
            return (top - p.position).sqrMagnitude;
        }).ElementAt(0);

        Utility.SetPointColor(pointTop.component, Color.cyan);

        var pointBottom = Utility.getPointsAtWorldPos(
            bottom,
            Utility.CleanNormal(transform.up)
        ).OrderBy(p => {
            return (bottom - p.position).sqrMagnitude;
        }).ElementAt(0);

        Utility.SetPointColor(pointBottom.component, Color.cyan);

		pointTop.stairConn = pointBottom;
		pointBottom.stairConn = pointTop;

        pointTop.stairPos = PathPoint.StairPos.top;
        pointBottom.stairPos = PathPoint.StairPos.bottom;
		
	}
}
