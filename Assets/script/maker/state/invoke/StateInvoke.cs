using UnityEngine;
using UnityEngine.UI;

public abstract class StateInvoke<T> : MonoBehaviour {

  public T stateToInvoke;

  protected abstract MakerStateType StateType {
    get;
  }

  MakerStateManager manager;

  void Start () {

    manager = GameObject.FindObjectOfType<MakerStateManager> ();

    var btn = gameObject.GetComponent<Button> ();
    if (btn != null) {
      btn.onClick.AddListener (() => Invoke ());
    }

  }

  public void Invoke () {
    manager.SetState (StateType, stateToInvoke);
  }

}