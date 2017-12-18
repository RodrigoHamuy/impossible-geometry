// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class HandleComponent : MonoBehaviour {

	Transform[] handleParts = new Transform[2];

	bool canRotate = true;

	public float onScale = 2.0f;
	public float offScale = 0.75f;

	void Start () {

		handleParts[0] = transform.Find("Handle/HandlePart0");
		handleParts[1] = transform.Find("Handle/HandlePart1");

		var rotateComponent = GetComponent<RotateComponent>();

		rotateComponent.onCanRotateChange.AddListener( () => {
			canRotate = rotateComponent.canRotate;
		});

	}

	void Update () {

		var y = canRotate ? onScale : offScale;
		var localScale = handleParts[0].localScale;
		localScale.y = Mathf.Lerp( localScale.y, y, Time.deltaTime*10.0f);
		handleParts[0].localScale = localScale;
		handleParts[1].localScale = localScale;

	}
}
