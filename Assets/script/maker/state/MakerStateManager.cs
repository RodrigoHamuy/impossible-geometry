using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MakerStateManager : MonoBehaviour {

  List<State> states;

#pragma warning disable 0414

  [SerializeField]
  string currentState = "";

#pragma warning restore 0414

  void Awake () {

    var makerStates = System.Enum.GetNames (typeof (MakerState)).ToList ();
    var typeId = System.Enum.GetName (typeof (MakerStateType), MakerStateType.General);

    states = EnumsToStates<MakerState, MakerStateType> (MakerStateType.General);

    states.Concat (
      EnumsToStates<MenuState, MakerStateType> (MakerStateType.Menu)
    );

    states.Concat (
      EnumsToStates<EditorState, MakerStateType> (MakerStateType.EditMode)
    );

    SetState (MakerState.MakerEdit);
    // SetState (MenuState.MenuClose);

  }

  public void SetState<T> (T state) {

    var currState = states.Find (s => s.Enable);

    var stateId = EnumToString (state);

    var newState = states.Find (s => s.Id == stateId);

    if (currState != newState) {

      if (currState != null) {
        currState.Exit ();
      }

    } else return;

    newState.Enter ();

    currentState = newState.Id;

  }

  public State GetState <T>(T state) {

    return states.Find (s => s.Id == EnumToString (state));

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