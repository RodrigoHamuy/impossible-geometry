using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour {

  public Button RemoveBlockBtn;
  public Button RotateBlockBtn;
  public Button ReplaceBtn;
  public GameObject SelectBtns;
  public GameObject ReplaceBtns;
  public RotateComponent rotateComponent;
  public Transform rotateComponentHolder;

  bool isRotating = false;
  bool isDragging = false;

  Plane dragPlane;

  Renderer target;
  Renderer targetClone;
  MakerActionsManager manager;
  Color targetOriginalColor;
  TouchComponent touchComponent;

  List<Button> allButtons;
  List<bool> allButtonsState;

  public void Replace (GameObject value) {

    var block = manager.ReplaceBlock (value.transform, target.transform);
    ClearTarget ();
    Select (block);
    ShowSelectUi ();

  }

  void Start () {

    allButtons = GameObject.FindObjectsOfType<Button> ().ToList ();
    manager = GameObject.FindObjectOfType<MakerActionsManager> ();
    touchComponent = GetComponent<TouchComponent> ();

    touchComponent.onTouchStart.AddListener (StartDrag);
    touchComponent.onTouchMove.AddListener (MoveDrag);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    RemoveBlockBtn.onClick.AddListener (RemoveBlock);
    RotateBlockBtn.onClick.AddListener (ShowRotationUi);
    ReplaceBtn.onClick.AddListener (ShowReplaceUi);

    rotateComponent.onRotationStart.AddListener (() => {

      allButtonsState = allButtons.ConvertAll (b => b.interactable);
      foreach (var btn in allButtons) {
        btn.interactable = false;
      }
      isRotating = true;

    });

    rotateComponent.onRotationDone.AddListener (() => {

      for (int i = 0; i < allButtons.Count; i++) {

        allButtons[i].interactable = allButtonsState[i];

      }

      isRotating = false;

    });

    ClearTarget ();

  }

  void OnTouchEnd (Vector2 touchPos) {

    if (isDragging) EndDrag (touchPos);

    else Select (touchPos);

  }

  void Select (Vector2 touchPos) {

    if (isRotating) return;

    ClearTarget ();

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (block) Select (block);

  }

  void Select (Transform block) {

    target = block.GetComponent<Renderer> ();
    targetOriginalColor = target.material.color;
    target.material.color = Color.grey;
    RemoveBlockBtn.interactable = true;
    RotateBlockBtn.interactable = true;
    ReplaceBtn.interactable = true;

  }

  void OnEnable () {

    ShowSelectUi ();

  }

  void OnDisable () {

    ClearTarget ();

  }

  void ClearTarget () {

    if (target) {

      target.material.color = targetOriginalColor;
      target.transform.parent = null;

    }

    target = null;

    if (RemoveBlockBtn) {

      RemoveBlockBtn.interactable = false;
      RotateBlockBtn.interactable = false;
      ReplaceBtn.interactable = false;

    }

    if (rotateComponent) rotateComponent.gameObject.SetActive (false);

  }

  public void RemoveBlock () {

    if (target) manager.RemoveBlock (target.transform);
    ClearTarget ();

  }

  void ShowRotationUi () {

    rotateComponent.transform.position = target.transform.position;
    target.transform.parent = rotateComponentHolder;
    rotateComponent.gameObject.SetActive (true);

  }

  void ShowReplaceUi () {

    SelectBtns.SetActive (false);
    ReplaceBtns.SetActive (true);

  }

  void ShowSelectUi () {

    SelectBtns.SetActive (true);
    ReplaceBtns.SetActive (false);

  }

  void StartDrag (Vector2 touchPos) {

    if (!target) return;

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (!block) return;

    if (block.transform != target.transform) return;

    isDragging = true;

    dragPlane = new Plane (Vector3.up, block.position);

    targetClone = GameObject.Instantiate (target, target.transform.position, target.transform.rotation);

    target.enabled = false;

  }

  void MoveDrag (Vector2 touchPos) {

    if (!isDragging) return;

    updateDragPosition (touchPos);

  }

  void EndDrag (Vector2 touchPos) {

    updateDragPosition (touchPos);

    var pos = targetClone.transform.position;

    for (var i = 0; i < 3; i++) {

      pos[i] = Mathf.Round (pos[i]);

    }

    targetClone.transform.position = pos;

    manager.EditBlock (target.transform, targetClone.transform);

    target.enabled = true;

    targetClone = null;

    isDragging = false;

  }

  void updateDragPosition (Vector2 touchPos) {

    var ray = Camera.main.ScreenPointToRay (touchPos);

    float dist;

    if (!dragPlane.Raycast (ray, out dist)) return;

    targetClone.transform.position = ray.origin + ray.direction * dist;

  }

}