using UnityEngine;

public class StateObserver<T> : MonoBehaviour {

  public T state;

  State observableState;

  void Start () {

    var manager = GameObject.FindObjectOfType<MakerStateManager> ();

    observableState = manager.GetState (state);

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