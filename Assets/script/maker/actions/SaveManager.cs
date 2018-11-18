using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

[RequireComponent (typeof (MakerActionsManager))]

public class SaveManager : MonoBehaviour {

  Transform world;

  MakerActionsManager actionsManager;

  bool isLoading = false;

  bool isSaving = false;
  bool saveRequest = false;
  bool readyToSave = false;

  int maxConsecutiveErrorsAllow = 10;

  int consecutiveErrors = 0;

  public void Start () {

    actionsManager = GetComponent<MakerActionsManager> ();

    world = GameObject.Find ("World").transform;

    if (!PlayFabClientAPI.IsClientLoggedIn ()) {

      var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
      PlayFabClientAPI.LoginWithCustomID (request, OnLoginSuccess, OnLoginFailure);

    }

  }

  void Update () {

    if (!isSaving && saveRequest) {
      Save ();
    }

  }

  public void Save () {

    if (isLoading) return;

    if (
      isSaving || !
      readyToSave ||
      consecutiveErrors > maxConsecutiveErrorsAllow
    ) {
      saveRequest = true;
      return;
    }

    isSaving = true;
    saveRequest = false;

    var allBlocks = world.GetComponentsInChildren<EditableBlock> ();

    var allBlocksData = Array.ConvertAll (allBlocks, a => a.data);

    var allBlocksDataJson = JsonConvert.SerializeObject (allBlocksData);

    PlayFabClientAPI.UpdateUserData (
      new UpdateUserDataRequest () {
        Data = new Dictionary<string, string> () { { "level_0", allBlocksDataJson }
        }
      },
      result => {
        Debug.Log ("Successfully updated user data");
        isSaving = false;
        Invoke ("ResetReadyToSave", 5.0f);
        consecutiveErrors = 0;
      },
      error => {
        ++consecutiveErrors;
        Debug.Log ("Got error setting user data");
        Debug.Log (error.GenerateErrorReport ());
      });
  }

  public void Load () {

    if (isLoading) return;

    isLoading = true;

    PlayFabClientAPI.GetUserData (
      new GetUserDataRequest () {
        Keys = new List<string> { "level_0" }
      },
      result => {
        Debug.Log ("Got user data:");
        LoadLevel (result.Data["level_0"].Value);
      },
      error => {
        ++consecutiveErrors;
        Debug.Log ("Got error getting user data");
        Debug.Log (error.GenerateErrorReport ());
      });

  }

  void LoadLevel (string levelDataJson) {

    var levelData = JsonConvert.DeserializeObject<EditableBlockData[]> (levelDataJson);

    foreach (var blockData in levelData) {

      var blockType = actionsManager.GetMakerBlockType (blockData.blockType);

      var block = Instantiate (
        blockType.prefab,
        blockData.position,
        blockData.rotation,
        world
      );

      block.gameObject.AddComponent<EditableBlock> ().data = blockData;

    }

  }

  void ResetReadyToSave () {
    readyToSave = true;
  }

  void OnLoginSuccess (LoginResult result) {
    readyToSave = true;
    Debug.Log ("Congratulations, you made your first successful API call!");
  }

  void OnLoginFailure (PlayFabError error) {
    Debug.LogWarning ("Something went wrong with your first API call.  :(");
    Debug.LogError ("Here's some debug information:");
    Debug.LogError (error.GenerateErrorReport ());
  }

}