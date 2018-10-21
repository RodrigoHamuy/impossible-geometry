using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceBtn : MonoBehaviour {

  public GameObject prefab;

  EditManager manager;
  
  Button btn;

  void Start() {

    manager = GameObject.FindObjectOfType<EditManager>();

    btn = gameObject.GetComponent<Button>();

    btn.onClick.AddListener( ()=> {
      manager.Replace(prefab);
    });

  }


}