using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour {

	public Transform head;
	public Transform body;

	PlayerComponent player;

	float animStepTime = 0.1f;

	void Start() {
		player = GetComponent<PlayerComponent>();
	}

	void Update() {

		if( ! player.isMoving ) {

			if( Vector3.Dot( head.up, body.up ) > 0.001f ) {
				head.rotation = Quaternion.SlerpUnclamped( 
					head.rotation, 
					body.rotation, 
					Time.deltaTime * 10.0f 
				);
			} else {
				head.rotation = body.rotation;
			}

			return; 
		} else {

			WaveHead();

		}
	}

	void WaveHead() {

		var angle = 10.0f * Mathf.Sin(Time.time * (1 / animStepTime * 2.0f));

		var rotation = Quaternion.AngleAxis( angle, body.forward );

		head.rotation = rotation * body.rotation;

	}

}
