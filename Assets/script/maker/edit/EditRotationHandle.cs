using Generic;
using UnityEngine;

[RequireComponent (typeof (EditManager))]
[RequireComponent (typeof (TouchComponent))]

public class EditRotationHandle : MonoBehaviour {

  EditManager editManager;

  bool selectMode = false;

  void Start () {

    editManager = GetComponent<EditManager> ();
    var stateManager = GameObject.FindObjectOfType<MakerStateManager> ();
    var touchComponent = GetComponent<TouchComponent> ();

    stateManager.OnEditHandleClick.AddListener (OnEditHandleClick);

    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

  }

  void OnEditHandleClick () {

    print ("choose affected blocks");
    selectMode = true;

  }

  void OnTouchEnd (Vector2 touchPos) {

    if (!selectMode) return;

  }

}