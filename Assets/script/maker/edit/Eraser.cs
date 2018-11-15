using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour {

  Renderer targetBlockRenderer;
  public MakerActionsManager actionsManager;
  Color targetBlockColor;

  public void OnTouchEnd (Vector2 touchPos) {

    var block = Utility.MakerGetBlockOnTapPos (touchPos);

    if (block) {

      var blockRenderer = block.GetComponent<Renderer> ();

      if (targetBlockRenderer == blockRenderer) {
        actionsManager.RemoveBlock (block);
      } else {
        ClearTarget ();
        targetBlockRenderer = blockRenderer;
        targetBlockColor = targetBlockRenderer.material.color;
        targetBlockRenderer.material.color = Color.grey;
      }

    }

  }

  void OnDisable () {
    ClearTarget ();
    targetBlockRenderer = null;
  }

  void ClearTarget () {
    if (targetBlockRenderer) {
      targetBlockRenderer.material.color = targetBlockColor;
    }
  }
}