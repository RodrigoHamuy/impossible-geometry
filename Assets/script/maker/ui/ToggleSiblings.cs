using UnityEngine;

public class ToggleSiblings : MonoBehaviour {

  void OnEnable () {

    foreach (Transform child in transform.parent) {

      if (child.name == "Bg") continue;

      if (child.gameObject == gameObject) continue;

      child.gameObject.SetActive (false);

    }

  }

}