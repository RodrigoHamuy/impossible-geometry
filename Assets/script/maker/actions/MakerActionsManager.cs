using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MakerActionsManager : MonoBehaviour {

  public List<MakerAction> actions = new List<MakerAction> ();

  public UnityEvent2Vector3 OnBlockAdded;
  public UnityEvent2Vector3 OnBlockRemoved;
  public UnityEventInt OnBlockAmountChange;
  public UnityEvent OnHistoryClear;

  public List<Transform> blockHistory = new List<Transform> ();

  void Start () {

    OnBlockAdded.AddListener ((v1, v2) => {
      OnBlockAmountChange.Invoke (blockHistory.Count);
    });

    OnBlockRemoved.AddListener ((v1, v2) => {
      OnBlockAmountChange.Invoke (blockHistory.Count);
    });

    OnBlockAmountChange.AddListener (amount => {
      if (amount == 0) OnHistoryClear.Invoke ();
    });

    OnBlockAmountChange.Invoke (blockHistory.Count);

  }

  public void Undo () {

    var action = actions[actions.Count - 1];
    actions.RemoveAt (actions.Count - 1);

    switch (action.type) {
      case MakerActionType.Add:
        RemoveLastBlock ();
        break;
      case MakerActionType.Remove:
        RestoreBlock (action);
        break;
      case MakerActionType.Edit:
        RestoreBlockEdition (action);
        break;
    }

  }

  public void RemoveBlock (Transform block) {

    var i = blockHistory.IndexOf (block);
    actions.Add (new MakerAction (
      MakerActionType.Remove,
      block,
      block.position,
      block.localScale,
      block.rotation,
      i
    ));
    blockHistory.RemoveAt (i);
    GameObject.Destroy(block.gameObject);
    OnBlockRemoved.Invoke (block.position, block.transform.position);

  }

  Transform RemoveLastBlock () {

    var lastBlock = blockHistory[blockHistory.Count - 1];
    blockHistory.RemoveAt (blockHistory.Count - 1);
    lastBlock.gameObject.SetActive (false);
    OnBlockRemoved.Invoke (lastBlock.position, lastBlock.transform.position);

    return lastBlock;

  }

  void RestoreBlock (MakerAction action) {

    action.target.gameObject.SetActive (true);
    blockHistory.Insert (action.historyIndex, action.target);

  }

  public Transform ReplaceBlock (Transform prefab, Transform target) {

    var block = AddBlock (prefab, target.position);

    RemoveBlock (target);

    return block;

  }

  public Transform AddBlock (Transform prefab, Vector3 position, bool restore = false, int index = 0) {

    var block = Instantiate (prefab, position, Quaternion.identity);

    if (restore) {
      blockHistory.Insert (index, block);
    } else {
      blockHistory.Add (block);
      actions.Add (new MakerAction (
        MakerActionType.Add,
        block,
        block.position,
        block.localScale,
        block.rotation,
        blockHistory.Count - 1
      ));
    }
    OnBlockAdded.Invoke (position, Vector3.zero);
    return block;

  }

  public void EditBlock (Transform target, Transform placeholder) {

    target.transform.position = placeholder.transform.position;
    target.transform.rotation = placeholder.transform.rotation;
    target.transform.localScale = placeholder.transform.localScale;

    var i = blockHistory.IndexOf (target);

    actions.Add (new MakerAction (
      MakerActionType.Edit,
      target,
      target.position,
      target.localScale,
      target.rotation,
      i
    ));

    GameObject.Destroy (placeholder.gameObject);

  }

  void RestoreBlockEdition(MakerAction action){

    var lastPosition = actions.FindLast( a => a.target == action.target);
    
    action.target.transform.position = lastPosition.position;
    action.target.transform.rotation = lastPosition.rotation;
    action.target.transform.localScale = lastPosition.scale;

  }

}