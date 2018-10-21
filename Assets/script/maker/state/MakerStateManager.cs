using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MakerStateManager : MonoBehaviour {

  Dictionary<MakerStateType, List<State>> states = new Dictionary<MakerStateType, List<State>> ();

#pragma warning disable 0414

  [SerializeField]
  List<string> currentStates;

#pragma warning restore 0414

  void Awake () {

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

  public void SetState<T> (MakerStateType type, T state, bool init = false) {

    var currState = states[type].Find (s => s.Enable);

    var stateId = EnumToString (state);

    var newState = states[type].Find (s => s.Id == stateId);

    if (currState != newState) {

      if (currState != null) {
        currState.Exit ();
      }

    } else return;

    newState.Enter ();

    if (!init) UpdateStateInspector ();

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