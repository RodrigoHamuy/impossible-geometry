using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.UI;

enum MultiSelectState {
  SelectBlocks,
  ChooseCenter,
  AddHandler
}

[RequireComponent (typeof (TouchComponent))]
public class MultiSelect : MonoBehaviour {

  public Transform rotateCenter;
  public Button AddRotationBtn;

  List<Transform> blocks = new List<Transform> ();
  List<Color> blocksColor = new List<Color> ();
  MultiSelectState state;

  void Start () {

    var touchComponent = GetComponent<TouchComponent> ();

    touchComponent.onTouchStart.AddListener (MoveCenter);
    touchComponent.onTouchMove.AddListener (MoveCenter);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    AddRotationBtn.onClick.AddListener (() => {
      SetState (MultiSelectState.ChooseCenter);
    });

  }

  void OnEnable () {

    SetState (MultiSelectState.SelectBlocks);

  }

  void OnDisable () {

    rotateCenter.gameObject.SetActive (false);
    ClearAll ();

  }

  void InitChooseCenter () {

    rotateCenter.gameObject.SetActive (true);

    var pos = Vector3.zero;

    foreach (var b in blocks) {

      pos += b.position;

    }

    pos = pos / blocks.Count;

    rotateCenter.position = pos;

  }

  void Select (Vector2 touchPos) {

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (!block) return;

    else if (blocks.Exists (b => b == block)) {

      var i = blocks.IndexOf (block);
      var color = blocksColor[i];
      var renderer = block.GetComponent<Renderer> ();

      renderer.material.color = color;

      blocks.RemoveAt (i);
      blocksColor.RemoveAt (i);

    } else {

      blocks.Add (block);
      var renderer = block.GetComponent<Renderer> ();
      blocksColor.Add (renderer.material.color);
      renderer.material.color = Color.grey;

    }

    AddRotationBtn.interactable = blocks.Count > 0;

  }

  void OnTouchEnd (Vector2 touchPos) {

    if (state == MultiSelectState.SelectBlocks) Select (touchPos);

    if (state == MultiSelectState.ChooseCenter) MoveCenter (touchPos);

  }

  void ClearAll () {

    for (int i = 0; i < blocks.Count; i++) {

      var b = blocks[i];
      if (!b) continue;
      var renderer = b.GetComponent<Renderer> ();
      renderer.material.color = blocksColor[i];

    }

    blocks.Clear ();
    blocksColor.Clear ();

  }

  void SetState (MultiSelectState state) {

    this.state = state;

    switch (state) {

      case MultiSelectState.SelectBlocks:
        ClearAll ();
        rotateCenter.gameObject.SetActive (false);
        AddRotationBtn.interactable = false;
        state = MultiSelectState.SelectBlocks;
        break;

      case MultiSelectState.ChooseCenter:
        InitChooseCenter ();
        break;

    }

  }

  void MoveCenter (Vector2 touchPos) {

    if (state != MultiSelectState.ChooseCenter) return;

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (!blocks.Exists (b => b == block)) return;

    rotateCenter.transform.position = block.position;

  }
}