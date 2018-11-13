using System.Collections.Generic;
using UnityEngine;

public class SelectStyleMnger {

  Transform target;

  Color[] targetOriginalColors;
  Renderer[] targetRenderers;

  Color selectColor = Color.gray;

  public void Select (Transform target) {

    this.target = target;

    targetRenderers = target.GetComponentsInChildren<Renderer> ();
    targetOriginalColors = new Color[targetRenderers.Length];

    for (int i = 0; i < targetRenderers.Length; i++) {

      targetOriginalColors[i] = targetRenderers[i].material.color;

      targetRenderers[i].material.color = selectColor;

    }

  }

  public void Deselect () {

    for (int i = 0; i < targetRenderers.Length; i++) {

      targetRenderers[i].material.color = targetOriginalColors[i];

    }

    targetRenderers = null;
    targetOriginalColors = null;
  }

  public void SetColor (Color color) {

    selectColor = color;

  }

}