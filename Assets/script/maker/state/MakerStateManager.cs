using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MakerStateManager : MonoBehaviour {

  public UnityEventVector3 OnAxisSelect;
  public UnityEventTransform OnPrefabSelect;
  public UnityEventTransform OnPrefabMenuShow;
  public UnityEvent OnEditHandleClick;
  public UnityEvent OnSelectAffectedBlocksModeClick;
  public UnityEvent OnSelectCenterModeClick;

  public Transform world;

  GameObject canvas;

  Dictionary<MakerStateType, List<State>> states = new Dictionary<MakerStateType, List<State>> ();

#pragma warning disable 0414

  [SerializeField]
  List<string> currentStates;

#pragma warning restore 0414

  void Awake () {

    canvas = GameObject.FindObjectOfType<Canvas> ().gameObject;

    var makerStates = System.Enum.GetNames (typeof (MakerState)).ToList ();
    var typeId = System.Enum.GetName (typeof (MakerStateType), MakerStateType.General);

    states.Add (
      MakerStateType.General,
      EnumsToStates<MakerState, MakerStateType> (MakerStateType.General)
    );

    states.Add (
      MakerStateType.Menu,
      EnumsToStates<MenuState, MakerStateType> (MakerStateType.Menu)
    );

    states.Add (
      MakerStateType.EditMode,
      EnumsToStates<EditorState, MakerStateType> (MakerStateType.EditMode)
    );

    SetState (MakerStateType.General, MakerState.MakerEdit, true);
    SetState (MakerStateType.Menu, MenuState.MenuClose, true);
    SetState (MakerStateType.EditMode, EditorState.EditorAdd, true);
    UpdateStateInspector ();

  }

  public void EmitAxis (Vector3 axis) {
    OnAxisSelect.Invoke (axis);
  }

  public void EmitPrefab (Transform prefab) {
    OnPrefabSelect.Invoke (prefab);
  }

  public void EmitPrefabMenuShow (Transform container) {
    OnPrefabMenuShow.Invoke (container);
  }

  public void EmitOnEditHandleClick () {
    OnEditHandleClick.Invoke ();
  }

  public void EmitOnSelectAffectedBlocksModeClick () {
    OnSelectAffectedBlocksModeClick.Invoke ();
  }

  public void EmitOnSelectCenterModeClick () {
    OnSelectCenterModeClick.Invoke ();
  }

  public void SetState<T> (MakerStateType type, T state, bool init = false) {

    var currState = states[type].Find (s => s.Enable);

    var stateId = EnumToString (state);

    var newState = states[type].Find (s => s.Id == stateId);

    if (currState != newState) {

      if (currState != null) {

        if (currState.Id == EnumToString (MakerState.MakerPlay)) {

          ExitPlayMode ();

        }

        currState.Exit ();
      }

    } else return;

    if (newState.Id == EnumToString (MakerState.MakerPlay)) {

      EnterPlayMode ();

    }

    newState.Enter ();

    if (!init) UpdateStateInspector ();

  }

  void EnterPlayMode () {

    var player = world.GetComponentInChildren<PlayerComponent> ();

    if (player) {
      player.onStartMoving.AddListener (HideCanvas);
      player.onTargetReached.AddListener (ShowCanvas);
    }

    var rotators = world.GetComponentsInChildren<RotateTouchEmitter> ();
    var rotations = world.GetComponentsInChildren<RotateController> ();
    foreach (var r in rotators) {
      r.enabled = true;
    }
    foreach (var r in rotations) {
      r.Init ();
      r.enabled = true;
      r.onRotationStart.AddListener (HideCanvas);
      r.onRotationDone.AddListener (ShowCanvas);
    }

  }

  void ExitPlayMode () {

    var player = world.GetComponentInChildren<PlayerComponent> ();

    if (player) {

      player.onStartMoving.RemoveListener (HideCanvas);
      player.onTargetReached.RemoveListener (ShowCanvas);

      var playerStart = GameObject.FindWithTag ("maker.player").GetComponent<CapsuleCollider> ();
      player.transform.position = playerStart.transform.position + playerStart.transform.rotation * playerStart.center * .5f;
      player.transform.rotation = playerStart.transform.rotation;
    }

    var rotators = world.GetComponentsInChildren<RotateTouchEmitter> ();
    var rotations = world.GetComponentsInChildren<RotateController> ();
    foreach (var r in rotators) {
      r.enabled = false;
    }
    foreach (var r in rotations) {
      r.enabled = false;
      r.onRotationStart.RemoveListener (HideCanvas);
      r.onRotationDone.RemoveListener (ShowCanvas);
    }

  }

  void HideCanvas () {
    canvas.SetActive (false);
  }

  void ShowCanvas () {
    canvas.SetActive (true);
  }

  void UpdateStateInspector () {

    currentStates.Clear ();

    for (int i = 0; i < states.Count (); i++) {

      var state = states.ElementAt (i);

      var stateString = System.Enum.GetName (typeof (MakerStateType), state.Key);

      stateString += ": " + state.Value.Find (s => s.Enable).Id;

      currentStates.Add (stateString);

    }

  }

  public State GetState<T> (MakerStateType type, T state) {

    return states[type].Find (s => s.Id == EnumToString (state));

  }

  static List<State> EnumsToStates<S, T> (T type) {

    var makerStates = System.Enum.GetNames (typeof (S)).ToList ();
    var typeId = System.Enum.GetName (typeof (T), type);

    return makerStates.ConvertAll (s => new State (s, typeId));

  }

  static string EnumToString<T> (T value) {
    return System.Enum.GetName (typeof (T), value);
  }

}