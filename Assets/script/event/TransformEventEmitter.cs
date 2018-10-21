using UnityEngine;

public class TransformEventEmitter : MonoBehaviour {

  public Transform element;

  public UnityEventTransform OnEvent;

  public void Emit(){
    OnEvent.Invoke(element);
  }

}