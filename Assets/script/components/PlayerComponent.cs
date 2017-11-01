using UnityEngine;

public class PlayerComponent : MonoBehaviour {

	bool isMoving = false;

	public void FindCurrentPoint () {

		var layerMask = LayerMask.GetMask("Debug.Point");
		var dir = transform.position - Camera.main.transform.position;
		var ray = new Ray {
			direction = dir,
			origin = Camera.main.transform.position
		};

		var hits = Physics.RaycastAll(ray, 100.0f, layerMask);

		Debug.Log(hits.Length);

		foreach( var hit in hits ) {

			var point = hit.collider.transform;

			if( point.parent.up == transform.up ){
				var rend = point.GetComponent<Renderer>();
				var color = rend.material.color;
				color.r = 1;
				rend.material.color = color;
			}

		}

	}

	public void MoveTo(){

		if ( !Input.GetMouseButtonDown(0) || isMoving ) return;

		isMoving = true;

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

	}

}
