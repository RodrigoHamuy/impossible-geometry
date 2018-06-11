using UnityEngine;

public class TouchUtility {

  public static Vector3 HitPosition (Vector3 screenPos, GameObject plane) {

    var camera = Camera.main;

    screenPos.z = plane.transform.position.z;

    var ray = camera.ScreenPointToRay (screenPos);

    string[] layerName = new string[] {
      LayerMask.LayerToName (plane.layer),
      "Block"
    };

    var layerMask = LayerMask.GetMask (layerName);

    RaycastHit hit;

    Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask);

    var pos = hit.point;

    for (int i = 0; i < 3; i++) {

      float round = 0;

      if (pos[i] >= 0) round = .5f;

      else round = -.5f;

      if (i == 1) pos[i] = Mathf.Round (pos[i]) + round;

      else pos[i] = Mathf.Floor (pos[i]) + .5f;

    }

    return pos;

  }

}