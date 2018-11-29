using System;
using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AddOnHold : MonoBehaviour {

  public Transform world;

  public Transform marker;

  public UnityEvent OnBrushStart;
  public UnityEvent OnBrushEnd;

  public GameObject canvas;

  public MakerBlockType[] config;

  MakerBlockType currentConfig;

  Vector3 planeNormal = Vector3.up;

  MakerActionsManager actionsManager;
  MakerStateManager stateManager;

  Vector3 currentPlanePoint;
  Vector2 firstTouchPos;

  bool secondTouchPass = false;

  List<Transform> currentRow = new List<Transform> ();

  Vector3 lastHitPosNoRound;
  Vector3 lastHitPos;

  TouchComponent touchComponent;

  void Start () {

    marker.gameObject.SetActive (false);

    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();
    stateManager = GameObject.FindObjectOfType<MakerStateManager> ();
    touchComponent = gameObject.GetComponent<TouchComponent> ();

    touchComponent.onTouchStart.AddListener (StartStroke);
    touchComponent.onTouchMove.AddListener (MoveStroke);
    touchComponent.onTouchEnd.AddListener (EndStroke);

    stateManager.OnAxisSelect.AddListener (SetPlaneAxis);
    stateManager.OnPrefabMenuShow.AddListener (OnPrefabMenuShow);
    stateManager.OnPrefabSelect.AddListener (OnPrefabSelect);

    currentPlanePoint = transform.position;

    var cam = Camera.main;

    currentConfig = config[0];

  }

  void OnPrefabSelect (Transform selected) {

    if (!gameObject.activeInHierarchy) return;

    currentConfig = Array.Find (config, configItem => configItem.prefab == selected);

  }

  void OnPrefabMenuShow (Transform menuContainer) {

    if (!gameObject.activeInHierarchy) return;

    var menuItems = menuContainer.gameObject.GetComponentsInChildren<TransformEventEmitter> (true);

    foreach (var item in menuItems) {
      item.gameObject.SetActive (false);
    }

    foreach (var item in menuItems) {

      item.GetComponent<Toggle> ().isOn = item.element == currentConfig.prefab;

      item.gameObject.SetActive (
        Array.Exists (config, configItem =>
          configItem.prefab == item.element
        )
      );

    }

  }

  void StartStroke (Vector2 screenPos) {

    marker.gameObject.SetActive (true);

    bool found;

    var hitPos = GetBlockHitPosition (screenPos, out found);

    if (!found) {

      currentPlanePoint = transform.position;
      hitPos = GetHitPosition (screenPos);

    } else {

      currentPlanePoint = hitPos;

    }

    currentRow.Clear ();

    canvas.SetActive (false);

    OnBrushStart.Invoke ();

    firstTouchPos = screenPos;
    secondTouchPass = false;

    if (found) screenPos = Camera.main.WorldToScreenPoint (hitPos);

    MoveStroke (screenPos, true);

  }

  void MoveStroke (Vector2 screenPos) {
    MoveStroke (screenPos, false);
  }

  void MoveStroke (Vector2 screenPos, bool isFirstTouch) {

    if (!currentConfig.addOnHold && currentRow.Count == 1) return;

    if (!secondTouchPass) {

      if (!isFirstTouch &&
        (screenPos - firstTouchPos).magnitude < 10
      ) return;

      else if (!isFirstTouch) secondTouchPass = true;

    }

    var hitPos = GetDirPosition (screenPos);

    if (hitPos == Vector3.zero) return;

    var spaceTaken = currentRow.Exists ((Transform oldBlock) => {

      return oldBlock.position == hitPos;

    });

    if (spaceTaken) print ("space taken: " + hitPos + (currentRow[currentRow.Count - 2].position == hitPos));

    if (
      currentRow.Count > 1 &&
      currentRow[currentRow.Count - 2].position == hitPos
    ) {

      currentRow.RemoveAt (currentRow.Count - 1);
      actionsManager.Undo ();
      lastHitPosNoRound = GetHitPosition (screenPos, false);
      lastHitPos = hitPos; //currentRow[currentRow.Count - 2].position;

      return;

    }

    if (spaceTaken) return;

    if (currentConfig.isUnique) {

      var prevObj = GameObject.FindWithTag (currentConfig.prefab.tag);

      if (prevObj) {

        actionsManager.RemoveBlock (prevObj.transform);

      }

    }

    var block = AddBlock (hitPos);

    marker.transform.position = hitPos;

    currentRow.Add (block);

    lastHitPosNoRound = GetHitPosition (screenPos, false);
    lastHitPos = hitPos;

  }

  Vector3 GetDirPosition (Vector2 screenPos) {

    if (lastHitPos == Vector3.zero) {

      return GetHitPosition (screenPos);

    }

    var currHitNoRound = GetHitPosition (screenPos, false);

    var dir = currHitNoRound - lastHitPosNoRound;

    if (dir.magnitude < 1.0f) return Vector3.zero;

    dir = dir.normalized;

    var max = -Mathf.Infinity;
    var maxIndex = 0;

    for (var i = 0; i < 3; i++) {

      if (planeNormal[i] == 1) continue;

      if (max < Mathf.Abs (dir[i])) {
        max = Mathf.Abs (dir[i]);
        maxIndex = i;
      }

    }

    for (var i = 0; i < 3; i++) {

      if (planeNormal[i] == 1) {

        dir[i] = 0;
        continue;

      }

      if (i == maxIndex) {

        dir[i] = dir[i] > 0 ? 1 : -1;

      } else dir[i] = 0;

    }

    return lastHitPos + dir;

  }

  void EndStroke (Vector2 screenPos) {

    lastHitPosNoRound = Vector3.zero;
    lastHitPos = Vector3.zero;

    canvas.SetActive (true);

    OnBrushEnd.Invoke ();
    marker.gameObject.SetActive (false);

  }

  void SetPlaneAxis (Vector3 axis) {

    if (!gameObject.activeSelf) return;
    planeNormal = axis;

  }

  Vector3 GetLastBlockDirection () {

    var count = currentRow.Count;

    if (count < 2) return Vector3.zero;

    var preLast = currentRow[count - 2].position;
    var last = currentRow[count - 1].position;

    return (last - preLast).normalized;

  }

  void CheckCornerBlock (Vector3 hitPos) {

    if (currentRow.Count == 0) return;

    var last = currentRow[currentRow.Count - 1].position;

    if (hitPos.x != last.x && hitPos.z != last.z) {

      // Add a middle block for corners

      var middlePos = hitPos;
      middlePos.z = last.z;

      var midSpaceTaken = currentRow.Exists ((Transform oldBlock) => {

        return oldBlock.position == middlePos;

      });

      if (midSpaceTaken) {

        middlePos = hitPos;
        middlePos.x = last.x;

      }

      var midBlock = AddBlock (middlePos);

      currentRow.Add (midBlock);

    }
  }

  Transform AddBlock (Vector3 pos) {

    var rotation = Quaternion.identity;

    if (planeNormal == Vector3.forward) {
      rotation = Quaternion.LookRotation (Vector3.up, -Vector3.forward);
    } else if (planeNormal == Vector3.right) {
      rotation = Quaternion.LookRotation (Vector3.up, -Vector3.right);
    }
    var block = actionsManager.AddBlock (
      new MakerAction (
        MakerActionType.Add,
        null,
        currentConfig,
        pos,
        Vector3.one,
        rotation,
        world
      ),
      false
    );

    return block;

  }

  Vector3 GetHitPosition (Vector2 screenPos, bool round = true) {

    var cam = Camera.main;

    var ray = cam.ScreenPointToRay (screenPos);

    var plane = new Plane (planeNormal, currentPlanePoint);

    float enter;

    plane.Raycast (ray, out enter);

    var worldPos = ray.GetPoint (enter);

    if (!round) return worldPos;

    for (var i = 0; i < 3; i++) {

      worldPos[i] = Mathf.Round (worldPos[i]);

    }

    // worldPos.y += .5f;

    return worldPos;

  }

  Vector3 GetBlockHitPosition (Vector2 screenPos, out bool found) {

    var cam = Camera.main;

    var ray = cam.ScreenPointToRay (screenPos);

    string[] layerNames = { "maker.object" };

    var layerMask = LayerMask.GetMask (layerNames);

    RaycastHit hit;

    found = Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask);

    if (!found) return Vector3.zero;

    var block = hit.transform;

    var forward = hit.point - block.transform.position;

    var dotF = Vector3.Dot (forward, Vector3.forward);
    var dotU = Vector3.Dot (forward, Vector3.up);
    var dotR = Vector3.Dot (forward, Vector3.right);

    Vector3 finalForward;

    float closestDot;

    Vector3 closestAxis;

    if (
      Mathf.Abs (dotF) > Mathf.Abs (dotU) &&
      Mathf.Abs (dotF) > Mathf.Abs (dotR)
    ) {

      closestDot = dotF;
      closestAxis = Vector3.forward;

    } else if (
      Mathf.Abs (dotU) > Mathf.Abs (dotF) &&
      Mathf.Abs (dotU) > Mathf.Abs (dotR)
    ) {

      closestDot = dotU;
      closestAxis = Vector3.up;

    } else {

      closestDot = dotR;
      closestAxis = Vector3.right;

    }

    var side = closestDot >.0f ? 1.0f : -1.0f;

    finalForward = closestAxis * side;

    var finalPos = block.transform.position + finalForward;

    return finalPos;

  }

}