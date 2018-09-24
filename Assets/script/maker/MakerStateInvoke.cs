using UnityEngine;

public class MakerStateInvoke : MonoBehaviour {

  public MakerState stateToInvoke;

  public UnityEventMakerState OnStateInvoke;

  public void Invoke(){
    OnStateInvoke.Invoke(stateToInvoke);
  }

}