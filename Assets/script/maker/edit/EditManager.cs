using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditManager : MonoBehaviour {

  public Button RemoveBlockBtn;
  public Button RotateBlockBtn;
  public Button ReplaceBtn;
  public GameObject SelectBtns;
  public GameObject ReplaceBtns;
  public RotateController rotateController;
  public Transform rotateComponentHolder;
  public GameObject AddAndSelectButtons;

  bool isRotating = false;
  bool isDragging = false;

  Plane dragPlane;

  Renderer target;
  Transform targetParent;
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

    ClearTarget ();

  }

  void OnRotationStart () {

    allButtonsState = allButtons.ConvertAll (b => b.interactable);
    foreach (var btn in allButtons) {
      btn.interactable = false;
    }
    isRotating = true;

  }

  void OnRotationEnd () {

    for (int i = 0; i < allButtons.Count; i++) {

      allButtons[i].interactable = allButtonsState[i];

    }

    manager.EditBlock (target.transform, targetClone.transform);
    target.enabled = true;
    targetClone = null;
    isRotating = false;

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
    targetParent = target.transform.parent;
    targetOriginalColor = target.material.color;
    target.material.color = Color.grey;
    RemoveBlockBtn.interactable = true;
    RotateBlockBtn.interactable = true;
    ReplaceBtn.interactable = true;
    AddAndSelectButtons.SetActive (false);
    ReplaceBtns.SetActive (false);

  }

  void OnEnable () {

    ShowSelectUi ();

    rotateController.onRotationStart.AddListener (OnRotationStart);

    rotateController.onRotationDone.AddListener (OnRotationEnd);

  }

  void OnDisable () {

    ClearTarget ();
    if (SelectBtns) SelectBtns.SetActive (false);

    rotateController.onRotationStart.RemoveListener (OnRotationStart);

    rotateController.onRotationDone.RemoveListener (OnRotationEnd);

  }

  void ClearTarget () {

    if (target) {

      target.material.color = targetOriginalColor;
      target.transform.parent = targetParent;
      target.enabled = true;
      target = null;

    }

    if (targetClone) {

      GameObject.Destroy (targetClone.gameObject);
      targetClone = null;

    }

    if (RemoveBlockBtn) {

      RemoveBlockBtn.interactable = false;
      RotateBlockBtn.interactable = false;
      ReplaceBtn.interactable = false;

    }

    if (rotateController) rotateController.gameObject.SetActive (false);

    isRotating = false;
    isDragging = false;

    ShowSelectUi ();

  }

  public void RemoveBlock () {

    if (target) manager.RemoveBlock (target.transform);
    ClearTarget ();

  }

  void ShowRotationUi () {

    targetClone = GameObject.Instantiate (target, target.transform.position, target.transform.rotation);
    target.enabled = false;

    rotateController.transform.position = targetClone.transform.position;
    targetClone.transform.parent = rotateComponentHolder;
    rotateController.gameObject.SetActive (true);

  }

  void ShowReplaceUi () {

    ReplaceBtns.SetActive (true);
    SelectBtns.SetActive (false);
    AddAndSelectButtons.SetActive (false);

  }

  void ShowSelectUi () {

    if (SelectBtns) SelectBtns.SetActive (true);
    if (ReplaceBtns) ReplaceBtns.SetActive (false);
    if (AddAndSelectButtons) AddAndSelectButtons.SetActive (true);

  }

  void StartDrag (Vector2 touchPos) {

    if (rotateController.gameObject.activeSelf) return;

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