using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MakerStateManager : MonoBehaviour {

	//#region MakerState

	public GameObject[] playModeObjects;
	public GameObject[] editModeObjects;

	public UnityEventMakerState OnStateChange;

	public MakerState state;

	//#endregion

	//#region EditorState

	public GameObject[] BrushModeObjects;
	public GameObject[] PrismModeObjects;
	public GameObject[] RotateModeObjects;

	public EditorState editorState;

	//#endregion

	void Start () {
		OnStateUpdate ();
	}

	public void SetState (MakerState state) {

		if (this.state == state) return;
		this.state = state;

		OnStateUpdate ();

	}

	public void SetState (EditorState state) {

		if (editorState == state) return;
		editorState = state;

		OnEditorStateUpdate ();

	}

	void OnStateUpdate () {

		OnStateChange.Invoke (state);

		switch (state) {
			case MakerState.Play:
				SetAllActive (playModeObjects, true);
				SetAllActive (editModeObjects, false);
				break;
			case MakerState.Editor:
				SetAllActive (playModeObjects, false);
				SetAllActive (editModeObjects, true);
				break;
		}

	}

	void OnEditorStateUpdate () {

		switch (editorState) {
			case EditorState.Brush:
				SetAllActive (BrushModeObjects, true);
				SetAllActive (PrismModeObjects, false);
				SetAllActive (RotateModeObjects, false);
				break;
			case EditorState.Prism:
				SetAllActive (BrushModeObjects, false);
				SetAllActive (PrismModeObjects, true);
				SetAllActive (RotateModeObjects, false);
				break;
			case EditorState.Rotate:
				SetAllActive (BrushModeObjects, false);
				SetAllActive (PrismModeObjects, false);
				SetAllActive (RotateModeObjects, true);
				break;
		}

	}

	void SetAllActive (GameObject[] objs, bool value) {
		foreach (var item in objs) {
			item.SetActive (value);
		}
	}

}