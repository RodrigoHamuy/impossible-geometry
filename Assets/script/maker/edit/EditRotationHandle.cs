using System.Collections.Generic;
using Generic;
using UnityEngine;

[RequireComponent (typeof (EditManager))]
[RequireComponent (typeof (TouchComponent))]

public class EditRotationHandle : MonoBehaviour {

  Transform world;

  EditManager editManager;

  [HideInInspector]
  public bool selectMode = false;

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

    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

  }

  void OnEditHandleClick () {

    print ("choose affected blocks");
    selectMode = true;

    rotateContainer = new GameObject ().transform;

    rotateContainer.gameObject
      .AddComponent<RotateController> ()
      .AddRotateTouchEmitter (
        editManager.target.GetComponentInChildren<RotateTouchEmitter> ()
      );

    rotateContainer.position = editManager.target.position;
    rotateContainer.parent = world;

  }

  void OnTouchEnd (Vector2 touchPos) {

    if (!selectMode) return;

    var block = Utility.MakerGetBlockOnTapPos (touchPos);

    if (block) Select (block);

  }

  void Select (Transform block) {

    var style = new SelectStyleMnger ();
    style.SetColor (affectedBlocksColor);
    style.Select (block);

    block.parent = rotateContainer;

    selectStyleManager.Add (style);

    affectedBlocks.Add (block);

    // targetParent = target.parent;

    // selectStyleManager.Select (target);

    // OnTargetChange.Invoke (true);

  }

}