using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (EditManager))]

public class EditRotate : MonoBehaviour {

  public RotateController rotateController;

  GameObject canvas;

  MakerActionsManager actionsManager;
  EditManager editManager;

  [HideInInspector]
  public bool isRotating = false;
  [HideInInspector]
  public bool rotationMode = false;

  void Start () {

    canvas = GameObject.FindObjectOfType<Canvas> ().gameObject;
    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();
    var stateManager = GameObject.FindObjectOfType<MakerStateManager> ();

    editManager = GetComponent<EditManager> ();

    rotateController.onRotationStart.AddListener (OnRotationStart);
    rotateController.onRotationDone.AddListener (OnRotationEnd);

    stateManager.OnAxisSelect.AddListener (SetRotationAxis);

  }

  public void ShowRotationUi () {

    MoveRotateToCenter ();
    AttachBlocksToRotate (true);

    rotateController.gameObject.SetActive (true);

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

  void MoveRotateToCenter () {

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