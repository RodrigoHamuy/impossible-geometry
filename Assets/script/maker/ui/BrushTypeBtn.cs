using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BrushTypeBtn : MonoBehaviour {

  public Vector3 normal;

  AddOnHold manager;

  List<BrushTypeBtn> btns;

  Button button;

  void Start () {

    button = GetComponent<Button> ();

    manager = GameObject.FindObjectOfType<AddOnHold> ();
    btns = GameObject.FindObjectsOfType<BrushTypeBtn> ().ToList ();

    UpdateState ();

    button.onClick.AddListener (OnSelected);

  }

  public void OnSelected () {
    manager.planeNormal = normal;
    foreach (var b in btns) {
      b.UpdateState ();
    }
  }

  void UpdateState () {

    button.interactable = !(manager.planeNormal == normal);

  }
}