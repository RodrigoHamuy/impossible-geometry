using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleEnableEvent : MonoBehaviour {

  public UnityEvent onEnable;

  void Start () {

    var toggle = GetComponent<Toggle> ();
    toggle.onValueChanged.AddListener (OnValueChanged);

  }

  void OnValueChanged (bool value) {

    if (value) onEnable.Invoke ();

  }

}