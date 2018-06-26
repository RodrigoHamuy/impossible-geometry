using UnityEngine;
using UnityEngine.Events;

public class RotateComponent : MonoBehaviour {

  public UnityEventBool onCanRotateChange = new UnityEventBool ();

  public UnityEvent onRotationStart = new UnityEvent ();
  public UnityEventFloat onRotationMove = new UnityEventFloat ();
  public UnityEvent onRotationDone = new UnityEvent ();

  public Collider handleCollider;
  
  public bool canRotate {

    get { return _canRotate; }

    set {
      _canRotate = value;
      onCanRotateChange.Invoke (_canRotate);
    }

  }

  bool _canRotate = true;

  public RotationPhase phase = RotationPhase.Idle;

  Vector2 startVector;

  void Update () {

    switch (phase) {
      case RotationPhase.Idle:
        CheckTouchStart ();
        break;
      case RotationPhase.Move:
        OnTouchMove ();
        break;
    }

  }

  void CheckTouchStart () {

    if (!_canRotate) return;

    Vector2 inputPos = Utility.getTouchStart ();

    if (inputPos == Vector2.zero) return;

    var ray = Camera.main.ScreenPointToRay (inputPos);

    RaycastHit hit;

    var doesHit = handleCollider.Raycast (ray, out hit, Mathf.Infinity);

    if (!doesHit) return;

    startVector = TouchFromCenterVector (inputPos);

    phase = RotationPhase.Move;
    onRotationStart.Invoke ();

  }

  void OnTouchMove () {

    var inputPos = Utility.getTouchEnd ();

    if (inputPos == Vector2.zero) {

      inputPos = Utility.getTouch ();

    } else {

      phase = RotationPhase.Idle;
      onRotationDone.Invoke ();
      return;

    }

    Vector2 handleScreenPos = Camera.main.WorldToScreenPoint (handleCollider.transform.position);

    var currVector = TouchFromCenterVector (inputPos);

    var angle = Vector2.SignedAngle (startVector, currVector);

    onRotationMove.Invoke (angle);

  }

  Vector2 TouchFromCenterVector (Vector2 inputPos) {

    Vector2 handleScreenPos = Camera.main.WorldToScreenPoint (handleCollider.transform.position);

    return inputPos - handleScreenPos;

  }

}