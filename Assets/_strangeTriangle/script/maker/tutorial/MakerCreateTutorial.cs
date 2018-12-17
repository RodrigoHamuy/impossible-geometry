using System;
using System.Collections;
using System.Collections.Generic;
using Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class MakerCreateTutorial : MonoBehaviour {

  public GameObject ObjectivePrefab;

  public TutorialObjective[] TutorialObjectiveList;

  public TimelineAsset[] tutorialAnimations;

  public GameObject ShowObjectivesBtn;

  public GameObject IntroScreen;
  public GameObject TutorialSteps;

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

  // Add Buttons

  public GameObject ObjectTypeBtn;

  // Block types on tutorial

  public MakerBlockType[] supportedBlocks;

  Canvas canvas;
  MakerActionsManager actionsManager;
  Transform objectivesUI;
  RectTransform objectiveBadge;
  TouchComponent touchComponent;

  bool didShowTutorialSteps = false;

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

    GameObject.FindObjectOfType<AddOnHold> ().config = supportedBlocks;

    InitObjectivesUI ();

    IntroScreen.SetActive (true);
    touchComponent = GetComponent<TouchComponent> ();

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

    if (TutorialSteps.activeInHierarchy) TutorialSteps.SetActive (false);

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
        new Vector2 (objectiveBadge.rect.width + 20.0f, 0)
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
        new Action (OnTweenSlideInDone)
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

      if (tagName == MakerTags.Cube) {

        Invoke ("ShowTutorial1", 1);

      }

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
        new Action (RemoveIntro)
      }
    });

  }

  void RemoveIntro () {

    IntroScreen.SetActive (false);

    if (!didShowTutorialSteps) ShowTutorial (0);

  }

  void ShowTutorial1 () {

    ShowTutorial (1);

  }

  void ShowTutorial2 () {

    ShowTutorial (2);

  }

  void ShowTutorial (int i) {

    didShowTutorialSteps = true;

    var director = TutorialSteps.GetComponent<PlayableDirector> ();

    if (i == 0) {

      director.playableAsset = tutorialAnimations[0];

    } else if (i == 1) {

      director.playableAsset = tutorialAnimations[1];

      director.transform.position = AddBtn.transform.position;

      AddBtn.GetComponent<ToggleEnableEvent> ().onEnable.AddListener (() => {

        Invoke ("ShowTutorial2", .1f);

      });

    } else if (i == 2) {

      director.playableAsset = tutorialAnimations[1];

      director.transform.position = ObjectTypeBtn.transform.position;

      ObjectTypeBtn.GetComponent<ToggleEnableEvent> ().onEnable.AddListener (() => {

        TutorialSteps.SetActive (false);

      });
    }

    TutorialSteps.SetActive (true);
    director.Play ();

  }

  void OnShowObjectivesClick () {

    IntroScreen.SetActive (true);

  }

}