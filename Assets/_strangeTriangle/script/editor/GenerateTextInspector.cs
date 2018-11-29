using Newtonsoft.Json;
using TMPro;
using UnityEditor;
using UnityEngine;

struct Config {
  public string txt;
  public string type;
}

[CustomEditor (typeof (GenerateText))]
public class GenerateTextInspector : Editor {

  public override void OnInspectorGUI () {

    DrawDefaultInspector ();

    var component = (GenerateText) target;

    if (GUILayout.Button ("Generate Text")) {
      Generate (component);
    }
  }

  public void Generate (GenerateText component) {

    var config = JsonConvert.DeserializeObject<Config[]> (component.jsonString);

    foreach (var item in config) {

      Transform prefab = component.p;

      switch (item.type) {
        case "h1":
          prefab = component.h1;
          break;
        case "h2":
          prefab = component.h2;
          break;
        case "p":
          prefab = component.p;
          break;
      }

      GameObject gameObject = PrefabUtility.InstantiatePrefab (prefab.gameObject) as GameObject;

      var txt = gameObject.GetComponent<TextMeshProUGUI> ();

      txt.transform.parent = component.container;

      txt.text = item.txt;

    }

  }

}