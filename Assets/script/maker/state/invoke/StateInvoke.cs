using UnityEngine;
using UnityEngine.UI;

public class StateInvoke <T>: MonoBehaviour {

  public T stateToInvoke;

  MakerStateManager manager;

  void Start () {

    manager = GameObject.FindObjectOfType<MakerStateManager> ();

    var btn = GameObject.FindObjectOfType<Button>();
    if(btn != null) {
      btn.onClick.AddListener( () => Invoke() );
    }

  }

  public void Invoke () {
    manager.SetState(stateToInvoke);
  }

}