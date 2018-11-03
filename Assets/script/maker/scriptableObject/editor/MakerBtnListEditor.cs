using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor (typeof (MakerBtnList))]

public class MakerBtnListEditor : Editor {

  public override void OnInspectorGUI () {

    DrawDefaultInspector ();

    var btnList = (MakerBtnList) target;

    if (GUILayout.Button ("Build Object")) {
      InitBtns (btnList);
    }

  }

  void InitBtns (MakerBtnList btnList) {

    var toggleGroup = btnList.GetComponent<ToggleGroup> ();

    var toggles = new List<Toggle> ();

    foreach (var btnStyle in btnList.btnStyleList.btns) {

      var btn = (
        PrefabUtility.InstantiatePrefab (btnList.btnPrefab) as GameObject
      ).GetComponent<MakerBtn> ();

      btn.transform.SetParent (btnList.transform, false);

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