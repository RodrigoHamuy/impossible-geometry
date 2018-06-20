using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddOnHold : MonoBehaviour {

	public Transform blockPrefab;
	public Transform cubeBoyPrefab;
	public Transform targetPrefab;

	float currentY;

	bool isPainting = false;

	List<Transform> currentRow = new List<Transform> ();

	Transform cubeBoy;
	Transform target;

	public void StartStroke (Vector2 screenPos) {

		var hitPos = TouchUtility.HitPosition (screenPos, gameObject, true);

		currentY = hitPos.y;

		var pos = transform.position;
		pos.y = currentY;
		transform.position = pos;

		isPainting = true;

		currentRow.Clear ();

	}

	public void MoveStroke (Vector2 screenPos) {

		var hitPos = TouchUtility.HitPosition (screenPos, gameObject);

		if (currentY != hitPos.y) return;

		var spaceTaken = currentRow.Exists ((Transform oldBlock) => {

			return oldBlock.position == hitPos;

		});

		if (
			currentRow.Count > 1 &&
			currentRow[currentRow.Count - 2].position == hitPos
		) {

			var lastBlock = currentRow[currentRow.Count - 1];
			currentRow.RemoveAt (currentRow.Count - 1);
			Destroy(lastBlock.gameObject);

			return;

		}

		if (spaceTaken) return;

		var block = Instantiate (blockPrefab, hitPos, Quaternion.identity);

		block.gameObject.layer = LayerMask.NameToLayer ("maker.newBlock");

		currentRow.Add (block);

		if (cubeBoy == null) {

			cubeBoy = Instantiate (cubeBoyPrefab, hitPos + Vector3.up * 0.5f, Quaternion.identity).transform;

			cubeBoy.RotateAround (cubeBoy.position, Vector3.up, 180.0f);

		}

	}

	public void EndStroke (Vector2 screenPos) {

		isPainting = false;

		int layer = LayerMask.NameToLayer ("Block");

		currentRow.ForEach ((block) => {

			block.gameObject.layer = layer;

		});

		var hitPos = TouchUtility.HitPosition (screenPos, gameObject);

		if (target == null) {

			target = Instantiate (targetPrefab, hitPos + Vector3.up * 0.5f, Quaternion.identity).transform;

		}

	}

}