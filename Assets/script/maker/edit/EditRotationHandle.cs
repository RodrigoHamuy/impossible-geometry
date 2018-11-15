using System.Collections.Generic;
using Generic;
using UnityEngine;

[RequireComponent (typeof (EditManager))]
[RequireComponent (typeof (TouchComponent))]

public class EditRotationHandle : MonoBehaviour {

  public Transform rotateCenterView;

  Transform world;

  EditManager editManager;

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
    var stateManager = GameObject.FindObjectOfType<MakerStateManager> ();
    var touchComponent = GetComponent<TouchComponent> ();

    stateManager.OnEditHandleClick.AddListener (OnEditHandleClick);
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
  }

  void OnSelectCenterModeClick () {
    print ("choose center");
    mode = EditRotationHandleMode.ChooseCenter;
    rotateCenterView.gameObject.SetActive (true);
    SyncRotateCenterView ();
  }

  void SyncRotateCenterView () {
    rotateCenterView.position = rotateContainer.position;
    rotateCenterView.rotation = rotateContainer.rotation;
  }

  void OnEditHandleClick () {

    isActive = true;

    var blockData = editManager.target.GetComponent<EditableBlock> ();

    if (blockData.rotateController) {

      rotateContainer = blockData.rotateController;

      foreach (Transform child in rotateContainer.transform) {

        var style = new SelectStyleMnger ();
        style.SetColor (affectedBlocksColor);
        style.Select (child);
        selectStyleManager.Add (style);
        affectedBlocks.Add (child);

      }

    } else {

      rotateContainer = new GameObject ().transform;

      rotateContainer.gameObject
        .AddComponent<RotateController> ()
        .AddRotateTouchEmitter (
          editManager.target.GetComponentInChildren<RotateTouchEmitter> ()
        );

      rotateContainer.position = editManager.target.position;
      rotateContainer.parent = world;
      rotateContainer.rotation = editManager.target.rotation;

      blockData.rotateController = rotateContainer;

    }

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

    selectStyleManager.Add (style);

    affectedBlocks.Add (block);

  }

}