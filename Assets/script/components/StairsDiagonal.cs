using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StairsDiagonal : MonoBehaviour {

	private void Start() {
		var bounds = GetComponent<Renderer>().bounds;

		var top = transform.position
            + transform.up * bounds.extents.y 
			+ transform.forward * bounds.extents.z + transform.forward * 0.5f;

        var bottom = transform.position
            - transform.up * bounds.extents.y
            - transform.forward * bounds.extents.z - transform.forward * 0.5f;

        var pointTop = Utility.getPointsAtWorldPos(
            top,
            Utility.CleanNormal(transform.up)
        ).OrderBy(p =>
        {
            return (top - p.position).sqrMagnitude;
        }).ElementAt(0);

        Utility.SetPointColor(pointTop.component, Color.cyan);

        var pointBottom = Utility.getPointsAtWorldPos(
            bottom,
            Utility.CleanNormal(transform.up)
        ).OrderBy(p =>
        {
            return (bottom - p.position).sqrMagnitude;
        }).ElementAt(0);

        Utility.SetPointColor(pointBottom.component, Color.cyan);

        pointTop.stairDiagonalConn = pointBottom;
        pointBottom.stairDiagonalConn = pointTop;

        // pointTop.stairPos = PathPoint.StairPos.top;
        // pointBottom.stairPos = PathPoint.StairPos.bottom;
	}
	
}
