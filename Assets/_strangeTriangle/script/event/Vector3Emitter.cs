using UnityEngine;

public class Vector3Emitter : MonoBehaviour {

  public Vector3 value;

  public UnityEventVector3 emmiter;

  public void Emit () {
    emmiter.Invoke (value);
  }

}