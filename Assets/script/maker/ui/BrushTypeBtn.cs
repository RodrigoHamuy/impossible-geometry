using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BrushTypeBtn : MonoBehaviour {

  public Vector3 normal;

  AddOnHold manager;

  Toggle toggle;

  void Start () {

    manager = GameObject.FindObjectOfType<AddOnHold> ();
    toggle = GetComponent<Toggle> ();
    toggle.onValueChanged.AddListener (OnValueChanged);

  }

  void OnEnable () {

    UpdateState ();

  }

  public void OnValueChanged (bool value) {

    if (!value) return;

    manager.planeNormal = normal;

  }

  void UpdateState () {

    if (!manager) {

      manager = GameObject.FindObjectOfType<AddOnHold> ();
      toggle = GetComponent<Toggle> ();

    }

    toggle.isOn = manager.planeNormal == normal;

  }

}