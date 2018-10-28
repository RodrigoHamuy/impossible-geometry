using Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour {

  public RotateController rotateController;
  public Transform rotateComponentHolder;
  public Transform world;

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

  }

  void OnEnable () {

    rotateController.onRotationStart.AddListener (OnRotationStart);

    rotateController.onRotationDone.AddListener (OnRotationEnd);

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

  }

  void ShowRotationUi () {

    originalQuaternion = target.transform.rotation;

    rotateController.transform.position = target.transform.position;
    world.parent = rotateComponentHolder;
    rotateController.gameObject.SetActive (true);

  }

}