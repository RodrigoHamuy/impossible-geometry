using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour {

  public RotateController rotateController;
  public Transform rotateComponentHolder;
  public Transform world;
  public GameObject canvas;
  public Toggle[] rotationToogleBtns;

  bool isRotating = false;
  Renderer target;
  Color targetOriginalColor;

  Quaternion originalQuaternion;

  Vector3 worldStartPos;
  Vector3 dragStartPos;
  Vector3 dragCurrPos;

  Plane plane;

  void Start () {

    var touchComponent = GetComponent<TouchComponent> ();

    rotateController.onRotationStart.AddListener (OnRotationStart);
    rotateController.onRotationDone.AddListener (OnRotationEnd);

    touchComponent.onTouchStart.AddListener (OnTouchStart);
    touchComponent.onTouchMove.AddListener (OnTouchMove);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    for (int i = 0; i < rotationToogleBtns.Count (); i++) {

      AddRotationAxisListener (rotationToogleBtns[i], i);

    }

  }

  void OnDisable () {

    ClearTarget ();

  }

  void OnTouchStart (Vector2 touchPos) {

    if (isRotating) return;

    worldStartPos = world.position;

    plane = new Plane (Vector3.up, transform.position);
    var ray = Camera.main.ScreenPointToRay (touchPos);
    float dist;
    if (plane.Raycast (ray, out dist)) {
      dragStartPos = ray.origin + ray.direction * dist;
    }

  }

  void OnTouchMove (Vector2 touchPos) {

    if (isRotating) return;

    var ray = Camera.main.ScreenPointToRay (touchPos);
    float dist;
    if (plane.Raycast (ray, out dist)) {
      dragCurrPos = ray.origin + ray.direction * dist;
      world.position = worldStartPos + dragCurrPos - dragStartPos;
    }

  }

  void OnRotationStart () {

    isRotating = true;
    canvas.SetActive (false);

  }

  void OnRotationEnd () {

    isRotating = false;
    canvas.SetActive (true);

  }

  void OnTouchEnd (Vector2 touchPos) {

    if (isRotating) return;

    Select (touchPos);

    world.position = Utility.Round (world.position, 1.0f);

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

    ShowRotationUi ();

  }

  void ClearTarget () {

    if (target) {

      target.material.color = targetOriginalColor;
      target.enabled = true;
      target = null;
      world.parent = null;

    }

    isRotating = false;

    if (rotateController) rotateController.gameObject.SetActive (false);

    foreach (var btn in rotationToogleBtns) {
      btn.gameObject.SetActive (false);
    }

  }

  void ShowRotationUi () {

    originalQuaternion = target.transform.rotation;

    rotateController.transform.position = target.transform.position;
    world.parent = rotateComponentHolder;
    rotateController.gameObject.SetActive (true);

    foreach (var btn in rotationToogleBtns) {
      btn.gameObject.SetActive (true);
    }

    rotationToogleBtns[1].GetComponent<Toggle> ().isOn = true;

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
    world.parent = null;
    rotateController.transform.up = vector;
    world.parent = rotateComponentHolder;

  }

}