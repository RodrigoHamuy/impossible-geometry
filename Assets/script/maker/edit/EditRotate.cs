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

    rotateController.transform.position = editManager.target.position;
    editManager.target.parent = rotateController.transform;
    rotateController.gameObject.SetActive (true);

  }

  public void SetRotationAxis (Vector3 vector) {

    if (!gameObject.activeInHierarchy) return;

    if (editManager.target) editManager.target.parent = null;

    rotateController.transform.up = vector;

    if (editManager.target) editManager.target.parent = rotateController.transform;

  }

  public void ClearTarget () {

    if (rotateController) rotateController.gameObject.SetActive (false);

  }

  void OnRotationStart () {

    canvas.SetActive (false);
    isRotating = true;

  }

  void OnRotationEnd () {

    actionsManager.EditBlock (editManager.target);
    isRotating = false;

    ShowRotationUi ();

    canvas.SetActive (true);

  }

}