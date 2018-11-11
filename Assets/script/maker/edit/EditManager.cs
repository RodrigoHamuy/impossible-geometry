using System;
using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditManager : MonoBehaviour {

  public RotateController rotateController;
  public GameObject canvas;

  public UnityEventBool OnTargetChange;

  public MakerBlockType[] replaceBlockTypes;

  public MakerBlockType[] allBlockTypes;

  bool isRotating = false;
  bool isDragging = false;

  Plane dragPlane;

  Transform target;
  Transform targetParent;
  MakerActionsManager manager;
  MakerStateManager stateManager;
  TouchComponent touchComponent;

  SelectStyleMnger selectStyleManager = new SelectStyleMnger ();

  void Replace (Transform prefab) {

    if (!gameObject.activeInHierarchy) return;

    var config = Array.Find (replaceBlockTypes, r => r.prefab == prefab);

    var block = manager.ReplaceBlock (config, target.transform);
    ClearTarget ();
    Select (block);

  }

  void Start () {

    manager = GameObject.FindObjectOfType<MakerActionsManager> ();
    touchComponent = GetComponent<TouchComponent> ();
    stateManager = GameObject.FindObjectOfType<MakerStateManager> ();

    touchComponent.onTouchStart.AddListener (StartDrag);
    touchComponent.onTouchMove.AddListener (MoveDrag);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    rotateController.onRotationStart.AddListener (OnRotationStart);
    rotateController.onRotationDone.AddListener (OnRotationEnd);

    stateManager.OnPrefabMenuShow.AddListener (OnPrefabMenuShow);
    stateManager.OnPrefabSelect.AddListener (Replace);

    // stateManager.OnPrefabSelect.AddListener ();
    // stateManager.OnAxisSelect.AddListener();

    ClearTarget ();

  }

  void OnPrefabMenuShow (Transform menuContainer) {

    if (!gameObject.activeInHierarchy) return;

    var menuItems = menuContainer.gameObject.GetComponentsInChildren<TransformEventEmitter> (true);

    var selectedBlockType = target.GetComponent<EditableBlock> ().blockType;

    foreach (var item in menuItems) {
      item.gameObject.SetActive (false);
    }

    foreach (var item in menuItems) {

      item.GetComponent<Toggle> ().isOn = item.element == selectedBlockType.prefab;

      item.gameObject.SetActive (
        Array.Exists (replaceBlockTypes, r =>
          r.prefab == item.element
        )
      );

    }

  }

  void AddRotationAxisListener (Toggle btn, int axis) {

    btn.onValueChanged.AddListener (value => {

      if (!value) return;
      var vector = Vector3.zero;
      vector[axis] = 1;
      SetRotationAxis (vector);

    });

  }

  public void SetRotationAxis (Vector3 vector) {

    if (target) target.parent = null;
    rotateController.transform.up = vector;
    if (target) target.parent = rotateController.transform;

  }

  void OnRotationStart () {

    canvas.SetActive (false);
    isRotating = true;

  }

  void OnRotationEnd () {

    manager.EditBlock (target);
    isRotating = false;

    ShowRotationUi ();

    canvas.SetActive (true);

  }

  void OnTouchEnd (Vector2 touchPos) {

    if (isDragging) EndDrag (touchPos);

    else Select (touchPos);

  }

  void Select (Vector2 touchPos) {

    if (isRotating) return;

    ClearTarget ();

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (block) Select (block);

  }

  void Select (Transform block) {

    target = block;
    targetParent = target.parent;

    selectStyleManager.Select (target);

    OnTargetChange.Invoke (true);

  }

  void OnDisable () {

    ClearTarget ();

  }

  void ClearTarget () {

    if (target) {

      target.transform.parent = targetParent;

      var targetRenderer = target.GetComponent<Renderer> ();

      selectStyleManager.Deselect ();

      target = null;

    }

    if (rotateController) rotateController.gameObject.SetActive (false);

    isRotating = false;
    isDragging = false;

    OnTargetChange.Invoke (false);

  }

  public void RemoveBlock () {

    if (target) manager.RemoveBlock (target.transform);
    ClearTarget ();

  }

  public void ShowRotationUi () {

    rotateController.transform.position = target.position;
    target.parent = rotateController.transform;
    rotateController.gameObject.SetActive (true);

  }

  void StartDrag (Vector2 touchPos) {

    if (rotateController.gameObject.activeSelf) return;

    if (!target) return;

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (!block) return;

    if (block.transform != target.transform) return;

    canvas.SetActive (false);

    isDragging = true;

    dragPlane = new Plane (Vector3.up, block.position);

  }

  void MoveDrag (Vector2 touchPos) {

    if (!isDragging) return;

    updateDragPosition (touchPos);

  }

  void EndDrag (Vector2 touchPos) {

    updateDragPosition (touchPos);

    manager.EditBlock (target.transform);

    canvas.SetActive (true);

    isDragging = false;

  }

  void updateDragPosition (Vector2 touchPos) {

    var ray = Camera.main.ScreenPointToRay (touchPos);

    float dist;

    if (!dragPlane.Raycast (ray, out dist)) return;

    target.position = ray.origin + ray.direction * dist;

  }

}