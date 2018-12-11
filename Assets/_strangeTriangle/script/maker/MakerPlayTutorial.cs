using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MakerPlayTutorial : MonoBehaviour {

  public GameObject PauseBtn;
  public GameObject IntroScreen;

  string[] gameLevels = new string[] {
    // Level 0 - The basics
    "H4sIAAAAAAAAC93VPQvCMBAG4P9yc1ty/dDaTcTBQXBwE4eoh5SGRtIUreJ/N0pBLagIEYJTQnhzx8MRsjjBSsh1MW92BBmM6hVNeUEKPMg3kCF6oKTmmkay1EoKQWpizn1zvuOKSt3uZZXrXJaQneBgrgXMgwYydl2PJhIG7NxWuqdYJ3Vb9+1tqk2rYbkVVL2On03Ras0FdfpiG8Fb5AMxtENEh4mRFSJzWBhbEbo8w8SK0OWH2LMijBwW9q0IY4eFqRVh4o5wztWW9KMxZH/3J84Eb0g9DXLw+0Fix+jHQdRHjAZjn6XvtJh+611eADALqonoCAAA",
    // Level 1 - Rotation
    // "H4sIAAAAAAAAC9XYz0vDMBQH8P/lnetI+itdb7KTB8HDbuIhm2GMhXakGVrH/ndjqYpBJLwFyTu1ZG8NH17Jvm+PZ9jofntYj0cFLaxOG3UvD8pABvtnaHkGprfSqlXfWdNrrcydW75x60dpVGfn+37Y233fQXuGV2jzBctghJZ9XN+gFQt2mR/0XcS+igQTnNWiXk7F0/qLe+6PD9TJbX3b7bQavO/n4nMbd724fYat1Gou4nMRn0v4VPK3OUeZC9LmAmUuSZtLlLkiba5Q5pq0uUaZBWmziGKuSZmbKOaKlHkZxVySMnMWBV3QQuOSmI/OaaFxUcxHc1poXBbzf6OJoeOEMVroHPd6cw8dlEya38nN/3Y5ijcolSThjdPfoESShBd3cvneoDSShBd3aPlTc9ChlYQXNz36/Q0KXkl4cZOj/09Quv190HJU5vq5kU6H19LslL1+aky4x0/vAdPU4dMVAAA=",
    // // Level 2 - Vertical Rotation
    // "H4sIAAAAAAAAC9WXz2/CIBTH/xfOpQGEAr0txmQ7LFmMt2WH6ogxstZUjOuM//sqo1Fbf6XdoXCBwCvvfb68Ptr3HZjqbLacFCsFYjDcTNVrslQ5CMDiE8SDAOSZSYwaZqnJM61V/lJOQxyAVZKr1Lhxtl6YRZaCeAe+D1MhCkABYnTof0DMQrR3Ox2tUM3I9lsQ24fVpnT1lM61Wl8335ebrmeJVs6kcoudCbYmtwlpK8J6MD0GZK0APTpBjFsRco8ISStC4RFhu0Ij+0s4tjTPSfqpz0mvFBzMbpJS5xKyUEaIRpSKEUTiFjVHHKNIXGSHJ8tNCQj/JxFGXytTjGu0Z2q0q070sXPvgQKNREf3gW0qXL5vIAkHgmLBuCyPn1u3JES2EdrUoKRkDoNZ47+JbTVoksvKlZPgKrptYlApcGjYRVStUnFXD9lNDhxiSZAkRzUq35h4qIbopMZphP6x807szUSAXmdC1EmNC1UCelUm3nRS1G6KBxTx7FdkkuRzZc4gH3gJev4Z9PELbciwrVIOAAA=",
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