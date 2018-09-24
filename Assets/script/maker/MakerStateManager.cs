using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MakerStateManager : MonoBehaviour {

  public UnityEventMakerState OnStateChange;

	public MakerState state;

	public void SetState(MakerState state){

		if(this.state == state) return;
		this.state = state;

		OnStateChange.Invoke(state);

	}

}
