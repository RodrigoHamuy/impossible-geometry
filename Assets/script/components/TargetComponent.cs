using UnityEngine.SceneManagement;
﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TargetComponent : MonoBehaviour {

	PlayerComponent player;
	public UnityEvent onTargetReached = new UnityEvent();
	public string nextScene;

	void Start () {

		player = GameObject.FindObjectOfType <PlayerComponent> ();
		player.onTargetReached.AddListener( isNextTarget );

	}

	void isNextTarget(){
		var dist = player.transform.position - transform.position;
		if ( dist.sqrMagnitude < 1.5f ) {
			Debug.Log("Level Target reached: " + dist.sqrMagnitude);
			onTargetReached.Invoke();
			StartCoroutine( LoadNextLevel() );
		}
	}

	IEnumerator LoadNextLevel() {
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene("level03");
	}


}
