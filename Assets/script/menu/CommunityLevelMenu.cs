using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class CommunityLevelMenu : MonoBehaviour {

  public Transform levelListContainer;

  public Transform btnPrefab;

  public void Start () {

    if (!PlayFabClientAPI.IsClientLoggedIn ()) {

      var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
      PlayFabClientAPI.LoginWithCustomID (request, OnLoginSuccess, OnLoginFailure);

    } else {

      LoadLevelsRequest ();

    }

  }

  void LoadLevelsRequest () {

  }

  void OnLoginSuccess (LoginResult result) {
    Debug.Log ("Congratulations, you made your first successful API call!");
    LoadLevelsRequest ();
  }

  void OnLoginFailure (PlayFabError error) {
    Debug.LogWarning ("Something went wrong with your first API call.  :(");
    Debug.LogError ("Here's some debug information:");
    Debug.LogError (error.GenerateErrorReport ());
  }

}