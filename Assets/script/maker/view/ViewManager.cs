using Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ViewManager : MonoBehaviour {

  public RotateController rotateController;
  public Transform rotateComponentHolder;
  public Transform world;
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

    touchComponent.onTouchStart.AddListener (OnTouchStart);
    touchComponent.onTouchMove.AddListener (OnTouchMove);
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    rotateController.onRotationStart.AddListener (OnRotationStart);
    rotateController.onRotationDone.AddListener (OnRotationEnd);

    for (int i = 0; i < rotationToogleBtns.Count (); i++) {

      AddRotationAxisListener (rotationToogleBtns[i], i);

    }

  }

  void OnDisable () {

    ClearTarget ();

    rotateController.onRotationStart.RemoveListener (OnRotationStart);

    rotateController.onRotationDone.RemoveListener (OnRotationEnd);

  }

  void OnTouchStart (Vector2 touchPos) {

    worldStartPos = world.position;

    plane = new Plane (Vector3.up, transform.position);
    var ray = Camera.main.ScreenPointToRay (touchPos);
    float dist;
    if (plane.Raycast (ray, out dist)) {
      dragStartPos = ray.origin + ray.direction * dist;
    }

  }

  void OnTouchMove (Vector2 touchPos) {

    if(isRotating) return;

    var ray = Camera.main.ScreenPointToRay (touchPos);
    float dist;
    if (plane.Raycast (ray, out dist)) {
      dragCurrPos = ray.origin + ray.direction * dist;
      world.position = worldStartPos + dragCurrPos - dragStartPos;
    }

  }

  void OnRotationStart () {

    isRotating = true;

  }

  void OnRotationEnd () {

    isRotating = false;

  }

  void OnTouchEnd (Vector2 touchPos) {

    if(isRotating) return;

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

  void SetRotationAxis(Vector3 vector) {
    world.parent = null;
    rotateController.transform.up = vector;
    world.parent = rotateComponentHolder;

  }

}