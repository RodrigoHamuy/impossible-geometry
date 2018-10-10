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

  public void RemoveBlock (Transform block) {
    blockHistory.Remove (block);
    Destroy (block.gameObject);
    OnBlockRemoved.Invoke (block.position, block.transform.position);
  }

  public Transform RemoveLastBlock () {

    var lastBlock = blockHistory[blockHistory.Count - 1];
    blockHistory.RemoveAt (blockHistory.Count - 1);
    Destroy (lastBlock.gameObject);
    OnBlockRemoved.Invoke (lastBlock.position, lastBlock.transform.position);

    return lastBlock;

  }

  public Transform AddBlock (Transform prefab, Vector3 position) {

    var block = Instantiate (prefab, position, Quaternion.identity);
    blockHistory.Add (block);
    OnBlockAdded.Invoke (position, Vector3.zero);
    return block;

  }

}