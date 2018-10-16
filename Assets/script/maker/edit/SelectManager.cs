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

  Renderer target;
  MakerActionsManager manager;
  Color targetOriginalColor;
  TouchComponent touchComponent;

  List<Button> allButtons;
  List<bool> allButtonsState;

  public void Replace(GameObject value) {

    var block = manager.ReplaceBlock(value.transform, target.transform);
    ClearTarget ();
    Select(block);
    ShowSelectUi();

  }

  void Start () {

    allButtons = GameObject.FindObjectsOfType<Button> ().ToList ();
    manager = GameObject.FindObjectOfType<MakerActionsManager> ();
    touchComponent = GetComponent<TouchComponent> ();
    touchComponent.onTouchEnd.AddListener (Select);
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

  void Select (Vector2 touchPos) {

    if (isRotating) return;

    ClearTarget ();

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (block) Select(block);

  }

  void Select(Transform block){

    target = block.GetComponent<Renderer> ();
    targetOriginalColor = target.material.color;
    target.material.color = Color.grey;
    RemoveBlockBtn.interactable = true;
    RotateBlockBtn.interactable = true;
    ReplaceBtn.interactable = true;

  }

  void OnEnable () {

    ShowSelectUi();
    
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
    RemoveBlockBtn.interactable = false;
    RotateBlockBtn.interactable = false;
    ReplaceBtn.interactable = false;
    rotateComponent.gameObject.SetActive (false);

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

  void ShowReplaceUi() {

    SelectBtns.SetActive(false);
    ReplaceBtns.SetActive(true);

  }

  void ShowSelectUi() {

    SelectBtns.SetActive(true);
    ReplaceBtns.SetActive(false);

  }

}