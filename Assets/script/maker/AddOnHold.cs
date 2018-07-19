﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AddOnHold : MonoBehaviour {

  public UnityEvent2Vector3 OnBlockAdded;
  public UnityEvent2Vector3 OnBlockRemoved;

  public UnityEvent OnBrushStart;
  public UnityEvent OnBrushEnd;

  public Transform blockPrefab;
  public Transform cubeBoyPrefab;
  public Transform targetPrefab;

  Vector3 currentPlanePoint;

  bool isPainting = false;

  List<Transform> currentRow = new List<Transform> ();

  Transform cubeBoy;
  Transform target;

  void Start () {

    currentPlanePoint = transform.position + Vector3.up * .5f;

  }

  public void StartStroke (Vector2 screenPos) {

    bool found;

    var hitPos = GetBlockHitPosition (screenPos, out found);

    if (!found) hitPos = GetHitPosition (screenPos);

    print ("hitPos: " + hitPos);

    print ("found: " + found);

    currentPlanePoint = hitPos;

    isPainting = true;

    currentRow.Clear ();

    OnBrushStart.Invoke ();

    if(found) screenPos = Camera.main.WorldToScreenPoint(hitPos);
 
    MoveStroke (screenPos);

  }

  public void MoveStroke (Vector2 screenPos) {

    var dir = Vector3.zero;

    var hitPos = GetHitPosition (screenPos);

    if (currentPlanePoint.y != hitPos.y) return;

    var spaceTaken = currentRow.Exists ((Transform oldBlock) => {

      return oldBlock.position == hitPos;

    });

    if (
      currentRow.Count > 1 &&
      currentRow[currentRow.Count - 2].position == hitPos
    ) {

      var lastBlock = currentRow[currentRow.Count - 1];
      currentRow.RemoveAt (currentRow.Count - 1);

      OnBlockRemoved.Invoke (lastBlock.position, GetLastBlockDirection ());
      Destroy (lastBlock.gameObject);

      return;

    }

    if (spaceTaken) return;

    CheckCornerBlock (hitPos);

    var block = Instantiate (blockPrefab, hitPos, Quaternion.identity);

    // block.gameObject.layer = LayerMask.NameToLayer ("maker.newBlock");

    currentRow.Add (block);

    OnBlockAdded.Invoke (hitPos, GetLastBlockDirection ());

    if (cubeBoy == null) {

      cubeBoy = Instantiate (cubeBoyPrefab, hitPos + Vector3.up * 0.5f, Quaternion.identity).transform;

      cubeBoy.RotateAround (cubeBoy.position, Vector3.up, 180.0f);

    }

    currentPlanePoint = transform.position;

  }

  public void EndStroke (Vector2 screenPos) {

    isPainting = false;

    int layer = LayerMask.NameToLayer ("Block");

    currentRow.ForEach ((block) => {

      block.gameObject.layer = layer;

    });

    var hitPos = TouchUtility.HitPosition (screenPos, gameObject);

    if (target == null) {

      target = Instantiate (targetPrefab, hitPos + Vector3.up * 0.5f, Quaternion.identity).transform;

    }

    OnBrushEnd.Invoke ();

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

      var midBlock = Instantiate (blockPrefab, middlePos, Quaternion.identity);

      midBlock.gameObject.layer = LayerMask.NameToLayer ("maker.newBlock");

      currentRow.Add (midBlock);
      OnBlockAdded.Invoke (middlePos, GetLastBlockDirection ());

    }
  }

  Vector3 GetHitPosition (Vector2 screenPos) {

    var cam = Camera.main;

    var ray = cam.ScreenPointToRay (screenPos);

    var plane = new Plane (Vector3.up, currentPlanePoint);

    float enter;

    plane.Raycast (ray, out enter);

    var worldPos = ray.GetPoint (enter);

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

    print ("Dir: " + finalForward);

    return finalPos;

  }

}