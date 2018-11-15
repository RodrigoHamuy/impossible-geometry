using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockRotator : MonoBehaviour {

  public GameObject rotateController;
  public GameObject rotateHolder;

  public bool canChangeObject = true;

  void Start () {
    rotateController.SetActive (false);
  }

  public void CanChangeObject (bool value) {
    canChangeObject = value;
  }

  public void OnTouchEnd (Vector2 touchPos) {

    // already in use
    if (!canChangeObject) return;

    if (rotateHolder.transform.childCount > 0) {
      rotateHolder.GetComponentInChildren<Transform> ().parent = null;
    }

    var block = Utility.MakerGetBlockOnTapPos (touchPos);

    if (block) {

      rotateController.SetActive (true);
      rotateController.transform.position = block.position;
      block.parent = rotateHolder.transform;

    } else {
      rotateController.SetActive (false);
    }

  }

}