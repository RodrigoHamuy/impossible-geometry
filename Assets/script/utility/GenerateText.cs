using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateText : MonoBehaviour {

  public Transform container;

  public Transform h1;
  public Transform h2;
  public Transform p;
  [TextArea]
  public string jsonString;

  public string nextScene;

  public void LoadScene () {

    SceneManager.LoadScene (nextScene);

  }

}