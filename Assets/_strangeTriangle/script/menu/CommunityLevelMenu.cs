using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CommunityLevelMenu : MonoBehaviour {

  public Transform levelBtnsContainer;
  public Button newLevelBtn;

  public Transform btnPrefab;

  List<string> levels;

  GameObject canvas;

  public void Start () {

    canvas = GameObject.FindObjectOfType<Canvas> ().gameObject;

    canvas.SetActive (false);

    if (!PlayFabClientAPI.IsClientLoggedIn ()) {

      var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
      PlayFabClientAPI.LoginWithCustomID (request, OnLoginSuccess, OnLoginFailure);

    } else {

      GetLevelsRequest ();

    }

    newLevelBtn.onClick.AddListener (LoadNewLevel);

  }

  void GetLevelsRequest () {

    PlayFabClientAPI.GetUserData (
      new GetUserDataRequest () {
        Keys = new List<string> { "levels" }
      },
      GetLevelsResponse,
      OnError
    );

  }

  void GetLevelsResponse (GetUserDataResult result) {

    if (result.Data.ContainsKey ("levels")) {
      levels = JsonConvert.DeserializeObject<List<string>> (result.Data["levels"].Value);
    } else {
      levels = new List<string> ();
    }

    foreach (var level in levels) {

      var btn = Instantiate (
        btnPrefab,
        Vector3.zero,
        Quaternion.identity,
        levelBtnsContainer
      );

      btn.GetComponentInChildren<Text> ().text = level;

      AddLoadLevelListener (btn, level);

    }

    canvas.SetActive (true);

  }

  void AddLoadLevelListener (Transform btn, string level) {

    btn.GetComponent<Button> ().onClick.AddListener (() => {

      LoadLevel (level);

    });

  }

  void LoadLevel (string levelName) {

    LevelMakerConfig.Data = new LevelMakerSetting (levelName, true, true);
    SceneManager.LoadScene ("levelMaker");

  }

  void LoadNewLevel () {

    var level = "level_" + levels.Count;

    levels.Add (level);

    PlayFabClientAPI.UpdateUserData (
      new UpdateUserDataRequest () {
        Data = new Dictionary<string, string> { { "levels", JsonConvert.SerializeObject (levels) } }
      },
      result => OnLoadNewLevelSaved (level),
      OnError
    );

  }

  void OnLoadNewLevelSaved (string level) {

    LevelMakerConfig.Data = new LevelMakerSetting (level, false, true);
    SceneManager.LoadScene ("levelMaker");

  }

  void OnLoginSuccess (LoginResult result) {
    Debug.Log ("Congratulations, you made your first successful API call!");
    GetLevelsRequest ();
  }

  void OnError (PlayFabError error) {
    Debug.LogWarning ("Something went :(");
    Debug.LogError ("Here's some debug information:");
    Debug.LogError (error.GenerateErrorReport ());
  }

  void OnLoginFailure (PlayFabError error) {
    Debug.LogWarning ("Something went wrong with your first API call.  :(");
    Debug.LogError ("Here's some debug information:");
    Debug.LogError (error.GenerateErrorReport ());
  }

}