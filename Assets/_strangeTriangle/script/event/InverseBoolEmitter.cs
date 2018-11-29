using UnityEngine;

public class InverseBoolEmitter : MonoBehaviour {

  public UnityEventBool emmiter;

  public void Emit (bool value) {
    emmiter.Invoke (!value);
  }

}