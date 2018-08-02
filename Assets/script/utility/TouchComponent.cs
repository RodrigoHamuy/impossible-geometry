using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Generic {

  [System.Serializable]
  public class Vector2Event : UnityEvent<Vector2> { }

  public class TouchComponent : MonoBehaviour {

    public Vector2Event onTouchStart = new Vector2Event ();
    public Vector2Event onTouchMove = new Vector2Event ();
    public Vector2Event onTouchEnd = new Vector2Event ();

    bool mouseDown = false;
    bool touchDown = false;

    void Update () {

      if (Input.touchSupported) TouchUpdate();

      else if (Input.mousePresent) MouseUpdate ();

    }

    void TouchUpdate () {

      if (Input.touchCount == 0) return;

      var touch = Input.GetTouch (0);

      switch (touch.phase) {

        case TouchPhase.Began:

          if ( ! EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
            onTouchStart.Invoke (touch.position);
            touchDown = true;
          }
          break;

        case TouchPhase.Ended:       

          if(touchDown) onTouchEnd.Invoke (touch.position);
          touchDown = false;
          break;

        case TouchPhase.Moved:

          if(touchDown) onTouchMove.Invoke (touch.position);
          break;

      }

    }

    void MouseUpdate () {

      var input = Input.mousePosition;

      if (Input.GetMouseButtonDown (0)) {

        if ( ! EventSystem.current.IsPointerOverGameObject()){
          onTouchStart.Invoke (input);
          mouseDown = true;
        }

      } else if (Input.GetMouseButton (0)) {

        if(mouseDown) onTouchMove.Invoke (input);

      } else if (Input.GetMouseButtonUp (0)) {

        if(mouseDown) onTouchEnd.Invoke (input);

        mouseDown = false;

      }

    }

  }

}