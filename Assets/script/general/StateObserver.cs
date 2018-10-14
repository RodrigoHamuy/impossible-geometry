using UnityEngine;

public abstract class StateObserver<T> : MonoBehaviour {

  public T state;

  State observableState;

  protected abstract MakerStateType StateType {
    get;
  }

  void Start () {

    var manager = GameObject.FindObjectOfType<MakerStateManager> ();

    observableState = manager.GetState (StateType, state);

    observableState.OnChange.AddListener (OnChange);
    OnChange ();

  }

  void OnChange () {

    OnChange (observableState.Enable);

  }

  void OnChange (bool value) {

    gameObject.SetActive (value);

  }

}