using UnityEngine;

public class CubeTransition : MonoBehaviour {

	Material mat;

	Vector3 initialPos;
	Quaternion initialRotation;
	Vector3 initialScale;

	Color initialColor;


	void Start () {

		mat = GetComponent<MeshRenderer>().material;

		initialColor = mat.GetColor("_Color");

		var color = new Color();

		ColorUtility.TryParseHtmlString("#FF99004A", out color);

		mat.SetColor("_Color", color );

		initialPos = transform.position;
		// initialRotation = transform.rotation;
		initialScale = transform.localScale;

		var camera = Camera.main;

		transform.position = camera.transform.position + camera.transform.forward*10.0f;
		// transform.LookAt( camera.transform.position );
		float scale = 5.0f;
		transform.localScale = new Vector3(scale, scale, scale);

	}

	float totalTime = 0;
	int steps = 0;

	void Update () {

		float colorStartTime = 0.15f;

		if( steps == 0 ) {
			totalTime += Time.deltaTime * .75f;
			if (totalTime > 1 ) {
				totalTime = 0.0f;
				++ steps;
			}

		} else if( steps == 1 ) {
			totalTime += Time.deltaTime * 0.1f;

			if( totalTime > 1) {
				UpdatePosAndScale( 1 );
				totalTime = 1.0f - colorStartTime;
				++ steps;
			} else {
				UpdatePosAndScale( totalTime );
				if( totalTime >= colorStartTime ) {
					UpdateColor( totalTime - colorStartTime );
				}
			}
		} else if ( steps == 2 ) {
			totalTime += Time.deltaTime * 1.0f;
			if( totalTime > 1) {
				UpdateColor( 1 );
				totalTime = 0;
				++ steps;
			} else {
				UpdateColor( totalTime );
			}
		}

	}

	void UpdatePosAndScale( float amount ){
		// transform.rotation = Quaternion.Lerp( transform.rotation, initialRotation, totalTime );

		if ( amount == 1.0f ) {
			transform.position = initialPos;
			transform.localScale = initialScale;
			return;
		}
		transform.position = Vector3.Lerp( transform.position, initialPos, amount );
		transform.localScale = Vector3.Lerp( transform.localScale, initialScale, amount );
	}


	void UpdateColor( float amount ){

		if ( amount == 1.0f ) {
			mat.SetColor("_Color", initialColor);
			return;
		}

		var color = mat.GetColor("_Color");
		mat.SetColor("_Color", Color.Lerp(
			color, initialColor, amount
		));
	}
}
