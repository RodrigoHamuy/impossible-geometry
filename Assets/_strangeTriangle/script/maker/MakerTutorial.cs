using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MakerTutorial : MonoBehaviour {

  public GameObject PlayUI;
  public GameObject TutorialPlay;
  public GameObject TutorialCreate;

  string[] gameLevels = new string[] {
    // Level 0 - The basics
    "H4sIAAAAAAAAC93VPQvCMBAG4P9yc1ty/dDaTcTBQXBwE4eoh5SGRtIUreJ/N0pBLagIEYJTQnhzx8MRsjjBSsh1MW92BBmM6hVNeUEKPMg3kCF6oKTmmkay1EoKQWpizn1zvuOKSt3uZZXrXJaQneBgrgXMgwYydl2PJhIG7NxWuqdYJ3Vb9+1tqk2rYbkVVL2On03Ras0FdfpiG8Fb5AMxtENEh4mRFSJzWBhbEbo8w8SK0OWH2LMijBwW9q0IY4eFqRVh4o5wztWW9KMxZH/3J84Eb0g9DXLw+0Fix+jHQdRHjAZjn6XvtJh+611eADALqonoCAAA",
    // Level 1 - Rotation
    // "H4sIAAAAAAAAC9XYz0vDMBQH8P/lnetI+itdb7KTB8HDbuIhm2GMhXakGVrH/ndjqYpBJLwFyTu1ZG8NH17Jvm+PZ9jofntYj0cFLaxOG3UvD8pABvtnaHkGprfSqlXfWdNrrcydW75x60dpVGfn+37Y233fQXuGV2jzBctghJZ9XN+gFQt2mR/0XcS+igQTnNWiXk7F0/qLe+6PD9TJbX3b7bQavO/n4nMbd724fYat1Gou4nMRn0v4VPK3OUeZC9LmAmUuSZtLlLkiba5Q5pq0uUaZBWmziGKuSZmbKOaKlHkZxVySMnMWBV3QQuOSmI/OaaFxUcxHc1poXBbzf6OJoeOEMVroHPd6cw8dlEya38nN/3Y5ijcolSThjdPfoESShBd3cvneoDSShBd3aPlTc9ChlYQXNz36/Q0KXkl4cZOj/09Quv190HJU5vq5kU6H19LslL1+aky4x0/vAdPU4dMVAAA=",
    // // Level 2 - Vertical Rotation
    // "H4sIAAAAAAAAC9WXz2/CIBTH/xfOpQGEAr0txmQ7LFmMt2WH6ogxstZUjOuM//sqo1Fbf6XdoXCBwCvvfb68Ptr3HZjqbLacFCsFYjDcTNVrslQ5CMDiE8SDAOSZSYwaZqnJM61V/lJOQxyAVZKr1Lhxtl6YRZaCeAe+D1MhCkABYnTof0DMQrR3Ox2tUM3I9lsQ24fVpnT1lM61Wl8335ebrmeJVs6kcoudCbYmtwlpK8J6MD0GZK0APTpBjFsRco8ISStC4RFhu0Ij+0s4tjTPSfqpz0mvFBzMbpJS5xKyUEaIRpSKEUTiFjVHHKNIXGSHJ8tNCQj/JxFGXytTjGu0Z2q0q070sXPvgQKNREf3gW0qXL5vIAkHgmLBuCyPn1u3JES2EdrUoKRkDoNZ47+JbTVoksvKlZPgKrptYlApcGjYRVStUnFXD9lNDhxiSZAkRzUq35h4qIbopMZphP6x807szUSAXmdC1EmNC1UCelUm3nRS1G6KBxTx7FdkkuRzZc4gH3gJev4Z9PELbciwrVIOAAA=",
  };

  SaveManager saveManager;
  MakerStateManager manager;

  void Awake () {

    manager = GameObject.FindObjectOfType<MakerStateManager> ();

    if (LevelMakerConfig.Data != null) return;

    LevelMakerConfig.Data = new LevelMakerSetting (
      "tutorial_play_0",
      true,
      false,
      gameLevels[0]
    );

    TutorialPlay.SetActive (true);

  }

  void Start () {

    if (
      LevelMakerConfig.Data != null &&
      LevelMakerConfig.Data.LevelName.Contains ("tutorial_play_")
    ) Invoke ("Play", .1f);

  }

  void Play () {

    // TutorialPlay.GetComponent<Image> ().DOFade (0, 1.0f);
    // TutorialPlay.transform.DOMoveX (100, 1);

    iTween.FadeTo (TutorialPlay, 0, 10.0f);
    // iTween.FadeTo (TutorialPlay, new Hashtable () {
    //   {
    //     "alpha",
    //     0.0f
    //   }, {
    //     "time",
    //     1.0f
    //   },
    // });

    manager.SetState (
      MakerStateType.General,
      MakerState.MakerPlay
    );

    PlayUI.SetActive (false);

    var target = GameObject.FindObjectOfType<TargetComponent> ();
    target.onTargetReached.AddListener (OnTargetReached);

  }

  void OnTargetReached () {

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