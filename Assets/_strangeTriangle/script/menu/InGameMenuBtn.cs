using UnityEngine.SceneManagement;
// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class InGameMenuBtn : MonoBehaviour {


	public Animator fadeAnim;

	void OnMouseUp() {

		fadeAnim.SetTrigger("onLevelComplete");

		Invoke( "LoadMenu", 1 );
	}



	public void LoadMenu(){
		SceneManager.LoadScene("menu");
	}
}
