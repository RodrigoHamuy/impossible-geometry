using System.Collections;
using UnityEngine.SceneManagement;﻿
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetComponent : MonoBehaviour {

	public UnityEvent onTargetReached = new UnityEvent ();
	public string nextScene;

	PlayerComponent player;

	void Start () {

		player = GameObject.FindObjectOfType<PlayerComponent> ();
		player.onTargetReached.AddListener (isNextTarget);

	}

	void isNextTarget () {
		var dist = player.transform.position - transform.position;
		if (dist.sqrMagnitude < 1.5f) {
			Debug.Log ("Level Target reached: " + dist.sqrMagnitude);
			onTargetReached.Invoke ();
			if (nextScene != "") StartCoroutine (LoadNextLevel ());
		}
	}

	IEnumerator LoadNextLevel () {
		yield return new WaitForSeconds (1.5f);
		SceneManager.LoadScene (nextScene);
	}

}