using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MakerActionsManager : MonoBehaviour {

  public Transform world;

  public List<MakerAction> actions = new List<MakerAction> ();
  public UnityEventInt OnBlockAmountChange;
  public UnityEvent OnHistoryClear;

  public List<Transform> blocksInScene = new List<Transform> ();

  int idCounter = 0;

  void Start () {

    OnBlockAmountChange.AddListener (amount => {
      if (amount == 0) OnHistoryClear.Invoke ();
    });

    OnBlockAmountChange.Invoke (blocksInScene.Count);

  }

  public void Undo () {

    var action = actions[actions.Count - 1];
    actions.RemoveAt (actions.Count - 1);

    switch (action.type) {
      case MakerActionType.Add:
        RemoveBlock (action);
        break;
      case MakerActionType.Remove:
        AddBlock (action, true);
        break;
      case MakerActionType.Edit:
        RestoreBlockEdition (action);
        break;
    }

  }

  public void RemoveBlock (Transform block) {

    var blockData = block.GetComponent<EditableBlock> ();

    var blockType = block.GetComponent<EditableBlock> ().blockType;

    var action = new MakerAction (
      MakerActionType.Remove,
      block,
      blockType,
      block.position,
      block.localScale,
      block.rotation,
      block.parent
    );
    action.id = blockData.id;

    actions.Add (action);

    blocksInScene.Remove (block);
    GameObject.Destroy (block.gameObject);

  }

  void RemoveBlock (MakerAction action) {

    blocksInScene.Remove (action.target);

    var blocks = GameObject.FindObjectsOfType<EditableBlock> ();
    var block = Array.Find (blocks, b => b.id == action.id);

    GameObject.Destroy (block.gameObject);

  }

  public Transform AddBlock (MakerAction action, bool restore) {

    if (!restore && action.blockType.addOnTop) {
      action.position -= action.rotation * Vector3.up;
    }

    var block = Instantiate (
      action.blockType.prefab,
      action.position,
      action.rotation,
      action.parent
    );

    var editData = block.gameObject.AddComponent<EditableBlock> ();
    editData.blockType = action.blockType;

    action.target = block;

    blocksInScene.Add (block);

    if (!restore) {
      action.id = ++idCounter;
      actions.Add (action);
    }

    editData.id = action.id;

    return block;

  }

  public Transform ReplaceBlock (MakerBlockType blockType, Transform target) {

    var addAction = new MakerAction (
      MakerActionType.Add,
      null,
      blockType,
      target.position,
      target.localScale,
      target.rotation,
      target.parent
    );

    RemoveBlock (target);

    return AddBlock (addAction, false);

  }

  public void EditBlock (Transform target) {

    var blockType = target.GetComponent<EditableBlock> ().blockType;

    var action = new MakerAction (
      MakerActionType.Edit,
      target,
      blockType,
      target.position,
      target.localScale,
      target.rotation,
      target.parent
    );

    actions.Add (action);

    target.transform.position = action.position;
    target.transform.rotation = action.rotation;
    target.transform.localScale = action.scale;

  }

  void RestoreBlockEdition (MakerAction action) {

    var lastPosition = actions.FindLast (a => a.blockType == action.blockType);

    action.target.transform.position = lastPosition.position;
    action.target.transform.rotation = lastPosition.rotation;
    action.target.transform.localScale = lastPosition.scale;

  }

}