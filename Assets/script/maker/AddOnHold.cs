using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnHold : MonoBehaviour {

	public Transform blockPrefab;

	float currentY;

	bool isPainting = false;

	List<Transform> currentRow = new List<Transform> ();

	public void StartStroke (Vector2 screenPos) {

		var hitPos = TouchUtility.HitPosition (screenPos, gameObject);

		currentY = hitPos.y;

		// var pos = transform.position;
		// pos.y = currentY;
		// transform.position = pos;

		isPainting = true;

		currentRow.Clear ();

	}

	public void MoveStroke (Vector2 screenPos) {

		var hitPos = TouchUtility.HitPosition (screenPos, gameObject);

		if (currentY != hitPos.y) return;

		var spaceTaken = currentRow.Exists ((Transform oldBlock) => {

			return oldBlock.position == hitPos;

		});

		if (spaceTaken) return;

		var block = Instantiate (blockPrefab, hitPos, Quaternion.identity);

		block.gameObject.layer = LayerMask.NameToLayer ("maker.newBlock");

		currentRow.Add (block);

	}

	public void EndStroke (Vector2 screenPos) {

		isPainting = false;

		int layer = LayerMask.NameToLayer ("Block");

		currentRow.ForEach ((block) => {

			block.gameObject.layer = layer;

		});

	}

}