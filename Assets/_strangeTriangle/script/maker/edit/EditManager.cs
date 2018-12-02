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

  [HideInInspector]
  public List<SelectStyleMnger> selected = new List<SelectStyleMnger> ();

  GameObject canvas;

  bool isDragging = false;

  Plane dragPlane;

  MakerActionsManager actionsManager;
  MakerStateManager stateManager;
  TouchComponent touchComponent;
  EditJoin editJoin;

  EditRotate editRotate;
  EditRotationHandle editRotationHandle;

  [HideInInspector]
  public bool selectMultiple = false;

  [HideInInspector]
  public Vector3 normal = Vector3.up;

  void Replace (Transform prefab) {

    if (!gameObject.activeInHierarchy) return;

    var config = Array.Find (replaceBlockTypes, r => r.prefab == prefab);

    var newBlocks = new List<Transform> ();

    foreach (var item in selected) {
      newBlocks.Add (
        actionsManager.ReplaceBlock (config, item.GetTarget ())
      );
    }

    ClearTargets ();

    foreach (var item in newBlocks) {
      Select (item);
    }

  }

  void Start () {

    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();
    stateManager = GameObject.FindObjectOfType<MakerStateManager> ();
    canvas = GameObject.FindObjectOfType<Canvas> ().gameObject;

    touchComponent = GetComponent<TouchComponent> ();
    editRotate = GetComponent<EditRotate> ();
    editRotationHandle = GetComponent<EditRotationHandle> ();
    editJoin = GetComponent<EditJoin> ();

    touchComponent.onTouchStart.AddListener (StartDrag);
    touchComponent.onTouchMove.AddListener (MoveDrag);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    stateManager.OnPrefabMenuShow.AddListener (OnPrefabMenuShow);
    stateManager.OnPrefabSelect.AddListener (Replace);

    ClearTargets ();

  }

  public void OnSelectMultipleChange (bool value) {
    selectMultiple = value;
  }

  void OnPrefabMenuShow (Transform menuContainer) {

    if (!gameObject.activeInHierarchy) return;

    if (selectMultiple) return;

    var target = selected[0].GetTarget ();

    var menuItems = menuContainer.gameObject.GetComponentsInChildren<TransformEventEmitter> (true);

    var selectedBlockData = target.GetComponent<EditableBlock> ().data;

    var selectedBlockPrefab = actionsManager.GetMakerBlockType (selectedBlockData.blockType);

    foreach (var item in menuItems) {
      item.gameObject.SetActive (false);
    }

    foreach (var item in menuItems) {

      item.GetComponent<Toggle> ().isOn = item.element == selectedBlockPrefab.prefab;

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

    if (editJoin.isActive) return;
    if (editRotate.isActive) return;
    if (editRotationHandle.isActive) return;

    if (!selectMultiple) ClearTargets ();

    var block = Utility.MakerGetBlockOnTapPos (touchPos);

    if (!block) return;

    Select (block);

  }

  void Select (Transform block) {

    var select = new SelectStyleMnger ();
    selected.Add (select);

    select.Select (block);

    OnTargetChange.Invoke (true);

  }

  void OnEnable () {

    selectMultiple = false;

  }

  void OnDisable () {

    ClearTargets ();

  }

  void ClearTargets () {

    editRotationHandle.ClearTarget ();

    foreach (var item in selected) {
      item.SyncParent ();
      item.Deselect ();
    }

    selected.Clear ();
    editRotate.ClearTarget ();
    isDragging = false;

    OnTargetChange.Invoke (false);

  }

  public void RemoveBlock () {

    foreach (var item in selected) {
      actionsManager.RemoveBlock (item.GetTarget ());
    }
    ClearTargets ();

  }

  Vector3 startDragPos;

  void StartDrag (Vector2 touchPos) {

    if (editRotate.isActive) return;
    if (editRotate.isRotating) return;
    if (editRotationHandle.isActive) return;

    if (editRotate.rotateController.gameObject.activeSelf) return;

    if (selected.Count == 0) return;

    var block = Utility.MakerGetBlockOnTapPos (touchPos);

    if (!block) return;

    var blocks = selected.ConvertAll (s => s.GetTarget ());

    if (!blocks.Contains (block)) return;

    canvas.SetActive (false);

    isDragging = true;

    UpdatePlane (block.position);

    startDragPos = block.position;

  }

  public void UpdatePlane (Vector3 pos) {

    dragPlane = new Plane (normal, pos);

  }

  void MoveDrag (Vector2 touchPos) {

    if (!isDragging) return;

    updateDragPosition (touchPos);

  }

  void EndDrag (Vector2 touchPos) {

    updateDragPosition (touchPos);

    foreach (var item in selected) {
      actionsManager.EditBlock (item.GetTarget ());
      item.SyncStartPos ();
    }

    canvas.SetActive (true);

    isDragging = false;

  }

  void updateDragPosition (Vector2 touchPos) {

    var hit = false;

    var pos = RaycastPlane (touchPos, out hit);

    if (!hit) return;

    var diff = pos - startDragPos;

    foreach (var item in selected) {

      item.GetTarget ().position = item.GetStartPos () + diff;

    }

  }

  public Vector3 RaycastPlane (Vector2 touchPos, out bool hit) {

    var ray = Camera.main.ScreenPointToRay (touchPos);

    float dist;

    if (!dragPlane.Raycast (ray, out dist)) {

      hit = false;
      return Vector3.zero;

    }

    hit = true;

    return ray.origin + ray.direction * dist;

  }

}