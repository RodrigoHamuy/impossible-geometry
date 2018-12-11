using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakerCreateTutorial : MonoBehaviour {

  public GameObject ObjectivePrefab;

  public TutorialObjective[] TutorialObjective;

  public GameObject IntroScreen;

  // First row container
  public GameObject Row1Container;

  // Main Buttons group
  public GameObject MainBtnsGroup;

  // First row btns

  public GameObject BackBtn;
  public GameObject UndoBtn;
  public GameObject ReDoBtn;
  public GameObject PlayBtn;
  public GameObject AddBtn;
  public GameObject SelectBtn;
  public GameObject ViewBtn;

  MakerActionsManager actionsManager;

  void Awake () {

    if (
      LevelMakerConfig.Data == null ||
      !LevelMakerConfig.Data.LevelName.Contains ("tutorial_create_")
    ) {
      enabled = false;
      return;
    }

    InitObjectivesUI ();

    IntroScreen.SetActive (true);

  }

  void Start () {

    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();

    IntroScreen.GetComponent<Button> ().onClick.AddListener (FadeIntro);

    actionsManager.OnSceneChange.AddListener (CheckBlocks);

    Invoke ("HideBtns", .1f);

  }

  void InitObjectivesUI () {

    var objectivesUI = IntroScreen.transform.Find ("Objectives");

    foreach (Transform child in objectivesUI) {

      GameObject.Destroy (child.gameObject);

    }

    foreach (var objectiveData in TutorialObjective) {

      var objective = Instantiate (ObjectivePrefab, Vector3.zero, Quaternion.identity, objectivesUI);

      objective.transform.Find ("Icon").GetComponent<Image> ().sprite = objectiveData.icon;
      var textMesh = objective.transform.GetComponentInChildren<TextMeshPro> ();
      if (textMesh) textMesh.SetText (objectiveData.text);

    }

  }

  void CheckBlocks () {

    var hasPlayer = actionsManager.blocksInScene.Exists (a => a.tag == MakerTags.Player);
    var hasBlocks = actionsManager.blocksInScene.Exists (a => a.tag == MakerTags.Cube);
    var hasTarget = actionsManager.blocksInScene.Exists (a => a.tag == MakerTags.Target);

    if (hasPlayer && hasBlocks && hasTarget) {
      PlayBtn.GetComponent<RectTransform> ().sizeDelta = new Vector2 (30.0f, 30.0f);
    }

  }

  void HideBtns () {

    Row1Container.GetComponent<LayoutGroup> ().enabled = false;
    Row1Container.GetComponent<ContentSizeFitter> ().enabled = false;

    MainBtnsGroup.GetComponent<LayoutGroup> ().enabled = false;
    MainBtnsGroup.GetComponent<ContentSizeFitter> ().enabled = false;

    var btns = new List<GameObject> () {
      BackBtn,
      // UndoBtn,
      ReDoBtn,
      PlayBtn,
      SelectBtn,
      ViewBtn
    };

    foreach (var btn in btns) {

      var rect = btn.GetComponent<RectTransform> ();
      rect.sizeDelta = Vector3.zero;

    }

  }

  void FadeIntro () {

    iTween.FadeTo (IntroScreen, new Hashtable () {
      {
        "alpha",
        0
      }, {
        "time",
        .3f
      }, {
        "oncomplete",
        "RemoveIntro"
      }, {
        "oncompletetarget",
        gameObject
      }
    });

    // var childA = IntroScreen.transform.GetChild (0).GetComponent<RectTransform> ();

    // iTween.MoveAdd (childA.gameObject, new Vector3 (childA.position.x * 2.0f, 0, 0), 1);

    // var childB = IntroScreen.transform.GetChild (1).GetComponent<RectTransform> ();

    // iTween.MoveAdd (childB.gameObject, -new Vector3 (childA.position.x * 2.0f, 0, 0), 1);

  }

  void RemoveIntro () {

    IntroScreen.SetActive (false);

  }

}