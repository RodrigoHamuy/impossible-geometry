using UnityEngine.Events;
using UnityEngine;

public class RotateHandleComponent : MonoBehaviour {

	public UnityEvent onMouseDown = new UnityEvent();

	void OnMouseDown(){
		onMouseDown.Invoke();
	}
	
}
