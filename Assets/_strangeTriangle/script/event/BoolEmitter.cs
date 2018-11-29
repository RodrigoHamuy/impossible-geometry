using UnityEngine;

public class BoolEmitter : MonoBehaviour {

  public bool value;

  public UnityEventBool emmiter;

  public void Emit () {
    emmiter.Invoke (value);
  }

}