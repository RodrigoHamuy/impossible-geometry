using UnityEngine;

public class TouchUtility {

  public static Vector3 HitPosition (Vector3 screenPos, GameObject plane, bool includeBlocks = false) {

    var camera = Camera.main;

    screenPos.z = plane.transform.position.z;

    var ray = camera.ScreenPointToRay (screenPos);

    string[] layerName = new string[] {
      LayerMask.LayerToName (plane.layer),
      // "Block"
    };
    
    if(includeBlocks) layerName.push("Block");

    var layerMask = LayerMask.GetMask (layerName);

    RaycastHit hit;

    Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask);

    var pos = hit.point;

    for (int i = 0; i < 3; i++) {

      pos[i] = Mathf.Round (pos[i]) + .5f;

    }

    return pos;

  }

}
