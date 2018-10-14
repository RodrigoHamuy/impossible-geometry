using Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour {

  public Button RemoveBlockBtn;
  public Button RotateBlockBtn;

  Renderer target;
  MakerActionsManager manager;
  Color targetOriginalColor;
  TouchComponent touchComponent;

  void Start () {

    manager = GameObject.FindObjectOfType<MakerActionsManager> ();
    touchComponent = GetComponent<TouchComponent> ();
    touchComponent.onTouchEnd.AddListener (Select);
    RemoveBlockBtn.onClick.AddListener (RemoveBlock);
    ClearTarget ();

  }

  void Select (Vector2 touchPos) {

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (block) {

      ClearTarget ();

      target = block.GetComponent<Renderer> ();
      targetOriginalColor = target.material.color;
      target.material.color = Color.grey;
      RemoveBlockBtn.interactable = true;
      RotateBlockBtn.interactable = true;

    }

  }

  void OnDisable () {

    ClearTarget ();

  }

  void ClearTarget () {

    if (target) target.material.color = targetOriginalColor;
    target = null;
    RemoveBlockBtn.interactable = false;
    RotateBlockBtn.interactable = false;

  }

  public void RemoveBlock () {

    if (target) manager.RemoveBlock (target.transform);
    ClearTarget ();

  }

}