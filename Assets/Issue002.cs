using UnityEngine.SceneManagement;
﻿using UnityEngine;
using System.Collections.Generic;
// using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class Issue002 {

	[UnityTest]
	public IEnumerator Issue002_000_ShouldNotFind() {
		yield return LoadScene("issue-002.000");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}

	[UnityTest]
	public IEnumerator Issue002_001_ShouldNotFind() {
		yield return LoadScene("issue-002.001");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}


	[UnityTest]
	public IEnumerator Issue002_002_ShouldNotFind() {
		yield return LoadScene("issue-002.002");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}

	[UnityTest]
	public IEnumerator Issue002_003_ShouldFind() {
		yield return LoadScene("issue-002.003");
		Assert.IsTrue( HasPath(), "Should find a path." );
	}

	[UnityTest]
	public IEnumerator Issue002_004Passes_ShoulNeed5Steps() {
		yield return LoadScene("issue-002.004");
		Assert.AreEqual( 5, GetPath().Count, "Should need 5 steps." );
	}

	[UnityTest]
	public IEnumerator Issue002_005_ShouldNotFind() {
		yield return LoadScene("issue-002.005");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}

	[UnityTest]
	public IEnumerator Issue002_006_ShouldNotFind() {
		yield return LoadScene("issue-002.006");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}

	[UnityTest]
	public IEnumerator Issue005_000_ShouldNotFind() {
		yield return LoadScene("issue-005.000");
		Assert.IsFalse( HasPath(), "Should not find a path." );
	}

	[UnityTest]
	public IEnumerator Issue005_001_ShouldFind() {
		yield return LoadScene("issue-005.001");
		Assert.IsTrue( HasPath(), "Should find a path." );
	}

	[UnityTest]
	public IEnumerator Issue009_000_ShouldFind() {
		yield return LoadScene("issue-009.000");
		Assert.IsTrue( HasPath(), "Should find a path." );
	}

	[UnityTest]
	public IEnumerator Issue009_001_ShouldFind() {
		yield return LoadScene("issue-009.001");
		Assert.IsTrue( HasPath(), "Should find a path." );
	}

	IEnumerator LoadScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
		yield return null;
	}

	bool HasPath(){

		PathFinder pathFinder = new PathFinder();

		var player = GameObject.Find("Player").GetComponent<PlayerComponent>();
		Assert.IsNotNull(player, "Player not found.");
		var touchPoint = GameObject.Find("TouchPoint");
		Assert.IsNotNull(touchPoint, "TouchPoint not found.");

		var touchPointPos = Camera.main.WorldToScreenPoint(touchPoint.transform.position);

		var pathFound = pathFinder.MovePlayerTo(
			player.transform.position,
			touchPointPos,
			player.controller.normal
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
