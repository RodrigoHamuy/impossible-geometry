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

  void Start () {

    var touchComponent = GetComponent<TouchComponent> ();

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

  void OnRotationStart () {

    isRotating = true;

  }

  void OnRotationEnd () {

    isRotating = false;

  }

  void OnTouchEnd (Vector2 touchPos) {

    Select (touchPos);

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