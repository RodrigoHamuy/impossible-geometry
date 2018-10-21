using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SpawnerModeEvent : UnityEvent<SpawnerMode> { }

public class SpawnerModeEmitter : MonoBehaviour {

  public SpawnerMode mode;

  public SpawnerModeEvent onTrigger;

  public void OnTrigger () {

    onTrigger.Invoke (mode);

  }

}