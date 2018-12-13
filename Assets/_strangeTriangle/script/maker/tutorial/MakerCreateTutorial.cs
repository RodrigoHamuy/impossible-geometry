using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakerCreateTutorial : MonoBehaviour {

  public GameObject ObjectivePrefab;

  public TutorialObjective[] TutorialObjectiveList;

  public GameObject ShowObjectivesBtn;

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

  Canvas canvas;
  MakerActionsManager actionsManager;
  Transform objectivesUI;
  RectTransform objectiveBadge;

  bool hasBlocks;
  bool hasPlayer;
  bool hasTarget;

  void Awake () {

    if (
      LevelMakerConfig.Data == null ||
      !LevelMakerConfig.Data.LevelName.Contains ("tutorial_create_")
    ) {
      enabled = false;
      return;
    }

    canvas = GameObject.FindObjectOfType<Canvas> ();

    ShowObjectivesBtn.GetComponent<Button> ().onClick.AddListener (OnShowObjectivesClick);

    InitObjectivesUI ();

    IntroScreen.SetActive (true);

  }

  void Start () {

    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();

    IntroScreen.GetComponent<Button> ().onClick.AddListener (FadeIntro);

    actionsManager.OnSceneChange.AddListener (CheckBlocks);

    CheckBlocks ();

    Invoke ("HideBtns", .1f);

  }

  void InitObjectivesUI () {

    objectivesUI = IntroScreen.transform.Find ("Objectives");

    foreach (var objectiveData in TutorialObjectiveList) {

      var objective = Instantiate (ObjectivePrefab, Vector3.zero, Quaternion.identity, objectivesUI);

      SetObjectiveUI (objective.transform, objectiveData);

    }

    objectiveBadge = Instantiate (ObjectivePrefab, Vector3.zero, Quaternion.identity, canvas.transform).GetComponent<RectTransform> ();
    objectiveBadge.anchoredPosition = Vector2.zero;

  }

  void SetObjectiveUI (Transform view, TutorialObjective model) {

    view.Find ("Icon").GetComponent<Image> ().sprite = model.icon;
    var textMesh = view.GetComponentInChildren<TMP_Text> ();
    textMesh.SetText (model.text);

  }

  void CheckBlocks () {

    hasBlocks = CheckObjective (MakerTags.Cube, hasBlocks, 0, TutorialObjectiveList[0]);
    hasPlayer = CheckObjective (MakerTags.Player, hasPlayer, 1, TutorialObjectiveList[1]);
    hasTarget = CheckObjective (MakerTags.Target, hasTarget, 2, TutorialObjectiveList[2]);

  }

  void ShowCompletedTaskBadge () {

    iTween.ValueTo (objectiveBadge.gameObject, new Hashtable () {
      {
        "from",
        objectiveBadge.anchoredPosition
      }, {
        "to",
        new Vector2 (objectiveBadge.rect.width, 0)
      }, {
        "time",
        .5f
      }, {
        "onupdatetarget",
        gameObject
      }, {
        "onupdate",
        "TweenBadge"
      }, {
        "oncomplete",
        "OnTweenSlideInDone"
      }, {
        "oncompletetarget",
        gameObject
      }
    });

  }

  void TweenBadge (Vector2 pos) {

    objectiveBadge.GetComponent<RectTransform> ().anchoredPosition = pos;

  }

  void OnTweenSlideInDone () {

    iTween.ValueTo (objectiveBadge.gameObject, new Hashtable () {
      {
        "from",
        objectiveBadge.GetComponent<RectTransform> ().anchoredPosition
      }, {
        "to",
        Vector2.zero
      }, {
        "time",
        .5f
      }, {
        "onupdatetarget",
        gameObject
      }, {
        "onupdate",
        "TweenBadge"
      }, {
        "delay",
        2
      }
    });

  }

  bool CheckObjective (string tagName, bool oldHas, int index, TutorialObjective objective) {

    var newHas = actionsManager.blocksInScene.Exists (a => a.tag == tagName);

    if (!oldHas && newHas) {

      SetObjectiveUI (objectiveBadge, objective);
      ShowCompletedTaskBadge ();
    }

    objectivesUI.GetChild (index).Find ("CheckBox/Empty").GetComponent<Image> ().color = newHas ? new Color (255, 255, 255, 0) : Color.white;
    objectivesUI.GetChild (index).Find ("CheckBox/Checked").GetComponent<Image> ().color = newHas ? Color.white : new Color (255, 255, 255, 0);

    return newHas;

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

  void OnShowObjectivesClick () {

    IntroScreen.SetActive (true);

  }

}