// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuLevelItem : MonoBehaviour {

	public string level;
	public Animator fadeAnim;

	bool loadingLevel = false;

	void OnMouseUp() {

		if (! Utility.canPlayerMove) return;

		if( loadingLevel ) return;

		if ( ! isFacing() ) return;

		OnTap();

	}

	bool isFacing(){
		var cam = Camera.main;

		return Vector3.Dot( cam.transform.forward, transform.forward ) > 0.1f;
	}

	public void OnTap(){
		loadingLevel = true;

		fadeAnim.SetTrigger("onLevelComplete");

		Invoke( "LoadLevel", 1 );

	}

	void LoadLevel(){
		SceneManager.LoadScene(level);
		Utility.canPlayerMove = true;
	}
}
