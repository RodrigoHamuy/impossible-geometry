using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

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

  string currLevelName = "level_0";

  public void Start () {

    actionsManager = GetComponent<MakerActionsManager> ();

    world = GameObject.Find ("World").transform;

    if (!PlayFabClientAPI.IsClientLoggedIn ()) {

      var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
      PlayFabClientAPI.LoginWithCustomID (request, OnLoginSuccess, OnLoginFailure);

    } else {
      InitLevel ();
    }

  }

  public void OnBackToMenuClick () {
    SceneManager.LoadScene ("communityLevels");
  }

  void Update () {

    if (!isSaving && saveRequest) {
      Save ();
    }

  }

  void InitLevel () {
    readyToSave = true;
    if (CommunityLevelMenu.loadLevel) {
      CommunityLevelMenu.loadLevel = false;
      currLevelName = CommunityLevelMenu.levelName;
      LoadLevel ();
    } else {
      currLevelName = CommunityLevelMenu.levelName;
    }
  }

  public void Save () {

    if (isLoading) return;

    if (
      isSaving || !readyToSave ||
      consecutiveErrors > maxConsecutiveErrorsAllow
    ) {
      saveRequest = true;
      return;
    }

    var allBlocks = world.GetComponentsInChildren<EditableBlock> ();

    var allBlocksData = Array.ConvertAll (allBlocks, a => a.data);

    var allBlocksDataJson = Zipper.Zip (JsonConvert.SerializeObject (allBlocksData));

    var allBlocksDataJsonSplitted = SplitStringInChunks (allBlocksDataJson, 9999);

    if (allBlocksDataJsonSplitted.Count > 9) {

      print ("your level is to big and can't be saved.");
      return;

    }

    Debug.Log ("Save Request");

    isSaving = true;
    saveRequest = false;
    readyToSave = false;

    var data = new Dictionary<string, string> ();

    var levelKeys = new List<string> ();

    for (int i = 0; i < allBlocksDataJsonSplitted.Count; i++) {

      var key = currLevelName + "_" + i;

      levelKeys.Add (key);

      data[key] = allBlocksDataJsonSplitted[i];

    }

    data[currLevelName] = JsonConvert.SerializeObject (levelKeys);

    PlayFabClientAPI.UpdateUserData (
      new UpdateUserDataRequest () { Data = data },
      result => {
        Debug.Log ("Save Done");
        isSaving = false;
        Invoke ("ResetReadyToSave", 16.0f);
        consecutiveErrors = 0;
      },
      error => {
        ++consecutiveErrors;
        Debug.Log ("Got error setting user data");
        Debug.Log (error.GenerateErrorReport ());
      });
  }

  List<string> SplitStringInChunks (string value, int chunkSize) {

    var i = 0;

    var values = new List<string> ();

    while (i < value.Length) {

      var limit = i + chunkSize > value.Length ? value.Length - i : chunkSize;

      values.Add (value.Substring (i, limit));
      i += chunkSize;

    }

    return values;

  }

  void LoadLevel () {

    if (isLoading) return;

    isLoading = true;

    GetLevelFragmentNames ();
  }

  void GetLevelFragmentNames () {

    PlayFabClientAPI.GetUserData (
      new GetUserDataRequest () {
        Keys = new List<string> { currLevelName }
      },
      GetLevelFragments,
      OnRequestError
    );

  }

  void GetLevelFragments (GetUserDataResult fragResult) {

    Debug.Log ("GetLevelFragmentNames Response.");
    var levelPartNamesJson = fragResult.Data[currLevelName].Value;
    var levelPartNames = JsonConvert.DeserializeObject<List<string>> (levelPartNamesJson);

    PlayFabClientAPI.GetUserData (
      new GetUserDataRequest () {
        Keys = levelPartNames
      },
      result => { GetAllLevelFraments (result, levelPartNames); },
      OnRequestError
    );

  }

  void GetAllLevelFraments (GetUserDataResult result, List<string> levelPartNames) {

    Debug.Log ("GetAllLevelFraments Response.");

    var levelJson = "";

    foreach (var value in levelPartNames) {
      levelJson += result.Data[value].Value;
    }

    isLoading = false;

    LoadLevel (Zipper.Unzip (levelJson));

  }

  void LoadLevel (string levelDataJson) {

    var levelData = JsonConvert.DeserializeObject<EditableBlockData[]> (levelDataJson);

    var blocks = new List<EditableBlock> ();

    foreach (var blockData in levelData) {

      var blockType = actionsManager.GetMakerBlockType (blockData.blockType);

      var block = Instantiate (
        blockType.prefab,
        blockData.position,
        blockData.rotation,
        world
      );

      var blockEditData = block.gameObject.AddComponent<EditableBlock> ();
      blockEditData.data = blockData;
      blocks.Add (blockEditData);

    }

    foreach (var block in blocks) {

      block.SyncTransform (blocks);

    }

  }

  void ResetReadyToSave () {
    readyToSave = true;
  }

  void OnLoginSuccess (LoginResult result) {
    Debug.Log ("Congratulations, you made your first successful API call!");
    InitLevel ();
  }

  void OnLoginFailure (PlayFabError error) {
    Debug.LogWarning ("Something went wrong with your first API call.  :(");
    Debug.LogError ("Here's some debug information:");
    Debug.LogError (error.GenerateErrorReport ());
  }

  void OnRequestError (PlayFabError error) {
    isLoading = false;
    ++consecutiveErrors;
    Debug.Log ("Got error getting user data");
    Debug.Log (error.GenerateErrorReport ());
  }

}