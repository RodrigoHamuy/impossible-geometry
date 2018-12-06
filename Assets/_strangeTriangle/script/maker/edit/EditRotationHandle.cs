using System;
using System.Collections.Generic;
using Generic;
using UnityEngine;

[RequireComponent (typeof (EditManager))]
[RequireComponent (typeof (TouchComponent))]

public class EditRotationHandle : MonoBehaviour {

  public Transform rotateCenterView;

  public MakerBlockType emptyRotateController;

  public GameObject RotateBtns;

  Transform world;

  EditManager editManager;

  MakerActionsManager actionsManager;

  [HideInInspector]
  public bool isActive = false;

  EditRotationHandleMode mode = EditRotationHandleMode.SelectAffectedBlocks;

  Color affectedBlocksColor = Color.yellow;

  List<SelectStyleMnger> selectStyleManager = new List<SelectStyleMnger> ();

  List<Transform> affectedBlocks = new List<Transform> ();

  Transform rotateContainer;

  void Start () {

    world = GameObject.FindWithTag ("world").transform;

    editManager = GetComponent<EditManager> ();
    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();

    var stateManager = GameObject.FindObjectOfType<MakerStateManager> ();
    var touchComponent = GetComponent<TouchComponent> ();

    stateManager.OnEditBtnClick.AddListener (OnEditBtnClick);
    stateManager.OnSelectAffectedBlocksModeClick.AddListener (OnSelectAffectedBlocksModeClick);
    stateManager.OnSelectCenterModeClick.AddListener (OnSelectCenterModeClick);

    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

  }

  public void ClearTarget () {

    if (rotateCenterView) rotateCenterView.gameObject.SetActive (false);

    isActive = false;

    foreach (var style in selectStyleManager) {

      style.Deselect ();

    }

    selectStyleManager.Clear ();
    affectedBlocks.Clear ();

  }

  void OnSelectAffectedBlocksModeClick () {

    print ("choose affected blocks");
    mode = EditRotationHandleMode.SelectAffectedBlocks;
    rotateCenterView.gameObject.SetActive (false);

    isActive = true;

  }

  void OnSelectCenterModeClick () {

    print ("choose center");
    mode = EditRotationHandleMode.ChooseCenter;
    rotateCenterView.gameObject.SetActive (true);
    SyncRotateCenterView ();

    var target = editManager.selected[0].GetTarget ();

    var blockData = target.GetComponent<EditableBlock> ().data;

    if (blockData.rotateControllerId != -1) {

      var allBlocks = world.GetComponentsInChildren<EditableBlock> ();

      var rotateBlock = Array.Find (allBlocks, a => a.data.id == blockData.rotateControllerId);

      rotateContainer = rotateBlock.transform;

      foreach (Transform child in rotateContainer.transform) {

        var style = new SelectStyleMnger ();
        style.SetColor (affectedBlocksColor);
        style.Select (child);
        selectStyleManager.Add (style);
        affectedBlocks.Add (child);

      }

    } else {

      rotateContainer = actionsManager.AddBlock (
        new MakerAction (
          MakerActionType.Add,
          null,
          emptyRotateController,
          target.position,
          target.localScale,
          target.rotation,
          world
        ),
        false
      );

      rotateContainer
        .GetComponent<RotateController> ()
        .AddRotateTouchEmitter (
          target.GetComponentInChildren<RotateTouchEmitter> ()
        );

      blockData.rotateControllerId = rotateContainer
        .GetComponent<EditableBlock> ().data.id;

    }

    isActive = true;

  }

  void InitRotateContainer () {
    if (!rotateContainer) {

    }
  }

  void SyncRotateCenterView () {

    if (!rotateContainer) return;

    rotateCenterView.position = rotateContainer.position;
    rotateCenterView.rotation = rotateContainer.rotation;

  }

  void SetRotateBtnsVisibility (bool value) {
    RotateBtns.SetActive (value);
  }

  void OnEditBtnClick () {

    if (editManager.selectMultiple) {

      SetRotateBtnsVisibility (false);
      return;

    }

    var target = editManager.selected[0].GetTarget ();
    var blockData = target.GetComponent<EditableBlock> ().data;

    if (blockData.blockType != "RotateHandleMaker") {

      SetRotateBtnsVisibility (false);
      return;

    }

    SetRotateBtnsVisibility (true);

  }

  void OnTouchEnd (Vector2 touchPos) {

    if (!isActive) return;

    switch (mode) {
      case EditRotationHandleMode.SelectAffectedBlocks:
        SelectBlock (touchPos);
        break;
      case EditRotationHandleMode.ChooseCenter:
        ChooseCenter (touchPos);
        break;
    }

  }

  void ChooseCenter (Vector2 touchPos) {

    var block = Utility.MakerGetBlockOnTapPos (
      touchPos,
      new string[] { "maker.cube", "maker.halfCube" }
    );

    if (!block) return;

    var childs = new List<Transform> ();

    while (rotateContainer.childCount > 0) {

      var child = rotateContainer.GetChild (0);
      child.parent = null;
      childs.Add (child);

    }

    if (rotateContainer.transform.position == block.position) {
      var up = rotateContainer.up;
      for (int i = 0; i < 3; i++) {
        if (Mathf.Abs (up[i]) + .1f > 1.0f) {
          up[(i + 1) % 3] = Mathf.Round (up[i]) * -1.0f;
          up[i] = .0f;
          break;
        }
      }
      rotateContainer.up = up;
    } else {
      rotateContainer.transform.position = block.position;
    }

    foreach (var child in childs) {
      child.parent = rotateContainer.transform;
    }

    SyncRotateCenterView ();

  }

  void SelectBlock (Vector2 touchPos) {

    var block = Utility.MakerGetBlockOnTapPos (
      touchPos,
      new string[] { "maker.cube", "maker.halfCube" }
    );

    if (block) SelectBlock (block);

  }

  void SelectBlock (Transform block) {

    var style = new SelectStyleMnger ();
    style.SetColor (affectedBlocksColor);
    style.Select (block);

    block.parent = rotateContainer;

    actionsManager.EditBlock (block);

    selectStyleManager.Add (style);

    affectedBlocks.Add (block);

  }

}