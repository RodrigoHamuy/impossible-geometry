using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MakerActionsManager : MonoBehaviour {

  public Transform world;

  public List<MakerAction> actions = new List<MakerAction> ();
  public UnityEventInt OnBlockAmountChange;
  public UnityEvent OnHistoryClear;

  public List<Transform> blocksInScene = new List<Transform> ();

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

    var prefab = block.GetComponent<EditableBlock> ().blockPrefab;
    actions.Add (new MakerAction (
      MakerActionType.Remove,
      block,
      prefab,
      block.position,
      block.localScale,
      block.rotation,
      block.parent
    ));
    blocksInScene.Remove (block);
    GameObject.Destroy (block.gameObject);

  }

  void RemoveBlock (MakerAction action) {

    blocksInScene.Remove (action.target);
    GameObject.Destroy (action.target.gameObject);

  }

  public Transform AddBlock (MakerAction action, bool restore) {

    var block = Instantiate (action.prefab, action.position, action.rotation, action.parent);

    var editData = block.gameObject.AddComponent<EditableBlock> ();
    editData.blockPrefab = action.prefab;

    action.target = block;

    blocksInScene.Add (block);

    if (!restore) {
      actions.Add (action);
    }

    return block;

  }

  public Transform ReplaceBlock (Transform prefab, Transform target) {

    var addAction = new MakerAction (
      MakerActionType.Add,
      null,
      prefab,
      target.position,
      target.localScale,
      target.rotation,
      target.parent
    );

    RemoveBlock (target);

    return AddBlock (addAction, false);

  }

  public void EditBlock (Transform target, Transform placeholder) {

    target.transform.position = placeholder.transform.position;
    target.transform.rotation = placeholder.transform.rotation;
    target.transform.localScale = placeholder.transform.localScale;

    var prefab = target.GetComponent<EditableBlock> ().blockPrefab;

    actions.Add (new MakerAction (
      MakerActionType.Edit,
      target,
      prefab,
      target.position,
      target.localScale,
      target.rotation,
      target.parent
    ));

    GameObject.Destroy (placeholder.gameObject);

  }

  void RestoreBlockEdition (MakerAction action) {

    var lastPosition = actions.FindLast (a => a.prefab == action.prefab);

    action.prefab.transform.position = lastPosition.position;
    action.prefab.transform.rotation = lastPosition.rotation;
    action.prefab.transform.localScale = lastPosition.scale;

  }

}