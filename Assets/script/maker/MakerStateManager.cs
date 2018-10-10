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
	MakerState prevState;

	//#endregion

	//#region EditorState

	public GameObject[] BrushModeObjects;
	public GameObject[] PrismModeObjects;
	public GameObject[] RotateModeObjects;
	public GameObject[] DeleteModeObjects;

	public GameObject ComponentToRotateHolder;

	public EditorState editorState;
	EditorState prevEditorState;

	//#endregion

	void Start () {

		prevState = state;
		prevEditorState = editorState;
		OnStateUpdate ();
		OnEditorStateUpdate ();
	}

	public void SetState (MakerState newState) {

		if (state == newState) return;

		prevState = state;
		state = newState;

		OnStateUpdate ();

		if (prevState == MakerState.Editor || state == MakerState.Editor) {
			OnEditorStateUpdate ();
		}

	}

	public void SetState (EditorState newEditorState) {

		if (editorState == newEditorState) return;

		prevEditorState = editorState;
		editorState = newEditorState;

		OnEditorStateUpdate ();

	}

	void OnStateUpdate () {

		OnStateChange.Invoke (state);

		switch (state) {
			case MakerState.Play:
				SetAllActive (editModeObjects, false);
				SetAllActive (playModeObjects, true);
				break;
			case MakerState.Editor:
				SetAllActive (playModeObjects, false);
				SetAllActive (editModeObjects, true);
				break;
		}

	}

	void OnEditorStateUpdate () {

		var currPrevState = state == MakerState.Play ? editorState : prevEditorState;

		if (currPrevState == EditorState.Rotate) {

			OnRotateStateEnd ();

		}

		if (state == MakerState.Play) {

			SetAllActive (BrushModeObjects, false);
			SetAllActive (PrismModeObjects, false);
			SetAllActive (RotateModeObjects, false);
			SetAllActive (DeleteModeObjects, false);
			return;

		}

		switch (editorState) {
			case EditorState.Brush:
				SetAllActive (PrismModeObjects, false);
				SetAllActive (RotateModeObjects, false);
				SetAllActive (BrushModeObjects, true);
				SetAllActive (DeleteModeObjects, false);
				break;
			case EditorState.Prism:
				SetAllActive (BrushModeObjects, false);
				SetAllActive (RotateModeObjects, false);
				SetAllActive (PrismModeObjects, true);
				SetAllActive (DeleteModeObjects, false);
				break;
			case EditorState.Rotate:
				SetAllActive (BrushModeObjects, false);
				SetAllActive (PrismModeObjects, false);
				SetAllActive (RotateModeObjects, true);
				SetAllActive (DeleteModeObjects, false);
				break;
			case EditorState.Delete:
				SetAllActive (BrushModeObjects, false);
				SetAllActive (PrismModeObjects, false);
				SetAllActive (RotateModeObjects, false);
				SetAllActive (DeleteModeObjects, true);
				break;
		}

	}

	void SetAllActive (GameObject[] objs, bool value) {
		foreach (var item in objs) {
			item.SetActive (value);
		}
	}

	void OnRotateStateEnd () {
		var cube = ComponentToRotateHolder.GetComponentInChildren<Transform> ();
		if (cube) cube.parent = null;
	}
}