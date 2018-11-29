// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransitionComponent : MonoBehaviour {

	public UnityEvent levelStartTransitionStart = new UnityEvent ();
	public UnityEvent levelCompleteTransitionStart = new UnityEvent ();

	TargetComponent target;

	// Use this for initialization
	void Start () {

		target = GameObject.FindObjectOfType<TargetComponent> ();
		target.onTargetReached.AddListener (onTargetReached);

		levelStartTransitionStart.Invoke();

	}

	void onTargetReached () {
		levelCompleteTransitionStart.Invoke ();
	}
}