using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchBlock : MonoBehaviour {

	void Start()
    {

        var bounds = GetComponent<MeshFilter>().mesh.bounds;
		
        var rightDown = bounds.extents.x * Vector3.right 
		+ bounds.extents.y * Vector3.down + Vector3.down * .5f;

        var leftUp = bounds.extents.x * Vector3.left
        + bounds.extents.y * Vector3.up + Vector3.left * .5f;

		rightDown = transform.TransformPoint(rightDown);

		leftUp = transform.TransformPoint(leftUp);

		var rightDownNormal = Utility.CleanNormal(
			transform.TransformVector( Vector3.right )
		);

		var leftUpNormal = Utility.CleanNormal(
			transform.TransformVector(Vector3.up)
		);

		var rightNode = Utility.getPointsAtWorldPos(rightDown, rightDownNormal)[0];
        var leftNode = Utility.getPointsAtWorldPos(leftUp, leftUpNormal)[0];

		rightNode.archBlockConn = leftNode;
		leftNode.archBlockConn = rightNode;

    }

}
