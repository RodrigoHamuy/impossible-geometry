﻿using System.Collections;
using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.Events;

public class AddOnHold : MonoBehaviour {

  public Transform world;

  public Transform marker;

  public UnityEvent OnBrushStart;
  public UnityEvent OnBrushEnd;

  public Transform blockPrefab;
  public Transform cubeBoyPrefab;
  public Transform targetPrefab;

  public Vector3 planeNormal = Vector3.up;

  MakerActionsManager actionsManager;

  Vector3 currentPlanePoint;
  Vector2 firstTouchPos;

  bool secondTouchPass = false;

  List<Transform> currentRow = new List<Transform> ();

  Transform cubeBoy;
  Transform target;

  Vector3 lastHitPosNoRound;
  Vector3 lastHitPos;

  Vector2 margin;

  TouchComponent touchComponent;

  void Start () {

    marker.gameObject.SetActive (false);

    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();

    touchComponent = gameObject.GetComponent<TouchComponent> ();

    touchComponent.onTouchStart.AddListener (StartStroke);
    touchComponent.onTouchMove.AddListener (MoveStroke);
    touchComponent.onTouchEnd.AddListener (EndStroke);

    currentPlanePoint = transform.position;

    var cam = Camera.main;

    margin = cam.WorldToScreenPoint (Vector3.up) - cam.WorldToScreenPoint (Vector3.zero);

  }

  public void StartStroke (Vector2 screenPos) {

    marker.gameObject.SetActive (true);

    screenPos += margin;

    bool found;

    var hitPos = GetBlockHitPosition (screenPos, out found);

    if (!found) {

      currentPlanePoint = transform.position;
      hitPos = GetHitPosition (screenPos);

    } else {

      currentPlanePoint = hitPos;

    }

    currentRow.Clear ();

    OnBrushStart.Invoke ();

    firstTouchPos = screenPos;
    secondTouchPass = false;

    if (found) screenPos = Camera.main.WorldToScreenPoint (hitPos);

    screenPos -= margin;

    MoveStroke (screenPos, true);

  }

  public void MoveStroke (Vector2 screenPos) {
    MoveStroke (screenPos, false);
  }

  public void MoveStroke (Vector2 screenPos, bool isFirstTouch) {

    screenPos += margin;

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

    print (screenPos);

    var block = actionsManager.AddBlock (blockPrefab, hitPos);

    marker.transform.position = hitPos;

    currentRow.Add (block);

    if (cubeBoy == null) {

      cubeBoy = Instantiate (cubeBoyPrefab, hitPos + Vector3.up * 0.5f, Quaternion.identity, world).transform;

      cubeBoy.RotateAround (cubeBoy.position, Vector3.up, 180.0f);

    }

    lastHitPosNoRound = GetHitPosition (screenPos, false);
    lastHitPos = hitPos;

  }

  public Vector3 GetDirPosition (Vector2 screenPos) {

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

  public void EndStroke (Vector2 screenPos) {

    screenPos += margin;

    int layer = LayerMask.NameToLayer ("Block");

    currentRow.ForEach ((block) => {

      block.gameObject.layer = layer;

    });

    var hitPos = TouchUtility.HitPosition (screenPos, gameObject);

    if (target == null) {

      target = Instantiate (targetPrefab, hitPos + Vector3.up * 0.5f, Quaternion.identity, world).transform;

    }

    lastHitPosNoRound = Vector3.zero;
    lastHitPos = Vector3.zero;

    OnBrushEnd.Invoke ();
    marker.gameObject.SetActive (false);

  }

  public void SetPlaneNormalUp () {
    planeNormal = Vector3.up;
  }

  public void SetPlaneNormalRight () {
    planeNormal = Vector3.right;
  }

  public void SetPlaneNormalForward () {
    planeNormal = Vector3.forward;
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

      var midBlock = actionsManager.AddBlock (blockPrefab, middlePos);

      midBlock.gameObject.layer = LayerMask.NameToLayer ("maker.newBlock");

      currentRow.Add (midBlock);

    }
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

    string[] layerNames = { "Block" };

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

  public void SetPrefab (Transform element) {
    blockPrefab = element;
  }

}