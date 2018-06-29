using UnityEngine;
using UnityEngine.Events;

namespace Generic {

  [System.Serializable]
  public class Vector2Event : UnityEvent<Vector2> { }

  public class TouchComponent : MonoBehaviour {

    public Vector2Event onTouchStart = new Vector2Event ();
    public Vector2Event onTouchMove = new Vector2Event ();
    public Vector2Event onTouchEnd = new Vector2Event ();

    // Camera gameCamera;

    void Start () {

      // gameCamera = Camera.main;

    }

    void Update () {

      if (Input.mousePresent) {
        MouseUpdate ();
        return;
      }

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

    void MouseUpdate () {

      var input = Input.mousePosition;

      if (Input.GetMouseButtonDown (0)) {

          onTouchStart.Invoke (input);

      } else if (Input.GetMouseButton (0)) {
        
        onTouchMove.Invoke (input);

      } else if (Input.GetMouseButtonUp(0)){

        onTouchEnd.Invoke (input);

      }

    }

    void TouchUpdate () {

    }

  }

}