using UnityEngine;

public class Vector3Emmiter : MonoBehaviour {

  public Vector3 value;

  public UnityEventVector3 emmiter;

  public void Emit () {
    emmiter.Invoke (value);
  }

}