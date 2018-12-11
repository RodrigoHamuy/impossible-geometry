using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakerCreateTutorial : MonoBehaviour {

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

  // Second row btns

  void Awake () {

    if (
      LevelMakerConfig.Data == null ||
      !LevelMakerConfig.Data.LevelName.Contains ("tutorial_create_")
    ) {
      enabled = false;
      return;
    }

    IntroScreen.SetActive (true);

  }

  void Start () {

    IntroScreen.GetComponent<VerticalLayoutGroup> ().enabled = false;

    IntroScreen.GetComponent<Button> ().onClick.AddListener (FadeIntro);

    Invoke ("HideBtns", .1f);

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
        1
      }, {
        "oncomplete",
        "RemoveIntro"
      }, {
        "oncompletetarget",
        gameObject
      }
    });

    var childA = IntroScreen.transform.GetChild (0).GetComponent<RectTransform> ();

    iTween.MoveAdd (childA.gameObject, new Vector3 (childA.position.x * 2.0f, 0, 0), 1);

    var childB = IntroScreen.transform.GetChild (1).GetComponent<RectTransform> ();

    iTween.MoveAdd (childB.gameObject, -new Vector3 (childA.position.x * 2.0f, 0, 0), 1);

  }

  void RemoveIntro () {

    IntroScreen.SetActive (false);

  }

}