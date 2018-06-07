using UnityEngine;
using UnityEngine.Events;

namespace Generic {

  [System.Serializable]
  public class Vector3Event : UnityEvent<Vector3> { }

  public class TouchComponent : MonoBehaviour {

    public Vector3Event onTouchStart = new Vector3Event ();
    public Vector3Event onTouchEnd = new Vector3Event ();
    public Vector3Event onTouchMove = new Vector3Event ();

    bool wasDown = false;

    // Camera gameCamera;

    void Start () {

      // gameCamera = Camera.main;

    }

    void Update () {

      // if (Input.mousePresent) {
      MouseUpdate ();
      // }

    }

    void MouseUpdate () {

      var input = Input.mousePosition;
      RaycastHit hit;

      if (Input.GetMouseButtonDown (0)) {

        print ("input");
        print (input);

        hitPos (input, out hit);

        if (wasDown) {

          onTouchMove.Invoke (hit.point);

        } else {

          wasDown = true;
          onTouchStart.Invoke (hit.point);
          print ("onTouchStart");

        }

      } else if (wasDown) {

        hitPos (input, out hit);

        wasDown = false;
        onTouchEnd.Invoke (hit.point);

      }

    }

    bool hitPos (Vector2 screenPos, out RaycastHit hit) {

      var camera = Camera.main;

      var ray = camera.ScreenPointToRay (screenPos);

      var layer = LayerMask.LayerToName (gameObject.layer);

      var layerMask = LayerMask.GetMask (layer);

      var didHit = Physics.Raycast (
        Camera.main.transform.position,
        ray.direction,
        out hit,
        Mathf.Infinity,
        layerMask
      );

      return didHit;

    }

    void TouchUpdate () {

      if (Input.touchCount == 0) return;

      var touch = Input.GetTouch (0);

      switch (touch.phase) {
        case TouchPhase.Began:
          onTouchStart.Invoke (touch.position);
          break;
        case TouchPhase.Ended:
          onTouchEnd.Invoke (touch.position);
          break;
        case TouchPhase.Moved:
          onTouchMove.Invoke (touch.position);
          break;
      }

    }

  }

}