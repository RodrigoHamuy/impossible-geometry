using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maker {

  public class Spawner : MonoBehaviour {

    public Transform blockPrefab;
    public Transform blockPreviewPrefab;

    Renderer blockPreview;

    Camera _camera;

    void Start () {

      _camera = Camera.main;

      blockPreview = Instantiate (blockPreviewPrefab, Vector3.zero, Quaternion.identity).GetComponent<Renderer> ();

      blockPreview.enabled = false;

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

      /*
      
      A: Camera
      B: Floor
      C: Ray hitting floor

          A
          .
           \
            \
             \
      c       \   b
               \
                \
          -------# 

         B   a    C

      */

      var ray = _camera.ScreenPointToRay (screenPos);
      var A = ray.origin;
      var B = A;
      B.y = transform.position.y;
      var AtoB = B - A;
      var c = AtoB.magnitude;
      var AtoCDir = ray.direction;
      var angleA = Vector3.Angle (AtoB, AtoCDir);
      var angleB = 90;
      var angleC = 180 - angleA - angleB;
      var b = Utility.sin (angleB) * (c / Utility.sin (angleC));
      var C = A + AtoCDir * b;
      return C;

    }

  }

}