using System;
using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent (typeof (EditRotationHandle))]
[RequireComponent (typeof (EditRotate))]
[RequireComponent (typeof (TouchComponent))]

public class EditManager : MonoBehaviour {

  public UnityEventBool OnTargetChange;

  public MakerBlockType[] replaceBlockTypes;

  public MakerBlockType[] allBlockTypes;

  GameObject canvas;

  bool isDragging = false;

  Plane dragPlane;

  [HideInInspector]
  public Transform target;

  Transform targetParent;
  MakerActionsManager manager;
  MakerStateManager stateManager;
  TouchComponent touchComponent;

  EditRotate editRotate;
  EditRotationHandle editRotationHandle;

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
    stateManager = GameObject.FindObjectOfType<MakerStateManager> ();
    canvas = GameObject.FindObjectOfType<Canvas> ().gameObject;

    touchComponent = GetComponent<TouchComponent> ();
    editRotate = GetComponent<EditRotate> ();
    editRotationHandle = GetComponent<EditRotationHandle> ();

    touchComponent.onTouchStart.AddListener (StartDrag);
    touchComponent.onTouchMove.AddListener (MoveDrag);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    stateManager.OnPrefabMenuShow.AddListener (OnPrefabMenuShow);
    stateManager.OnPrefabSelect.AddListener (Replace);

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

  void OnTouchEnd (Vector2 touchPos) {

    if (isDragging) EndDrag (touchPos);

    else Select (touchPos);

  }

  void Select (Vector2 touchPos) {

    if (editRotate.isRotating) return;
    if (editRotationHandle.selectMode) return;

    ClearTarget ();

    var block = Utility.MakerGetBlockOnTapPos (touchPos);

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

      selectStyleManager.Deselect ();

      target = null;

    }

    editRotate.ClearTarget ();
    isDragging = false;

    OnTargetChange.Invoke (false);

  }

  public void RemoveBlock () {

    if (target) manager.RemoveBlock (target.transform);
    ClearTarget ();

  }

  void StartDrag (Vector2 touchPos) {

    if (editRotate.rotateController.gameObject.activeSelf) return;

    if (!target) return;

    var block = Utility.MakerGetBlockOnTapPos (touchPos);

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