using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour {

	Renderer renderer;

	private void Start() {
		renderer = GetComponent<Renderer>();
	}
	
}
