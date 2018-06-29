using System.Collections.Generic;
using UnityEngine;

public class TouchUtility {

  public static Vector3 HitPosition (Vector3 screenPos, GameObject plane, bool includeBlocks = false) {

    var camera = Camera.main;

    screenPos.z = plane.transform.position.z;

    var ray = camera.ScreenPointToRay (screenPos);

    var layerName = new List<string>() {
      LayerMask.LayerToName (plane.layer),
    };
    
    if(includeBlocks) layerName.Add("Block");

    var layerMask = LayerMask.GetMask (layerName.ToArray());

    RaycastHit hit;

    Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask);

    var pos = hit.point;

    for (int i = 0; i < 3; i++) {

      pos[i] = Mathf.Round (pos[i]) + .5f;

    }

    return pos;

  }

}
