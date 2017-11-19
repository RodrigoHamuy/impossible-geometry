// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class TransitionComponent : MonoBehaviour {

	TargetComponent target;

	Animator fadeAnim;

	// Use this for initialization
	void Start () {

		target = GameObject.FindObjectOfType <TargetComponent> ();
		fadeAnim = GetComponentInChildren<Animator> ();
		target.onTargetReached.AddListener( onTargetReached );

	}

	void onTargetReached() {
		fadeAnim.SetTrigger("onLevelComplete");
	}
}
