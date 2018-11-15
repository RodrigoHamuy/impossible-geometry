using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor (typeof (MakerBtnList))]
[CanEditMultipleObjects]

public class MakerBtnListEditor : Editor {

  public override void OnInspectorGUI () {

    DrawDefaultInspector ();

    if (GUILayout.Button ("Build Object")) {

      foreach (var t in targets) {
        var thisTarget = (MakerBtnList) t;
        InitBtns (thisTarget);
      }

    }

  }

  void InitBtns (MakerBtnList thisTarget) {

    while (thisTarget.transform.childCount > 0) {
      DestroyImmediate (thisTarget.transform.GetChild (0).gameObject);
    }

    var toggleGroup = thisTarget.GetComponent<ToggleGroup> ();

    var toggles = new List<Toggle> ();

    foreach (var btnStyle in thisTarget.btnStyleList.btns) {

      var btn = (
        PrefabUtility.InstantiatePrefab (thisTarget.btnPrefab) as GameObject
      ).GetComponent<MakerBtn> ();

      btn.transform.SetParent (thisTarget.transform, false);

      btn.style = btnStyle;

      btn.InitBtn ();

      var toggle = btn.GetComponent<Toggle> ();

      toggleGroup.RegisterToggle (toggle);

      toggle.group = toggleGroup;

      toggles.Add (toggle);

    }

    toggles[0].isOn = true;

  }

}