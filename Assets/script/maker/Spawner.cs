using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maker {

  public class Spawner : MonoBehaviour {

    public GameObject block;

    Camera _camera;

    void Start () {
      _camera = Camera.main;
    }

    public void AddBlock (Vector2 pos) {


      var newPos = screenToWorld (pos);

      block.transform.position = newPos;
      print ("start: " + newPos);

    }

    Vector3 screenToWorld (Vector3 screenPos) {

      // A: Camera
      // B: Floor
      // C: Ray hitting floor

      /*
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

      var A = _camera.transform.position;

      var B = A;
      B.y -= transform.position.y;

      var AtoB = B - A;

      var c = AtoB.magnitude;

      var AtoCDir = _camera.ScreenPointToRay (screenPos).direction;

      var angleA = Vector3.Angle (AtoB, AtoCDir);

      var angleB = 90;

      var angleC = 180 - angleA - angleB;

      var b = Utility.sin (angleB) * (c / Utility.sin (angleC));

      var C = A + AtoCDir * b;

      return C;

    }

  }

}