using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRotator : MonoBehaviour {

	public void OnTouchEnd(Vector2 touchPos) {

		var block = GetBlocksOnTapPos(touchPos);

		if(block){

			block.Rotate(Vector3.up, 90.0f, Space.World);

		}

	}


  public static Transform GetBlocksOnTapPos (Vector3 tapPos) {

    var points = new List<PathPoint> ();
    var ray = Camera.main.ScreenPointToRay (tapPos);
    var layerMask = LayerMask.GetMask ("Block");
    var hits = Physics.RaycastAll (ray, 100.0f, layerMask);

    foreach (var hit in hits) {

      var block = hit.collider.transform.GetComponent<Transform> ();

			return block;

    }

		return null;

  }
}
