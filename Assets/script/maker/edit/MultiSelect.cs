using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.UI;

enum MultiSelectState {
  SelectBlocks,
  AddCenter,
  AddHandler
}

[RequireComponent (typeof (TouchComponent))]
public class MultiSelect : MonoBehaviour {

  public List<Transform> focusModeButtons;

  public Transform world;
  public Transform rotateCenter;
  public Button AddRotationBtn;
  public Button OkBtn;

  public Toggle[] rotationToogleBtns;

  public Transform rotateComponentPrefab;
  public Transform handlePrefab;

  List<Transform> blocks = new List<Transform> ();
  List<Color> blocksColor = new List<Color> ();
  MultiSelectState state;
  Transform handle;
  Transform rotateComponent;

  void Start () {

    var touchComponent = GetComponent<TouchComponent> ();

    touchComponent.onTouchStart.AddListener (MoveCenter);
    touchComponent.onTouchMove.AddListener (MoveCenter);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    AddRotationBtn.onClick.AddListener (() => {
      SetState (MultiSelectState.AddCenter);
    });

    OkBtn.onClick.AddListener (() => {

      switch (state) {
        case MultiSelectState.AddCenter:
          SetState (MultiSelectState.AddHandler);
          break;
        case MultiSelectState.AddHandler:
          SetState (MultiSelectState.SelectBlocks);
          break;
      }

    });

    for (int i = 0; i < rotationToogleBtns.Count (); i++) {

      AddRotationAxisListener (rotationToogleBtns[i], i);

    }

  }

  void OnEnable () {

    SetState (MultiSelectState.SelectBlocks);

  }

  void OnDisable () {

    if (rotateCenter) rotateCenter.gameObject.SetActive (false);
    ClearAll ();

  }

  void AddRotationAxisListener (Toggle btn, int axis) {

    btn.onValueChanged.AddListener (value => {

      if (!value) return;
      var vector = Vector3.zero;
      vector[axis] = 1;
      SetRotationAxis (vector);

    });

  }

  void SetRotationAxis (Vector3 vector) {

    switch (state) {

      case MultiSelectState.AddCenter:
        var obj = rotateCenter.Find ("RotationAxis");
        if (obj.up == vector) vector *= -1f;
        obj.up = vector;
        break;

      case MultiSelectState.AddHandler:
        if (handle.up == vector) vector *= -1f;
        handle.up = vector;
        handle.position = handle.parent.position + vector * .5f;
        break;
    }
  }

  void InitChooseCenter () {

    foreach (var btn in rotationToogleBtns) {
      btn.gameObject.SetActive (true);
    }

    rotationToogleBtns[1].GetComponent<Toggle> ().isOn = true;

    foreach (var btn in focusModeButtons) {
      btn.gameObject.SetActive (false);
    }

    OkBtn.gameObject.SetActive (true);

    var okBtnText = OkBtn.GetComponentInChildren<Text> ();
    okBtnText.text = "Set Center";

    rotateCenter.gameObject.SetActive (true);

    var pos = Vector3.zero;

    foreach (var b in blocks) {

      pos += b.position;

    }

    pos = pos / blocks.Count;

    blocks = blocks.OrderBy (b => (b.position - pos).sqrMagnitude).ToList ();

    rotateCenter.position = blocks[0].position;

  }

  void InitSelectBlocks () {

    foreach (var btn in rotationToogleBtns) {
      btn.gameObject.SetActive (false);
    }

    foreach (var btn in focusModeButtons) {
      btn.gameObject.SetActive (true);
    }
    ClearAll ();
    OkBtn.gameObject.SetActive (false);
    rotateCenter.gameObject.SetActive (false);
    AddRotationBtn.interactable = false;
    state = MultiSelectState.SelectBlocks;

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

    else if (state == MultiSelectState.AddCenter) MoveCenter (touchPos);

    else if (state == MultiSelectState.AddHandler) MoveHandler (touchPos);

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
        InitSelectBlocks ();
        break;

      case MultiSelectState.AddCenter:
        InitChooseCenter ();
        break;

      case MultiSelectState.AddHandler:
        InitAddHandler ();
        break;

    }

  }

  void InitAddHandler () {

    var okBtnText = OkBtn.GetComponentInChildren<Text> ();
    okBtnText.text = "Add Rotation Handle";

    rotateComponent = GameObject.Instantiate (
      rotateComponentPrefab,
      rotateCenter.position,
      rotateCenter.rotation, world
    );
    var rotateComp = rotateComponent.GetComponent<RotateTouchEmitter> ();
    rotateComp.enabled = false;
    rotateComp.transform.up = rotateCenter.Find ("RotationAxis").up;

    handle = GameObject.Instantiate (handlePrefab, rotateCenter.position + Vector3.up * .5f, rotateCenter.rotation, world);
    var handleRotationController = handle.gameObject.AddComponent<RotateController> ();
    handleRotationController.AddRotateTouchEmitter (rotateComp);
    handle.parent = blocks[0];

    foreach (var b in blocks) {
      b.parent = rotateComponent;
    }

    var rotateController = rotateComponent.GetComponent<RotateTouchEmitter> ();
    rotateController.enabled = false;
    rotateController.handleCollider = handle.GetComponentInChildren<Collider> ();

  }

  void MoveCenter (Vector2 touchPos) {

    if (state != MultiSelectState.AddCenter) return;

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (!blocks.Exists (b => b == block)) return;

    rotateCenter.transform.position = block.position;

  }

  void MoveHandler (Vector2 touchPos) {

    if (state != MultiSelectState.AddHandler) return;

    if (!handle) return;

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (!block) return;

    handle.position = block.position + handle.up * .5f;
    handle.parent = block;

  }

}