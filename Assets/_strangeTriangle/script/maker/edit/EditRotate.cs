using Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (EditManager))]

public class EditRotate : MonoBehaviour {

  public RotateController rotateController;

  public Transform rotateCenter;

  GameObject canvas;

  MakerActionsManager actionsManager;
  EditManager editManager;
  TouchComponent touchComponent;

  [HideInInspector]
  public bool isRotating = false;
  [HideInInspector]
  public bool isActive = false;

  bool chooseCenterMode = false;
  bool hasCustomPivot = false;

  void Start () {

    canvas = GameObject.FindObjectOfType<Canvas> ().gameObject;
    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();
    var stateManager = GameObject.FindObjectOfType<MakerStateManager> ();

    editManager = GetComponent<EditManager> ();

    rotateController.onRotationStart.AddListener (OnRotationStart);
    rotateController.onRotationDone.AddListener (OnRotationEnd);

    stateManager.OnAxisSelect.AddListener (SetRotationAxis);

    touchComponent = GetComponent<TouchComponent> ();
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

  }

  public void OnChooseCenterToggle (bool isOn) {

    if (isOn) {

      rotateCenter.position = rotateController.transform.position;
      rotateCenter.rotation = rotateController.transform.rotation;
      AttachBlocksToRotate (false);
      rotateCenter.gameObject.SetActive (true);
      rotateController.gameObject.SetActive (false);
      chooseCenterMode = true;
      editManager.UpdatePlane (rotateCenter.position);
      hasCustomPivot = true;

    } else {

      rotateController.transform.position = rotateCenter.position;
      rotateCenter.gameObject.SetActive (false);
      rotateController.gameObject.SetActive (true);
      AttachBlocksToRotate (true);
      chooseCenterMode = false;

    }

  }

  public void ShowRotationUi () {

    UpdateRotaionPivot ();
    AttachBlocksToRotate (true);

    rotateController.gameObject.SetActive (true);
    isActive = true;

  }

  public void SetRotationAxis (Vector3 vector) {

    if (!gameObject.activeInHierarchy) return;

    AttachBlocksToRotate (false);

    rotateController.transform.up = vector;

    AttachBlocksToRotate (true);

  }

  public void ClearTarget () {

    if (rotateController) rotateController.gameObject.SetActive (false);

  }

  void OnDisable () {

    isActive = false;
    hasCustomPivot = false;

    if (rotateCenter) rotateCenter.gameObject.SetActive (false);

  }

  void OnTouchEnd (Vector2 touchPos) {
    if (!chooseCenterMode) return;
    SetRotationCenter (touchPos);
  }

  void SetRotationCenter (Vector2 touchPos) {

    Vector3 pos;
    var block = Utility.MakerGetBlockOnTapPos (touchPos);

    if (block) {

      pos = block.position;

    } else {

      var hit = false;

      pos = editManager.RaycastPlane (touchPos, out hit);
      if (!hit) return;

    }

    pos = Utility.Round (pos, 1.0f);

    rotateCenter.position = pos;

  }

  void UpdateRotaionPivot () {

    if (hasCustomPivot) {

      return;

    }

    var center = Vector3.zero;

    foreach (var item in editManager.selected) {
      var target = item.GetTarget ();
      center += target.position;
    }

    rotateController.transform.position = Utility.Round (center / editManager.selected.Count, 1.0f);

  }

  void AttachBlocksToRotate (bool attach) {

    foreach (var item in editManager.selected) {
      item.GetTarget ().parent = attach ? rotateController.transform : item.GetParent ();
    }

  }

  void OnRotationStart () {

    canvas.SetActive (false);
    isRotating = true;

  }

  void OnRotationEnd () {

    foreach (var item in editManager.selected) {
      actionsManager.EditBlock (item.GetTarget ());
    }

    isRotating = false;

    ShowRotationUi ();

    canvas.SetActive (true);

  }

}