using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class StairComponent : MonoBehaviour {
	
	void Start () {

		var bounds = transform.Find("Cube").GetComponent<Renderer>().bounds;

		var top = transform.position 
			+ transform.up * bounds.extents.y;

		var bottom = transform.position
            - transform.up * bounds.extents.y
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

	void Update () {
		
	}
}
