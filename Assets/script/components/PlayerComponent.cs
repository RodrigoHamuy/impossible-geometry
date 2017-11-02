using UnityEngine;

public class PlayerComponent : MonoBehaviour {

	bool isMoving = false;

	PathFinder pathFinder = new PathFinder();

	void Update(){
		MoveTo();
	}

	public void FindCurrentPoint () {

		var dir = transform.position - Camera.main.transform.position;
		var ray = new Ray {
			direction = dir,
			origin = Camera.main.transform.position
		};

		GetControlPoint(ray);
	}

	public void MoveTo(){

		if ( !Input.GetMouseButtonDown(0) || isMoving ) return;

		isMoving = true;

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		GetControlPoint(ray);
	}

	void GetControlPoint(Ray ray){

		var layerMask = LayerMask.GetMask("Debug.Point");

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

}
