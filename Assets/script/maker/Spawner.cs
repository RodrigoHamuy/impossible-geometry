using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maker {

  public class Spawner : MonoBehaviour {

    public Transform blockPrefab;
    public Transform blockPreviewPrefab;

    public SpawnerMode mode = SpawnerMode.Block;

    Renderer blockPreview;

    Camera _camera;

    void Start () {

      _camera = Camera.main;

      blockPreview = Instantiate (blockPreviewPrefab, Vector3.zero, Quaternion.identity).GetComponent<Renderer> ();

      blockPreview.enabled = false;

    }

    public void SetMode (SpawnerMode mode) {

      this.mode = mode;

    }

    public void StartBlockPreview (Vector2 screenPos) {

      blockPreview.transform.position = hitPosition (screenPos);
      blockPreview.enabled = true;

    }

    public void MoveBlockPreview (Vector2 screenPos) {

      blockPreview.transform.position = hitPosition (screenPos);

    }

    public void AddBlock (Vector2 screenPos) {

      blockPreview.enabled = false;

      var pos = hitPosition (screenPos);

      var block = Instantiate (blockPrefab, pos, Quaternion.identity);

    }

    Vector3 hitPosition (Vector3 screenPos) {

      screenPos.z = transform.position.z;

      var ray = _camera.ScreenPointToRay (screenPos);

      string[] layerName = new string[] {
        LayerMask.LayerToName (gameObject.layer),
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

}