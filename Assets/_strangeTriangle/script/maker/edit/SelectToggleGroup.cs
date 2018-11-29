using UnityEngine;
using UnityEngine.UI;

public class SelectToggleGroup : MonoBehaviour {

  EditManager editManager;
  Toggle[] toggles;

  void Start () {

    Init ();

  }

  void OnEnable () {

    Init ();

    foreach (var toggle in toggles) {
      toggle.isOn = false;
    }

  }

  void Init () {

    editManager = GameObject.FindObjectOfType<EditManager> ();
    toggles = GetComponentsInChildren<Toggle> ();

    if (!editManager) return;

    editManager.OnTargetChange.RemoveListener (OnTargetChange);
    editManager.OnTargetChange.AddListener (OnTargetChange);

  }

  void OnTargetChange (bool value) {

    foreach (var toggle in toggles) {
      toggle.interactable = value;
      if (!value) toggle.isOn = false;
    }

  }

}