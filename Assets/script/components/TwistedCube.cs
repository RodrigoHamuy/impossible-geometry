using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistedCube : MonoBehaviour {

	void Start() {

		var bounds = GetComponent<MeshFilter>().mesh.bounds;
		var matrix = transform.localToWorldMatrix;

		var right = (bounds.extents.x + .5f) * Vector3.right;

		var left = - right;

		var normals = new Vector3[4]{
			Vector3.up,
			Vector3.back,
			Vector3.down,
			Vector3.forward,
		};

		var rightFaces = new List<PathPoint>();
		var leftFaces = new List<PathPoint>();
		
		
		foreach( var normal in normals){

			var faceRight = transform.TransformPoint(right + normal * 0.5f);
			var faceLeft =transform.TransformPoint(left + normal * 0.5f);

			var worldNormal = Utility.CleanNormal( transform.TransformDirection(normal) );

			var rightNodes = Utility.getPointsAtWorldPos( faceRight, worldNormal );
			var leftNodes = Utility.getPointsAtWorldPos( faceLeft, worldNormal );

			// if( rightNodes.Count > 0 ){
			rightFaces.Add( rightNodes[0] );
			// }
			// if( leftNodes.Count > 0 ){
			leftFaces.Add( leftNodes[0] );
			// }

		}

		for( var i = 0; i < rightFaces.Count; i++ ) {

			var rightFace = rightFaces[i];

			var leftFace = leftFaces[ (i+1) % leftFaces.Count ];

			rightFace.twistedBlockConn = leftFace;
			leftFace.twistedBlockConn = rightFace;

		}



	}

}
