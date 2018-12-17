using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MakerPlayTutorial : MonoBehaviour {

  public GameObject PauseBtn;
  public GameObject IntroScreen;

  string[] gameLevels = new string[] {
    // Level 0 - The basics
    "eNrd2D1rwzAQBuD/crMdJMuf2kLo0KHQIVvpoKQihAgryAqtG/LfqwRDaaCGihtOnc6IF4lHwuaslzNsjN0e1uNRg4TVaaOf1EE7yGD/BpJn4KxXXq9s7501RrvHMJyH8aNyuvfTsx32fm97kGf4AFkvWAYjSHatnyC7BbtME32H2F3oVt/DkteqT2GlZb8zevg9fgmTDltl9BThU4RPEX6LzAMLFGBLFyhQgA1dYIkCrOkCKxRgRRdYowBLusAGBSjoAtsoYJUOsIsClukAOYsSioSEcb1MkZAwrpnhCQnjuhmWkDCunclTOsQKh1gQJtY4RE6Y2OAQGWFii0PMKR9jh2Qk/DYWDMlI+KNacCQjod+otXI77X8oxf9TPhs1aod/JzV76cbviGFDRMO56B5y1s5heftX7usXpmvIEw==",
    // Level 1 - Rotation
    // "",
    // // Level 2 - Vertical Rotation
    // "",
  };

  Canvas canvas;
  SaveManager saveManager;
  MakerStateManager manager;

  void Awake () {

    if (
      LevelMakerConfig.Data != null &&
      !LevelMakerConfig.Data.LevelName.Contains ("tutorial_play_")
    ) {

      enabled = false;
      return;

    }

    canvas = GameObject.FindObjectOfType<Canvas> ();
    manager = GameObject.FindObjectOfType<MakerStateManager> ();

    IntroScreen.SetActive (true);

    if (LevelMakerConfig.Data != null) return;

    LevelMakerConfig.Data = new LevelMakerSetting (
      "tutorial_play_0",
      true,
      false,
      gameLevels[0]
    );

  }

  void Start () {

    IntroScreen.GetComponent<VerticalLayoutGroup> ().enabled = false;

    IntroScreen.GetComponent<Button> ().onClick.AddListener (FadeIntro);

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
        new Action (RemoveIntro)
      }
    });

    var childA = IntroScreen.transform.GetChild (0).GetComponent<RectTransform> ();

    iTween.MoveAdd (childA.gameObject, new Vector3 (childA.position.x * 2.0f, 0, 0), 1);

    var childB = IntroScreen.transform.GetChild (1).GetComponent<RectTransform> ();

    iTween.MoveAdd (childB.gameObject, -new Vector3 (childA.position.x * 2.0f, 0, 0), 1);

    manager.SetState (
      MakerStateType.General,
      MakerState.MakerPlay
    );

    PauseBtn.SetActive (false);

    var target = GameObject.FindObjectOfType<TargetComponent> ();
    target.onTargetReached.AddListener (OnLevelCompleted);

  }

  void RemoveIntro () {

    IntroScreen.SetActive (false);

  }

  void OnLevelCompleted () {

    var player = GameObject.FindObjectOfType<PlayerComponent> ();
    player.enabled = false;

    print ("tutorial target reached");

    Invoke ("LoadCreateTutorial", 1.0f);

  }

  void LoadCreateTutorial () {

    LevelMakerConfig.Data = new LevelMakerSetting ("tutorial_create_0", false, false);
    SceneManager.LoadScene ("levelMaker");

  }

}