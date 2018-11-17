using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour {

  public void Start () {
    var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
    PlayFabClientAPI.LoginWithCustomID (request, OnLoginSuccess, OnLoginFailure);
  }

  void Save () {

    var world = GameObject.Find ("world");
    PlayFabClientAPI.UpdateUserData (new UpdateUserDataRequest () {
        Data = new Dictionary<string, string> () { { "Ancestor", "Arthur" }, { "Successor", "Fred" }
        }
      },
      result => Debug.Log ("Successfully updated user data"),
      error => {
        Debug.Log ("Got error setting user data Ancestor to Arthur");
        Debug.Log (error.GenerateErrorReport ());
      });
  }

  void OnLoginSuccess (LoginResult result) {
    Debug.Log ("Congratulations, you made your first successful API call!");
  }

  void OnLoginFailure (PlayFabError error) {
    Debug.LogWarning ("Something went wrong with your first API call.  :(");
    Debug.LogError ("Here's some debug information:");
    Debug.LogError (error.GenerateErrorReport ());
  }

}