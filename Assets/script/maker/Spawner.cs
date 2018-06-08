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
      print ("start: " + pos);

    }

    Vector3 screenToWorld (Vector3 screenPos) {

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