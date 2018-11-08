using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditManager : MonoBehaviour {

  public RotateController rotateController;
  public Transform rotateComponentHolder;
  public GameObject canvas;

  public UnityEventBool OnTargetChange;

  public UnityEvent OnRotateUIEnable;

  public Transform[] replacePrefabs;

  bool isRotating = false;
  bool isDragging = false;

  Plane dragPlane;

  Renderer target;
  Transform targetParent;
  Renderer targetClone;
  MakerActionsManager manager;
  MakerStateManager stateManager;
  Color targetOriginalColor;
  TouchComponent touchComponent;

  List<Button> allButtons;
  List<bool> allButtonsState;

  void Replace (GameObject prefab) {

    var block = manager.ReplaceBlock (prefab.transform, target.transform);
    ClearTarget ();
    Select (block);

  }

  void Start () {

    allButtons = GameObject.FindObjectsOfType<Button> ().ToList ();
    manager = GameObject.FindObjectOfType<MakerActionsManager> ();
    touchComponent = GetComponent<TouchComponent> ();
    stateManager = GameObject.FindObjectOfType<MakerStateManager> ();

    touchComponent.onTouchStart.AddListener (StartDrag);
    touchComponent.onTouchMove.AddListener (MoveDrag);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    rotateController.onRotationStart.AddListener (OnRotationStart);
    rotateController.onRotationDone.AddListener (OnRotationEnd);

    stateManager.OnPrefabMenuShow.AddListener (OnPrefabMenuShow);

    // stateManager.OnPrefabSelect.AddListener ();
    // stateManager.OnAxisSelect.AddListener();

    ClearTarget ();

  }

  void OnPrefabMenuShow (Transform menuContainer) {

    if (!gameObject.activeInHierarchy) return;

    var menuItems = menuContainer.gameObject.GetComponentsInChildren<TransformEventEmitter> ();

    foreach (var item in menuItems) {

      item.gameObject.SetActive (replacePrefabs.Contains (item.element));

    }

  }

  void AddRotationAxisListener (Toggle btn, int axis) {

    btn.onValueChanged.AddListener (value => {

      if (!value) return;
      var vector = Vector3.zero;
      vector[axis] = 1;
      SetRotationAxis (vector);

    });

  }

  public void SetRotationAxis (Vector3 vector) {

    if (targetClone) targetClone.transform.parent = null;
    rotateController.transform.up = vector;
    if (targetClone) targetClone.transform.parent = rotateComponentHolder;

  }

  void OnRotationStart () {

    allButtonsState = allButtons.ConvertAll (b => b.interactable);
    foreach (var btn in allButtons) {
      btn.interactable = false;
    }
    canvas.SetActive (false);
    isRotating = true;

  }

  void OnRotationEnd () {

    canvas.SetActive (true);

    for (int i = 0; i < allButtons.Count; i++) {

      allButtons[i].interactable = allButtonsState[i];

    }

    manager.EditBlock (target.transform, targetClone.transform);
    target.enabled = true;
    targetClone = null;
    isRotating = false;

    ShowRotationUi ();

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

    OnTargetChange.Invoke (true);

  }

  void OnDisable () {

    ClearTarget ();

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

    if (rotateController) rotateController.gameObject.SetActive (false);

    isRotating = false;
    isDragging = false;

    OnTargetChange.Invoke (false);

  }

  public void RemoveBlock () {

    if (target) manager.RemoveBlock (target.transform);
    ClearTarget ();

  }

  public void ShowRotationUi () {

    targetClone = GameObject.Instantiate (target, target.transform.position, target.transform.rotation);
    target.enabled = false;

    rotateController.transform.position = targetClone.transform.position;
    targetClone.transform.parent = rotateComponentHolder;
    rotateController.gameObject.SetActive (true);
    OnRotateUIEnable.Invoke ();

  }

  void StartDrag (Vector2 touchPos) {

    if (rotateController.gameObject.activeSelf) return;

    if (!target) return;

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (!block) return;

    if (block.transform != target.transform) return;

    canvas.SetActive (false);

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

    canvas.SetActive (true);

    isDragging = false;

  }

  void updateDragPosition (Vector2 touchPos) {

    var ray = Camera.main.ScreenPointToRay (touchPos);

    float dist;

    if (!dragPlane.Raycast (ray, out dist)) return;

    targetClone.transform.position = ray.origin + ray.direction * dist;

  }

}