using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (MakerBtn))]

public class MakerBtnEditor : Editor {

  public override void OnInspectorGUI () {

    DrawDefaultInspector ();

    var myScript = (MakerBtn) target;

    if (GUILayout.Button ("Build Object")) {
      myScript.InitBtn ();
    }

  }

}