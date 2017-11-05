using UnityEngine.SceneManagement;
﻿using UnityEngine;
using System.Collections.Generic;
// using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Issue002 {

	[UnityTest]
	public IEnumerator Issue002_000Passes() {
		yield return LoadScene("issue-002.000");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}

	[UnityTest]
	public IEnumerator Issue002_001Passes() {
		yield return LoadScene("issue-002.001");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}


	[UnityTest]
	public IEnumerator Issue002_002Passes() {
		yield return LoadScene("issue-002.002");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}

	[UnityTest]
	public IEnumerator Issue002_003Passes() {
		yield return LoadScene("issue-002.003");
		Assert.IsTrue( HasPath(), "Should find a path." );
	}

	[UnityTest]
	public IEnumerator Issue002_004Passes() {
		yield return LoadScene("issue-002.004");
		Assert.AreEqual( 5, GetPath().Count, "Should need 5 steps." );
	}

	[UnityTest]
	public IEnumerator Issue005_000Passes() {
		yield return LoadScene("issue-005.000");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}

	IEnumerator LoadScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
		yield return null;
	}

	bool HasPath(){

		PathFinder pathFinder = new PathFinder();

		var player = GameObject.Find("Player");
		Assert.IsNotNull(player, "Player not found.");
		var touchPoint = GameObject.Find("TouchPoint");
		Assert.IsNotNull(touchPoint, "TouchPoint not found.");

		var touchPointPos = Camera.main.WorldToScreenPoint(touchPoint.transform.position);

		var pathFound = pathFinder.MovePlayerTo(
			player.transform.position,
			touchPointPos
		);

		return pathFound;

	}

	List<PathPoint> GetPath(){

		PathFinder pathFinder = new PathFinder();

		var player = GameObject.Find("Player");
		Assert.IsNotNull(player, "Player not found.");
		var touchPoint = GameObject.Find("TouchPoint");
		Assert.IsNotNull(touchPoint, "TouchPoint not found.");

		var touchPointPos = Camera.main.WorldToScreenPoint(touchPoint.transform.position);

		var pathFound = pathFinder.MovePlayerTo(
			player.transform.position,
			touchPointPos
		);

		Assert.IsTrue( pathFound, "Should find a path." );

		return pathFinder.path;

	}
}
