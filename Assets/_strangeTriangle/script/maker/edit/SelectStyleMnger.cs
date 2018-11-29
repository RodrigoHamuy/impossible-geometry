using System.Collections.Generic;
using UnityEngine;

public class SelectStyleMnger {

  Transform target;
  Transform parent;
  Vector3 startPos;

  Color[] targetOriginalColors;
  Renderer[] targetRenderers;

  Color selectColor = Color.gray;

  public void Select (Transform target) {

    this.target = target;

    parent = target.parent;

    targetRenderers = target.GetComponentsInChildren<Renderer> ();
    targetOriginalColors = new Color[targetRenderers.Length];

    startPos = target.position;

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

  public Transform GetTarget () {
    return target;
  }

  public Transform GetParent () {
    return parent;
  }

  public void SetParent (Transform parent) {
    this.parent = parent;
  }

  public void SyncParent () {
    target.parent = parent;
  }

  public Vector3 GetStartPos () {
    return startPos;
  }

  public void SyncStartPos () {
    startPos = target.position;
  }

}