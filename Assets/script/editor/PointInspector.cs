using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor (typeof (PathPointComponent))]
public class PointInspector : Editor {

  public bool drawLines = !true;
  public bool paintNeighbours = !true;

  bool isDebugDone = false;
  PathPointComponent lastPoint;
  GUIStyle style;

  List<Vector3> lineSegments = new List<Vector3> ();

  void Awake () {
    var tex2 = new Texture2D (1, 1);
    var fillColor = new Color (1, 1, 1, .5f);
    var fillColorArray = tex2.GetPixels ();
    for (var i = 0; i < fillColorArray.Length; ++i) {
      fillColorArray[i] = fillColor;
    }
    tex2.SetPixels (fillColorArray);
    tex2.Apply ();

    style = new GUIStyle ();
    // style.normal.textColor = Color.green;
    style.normal.background = tex2;
  }

  void OnSceneGUI () {

    var targetObject = (PathPointComponent) target;

    if (drawLines || paintNeighbours) CheckRay (targetObject);

    if (drawLines) {
      for (var i = 0; i < lineSegments.Count; i += 2) {
        var start = lineSegments[i];
        var end = lineSegments[i + 1];
        Debug.DrawLine (start, end, Color.red);
      }
    }

    Handles.BeginGUI ();

    // Handles.DrawLines(lineSegments.ToArray());

    var point = targetObject.point;

    if (point != null) {

      var labelText = point.position.ToString () + "\n" +
        point.realCamPosition.ToString ();

      if (point.twistedBlockConn != null) {
        labelText += "\n" + point.twistedBlockConn.position.ToString ();
      }

      Handles.Label (
        targetObject.transform.position,
        labelText,
        style
      );
    }

    Handles.EndGUI ();
  }

  void CleanAll () {
    var allPoints = Object.FindObjectsOfType<PathPointComponent> ();
    lineSegments.Clear ();
    if (paintNeighbours) {
      foreach (var point in allPoints) {
        Utility.SetPointColor (point, new Color (.4f, .0f, .8f));
      }
    }
  }

  public void CheckRay (PathPointComponent pointComponent) {

    if (lastPoint != pointComponent) {

      CleanAll ();
      isDebugDone = false;
      lastPoint = pointComponent;
    }

    if (isDebugDone) return;
    isDebugDone = true;

    var point = pointComponent.point;

    Vector3[] directions = {
      point.component.transform.transform.forward,
      point.component.transform.transform.right,
      -point.component.transform.transform.forward,
      -point.component.transform.transform.right
    };

    foreach (var dir in directions) {

      var pos = point.position + dir;
      var nextPoints = Utility.getPointsAtWorldPos (pos, point.normal);

      var screenPoint = Camera.main.WorldToScreenPoint (pos);
      screenPoint.z = 0;

      var ray = Camera.main.ScreenPointToRay (screenPoint);
      AddDebugLine (ray);

      if (paintNeighbours) {
        foreach (var nextPoint in nextPoints) {
          Utility.SetPointColor (nextPoint.component, new Color (1.0f, .2f, 0));
        }
      }

    }

  }

  void AddDebugLine (Ray ray) {
    var start = ray.origin;
    var end = ray.origin + ray.direction * (lastPoint.transform.position - ray.origin).magnitude;
    lineSegments.Add (start);
    lineSegments.Add (end);
  }

  [MenuItem ("MyMenu/Align camera")]
  static void AlignCamera () {

    var camera = Camera.main.transform;

    SceneView.lastActiveSceneView.pivot = camera.position;
    SceneView.lastActiveSceneView.rotation = camera.rotation;
    SceneView.lastActiveSceneView.Repaint ();

  }

  [MenuItem ("MyMenu/Remove all  test scenes from Build")]
  static void RemoveAllTestScenes () {
    var sceneList = EditorBuildSettings.scenes.ToList ();
    sceneList.RemoveAll (scene => {
      return scene.path.Contains ("issue");
    });
    EditorBuildSettings.scenes = sceneList.ToArray ();
    Debug.Log ("Remove all test scenes from Build.");
  }

  [MenuItem ("MyMenu/Add level scenes to Build")]
  static void AddAllLevelScenes () {
    AddScenes ("Assets/scenes/levels/");
  }

  [MenuItem ("MyMenu/Add test scenes to Build")]
  static void AddAllTestScenes () {
    AddScenes ("Assets/scenes/tests/");
  }

  [MenuItem ("MyMenu/Find all references in scene")]
  static void FindAllReferencesInScene () {

    var target = Selection.activeTransform;

    if (!target) return;

    var matchList = new List<GameObject> ();

    var allObjs = GameObject.FindObjectsOfType<Transform> ();

    foreach (var obj in allObjs) {

      var componentList = obj.GetComponents<Component> ();

      foreach (var comp in componentList) {

        foreach (var prop in comp.GetType ().GetFields ()) {

          try {

            var attr = prop.GetValue (comp);

            if (attr.GetType ().IsGenericType && attr.GetType ().GetGenericTypeDefinition () == typeof (List<>)) {

              var t = attr.GetType ().GetGenericArguments () [0];

              if (t.IsSubclassOf (typeof (Component))) {

                var attrList = (IEnumerable) attr;

                foreach (var attrItem in attrList) {

                  if ((attrItem as Component).transform != target) continue;

                  matchList.Add (comp.gameObject);

                  break;

                }

              }

              if (t == typeof (GameObject)) {

                var attrList = (IEnumerable) attr;

                foreach (var attrItem in attrList) {

                  if ((attrItem as GameObject).transform != target) continue;

                  matchList.Add (comp.gameObject);

                  break;

                }

              }

            }

            if (attr.GetType ().IsArray) {

              var t = attr.GetType ().GetElementType ();

              if (t.IsSubclassOf (typeof (Component))) {

                var attrList = attr as Component[];

                foreach (var attrItem in attrList) {

                  if (attrItem.transform != target) continue;

                  matchList.Add (comp.gameObject);

                  break;

                }

              }

              if (t == typeof (GameObject)) {

                var attrList = attr as GameObject[];

                foreach (var attrItem in attrList) {

                  if (attrItem.transform != target) continue;

                  matchList.Add (comp.gameObject);

                  break;

                }

              }

              break;

            }

            if ((attr.GetType ().IsSubclassOf (typeof (Component)))) {

              var attrComp = attr as Component;

              if (attrComp.transform != target) continue;

              matchList.Add (comp.gameObject);

              break;

            }

            if (attr is GameObject) {

              var attrComp = attr as GameObject;

              if (attrComp.transform != target) continue;

              matchList.Add (comp.gameObject);

              break;

            }

          } catch (System.Exception) {

          }

        }

        if (matchList.Count != 0 && matchList.Last () == obj.gameObject) break;

      }

    }

    Selection.objects = matchList.ToArray ();

  }

  static void AddScenes (string dirName) {

    var files = GetSceneFiles (dirName);

    var scenes = EditorBuildSettings.scenes;

    var sceneList = EditorBuildSettings.scenes.ToList ();

    files.RemoveAll (file => {
      return sceneList.Exists (scene => {
        return scene.path == file;
      });
    });

    var moreScenes = new EditorBuildSettingsScene[scenes.Length + files.Count];
    System.Array.Copy (scenes, moreScenes, scenes.Length);

    for (var i = 0; i < files.Count; i++) {

      var sceneToAdd = new EditorBuildSettingsScene (files[i], true);
      moreScenes[scenes.Length + i] = sceneToAdd;

    }

    EditorBuildSettings.scenes = moreScenes;

    Debug.Log ("Scenes added.");

  }

  static List<string> GetSceneFiles (string dirName) {

    var fileList = new List<string> ();

    DirectoryInfo dir = new DirectoryInfo (dirName);
    var files = dir.GetFiles ("*.unity");

    foreach (var file in files) {
      fileList.Add (dirName + file.Name);
    }

    return fileList;
  }

}