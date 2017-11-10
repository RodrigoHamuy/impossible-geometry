// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class RotateComponent : MonoBehaviour {

	bool isRotated = false;

	public UnityEvent afterRotate = new UnityEvent();

	public void Rotate(){
		isRotated = !isRotated;
		if( isRotated ){
			transform.eulerAngles = new Vector3( 0, 0, 90 );
		}else{
			transform.eulerAngles = Vector3.zero;
		}
		afterRotate.Invoke();
	}
}
