using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplaceBtn : MonoBehaviour {

  public GameObject prefab;

  EditManager manager;

  Toggle btn;

  void Start () {

    manager = GameObject.FindObjectOfType<EditManager> ();

    btn = gameObject.GetComponent<Toggle> ();

    btn.onValueChanged.AddListener (value => {

      if (!value) return;

      manager.Replace (prefab);

    });

  }

}