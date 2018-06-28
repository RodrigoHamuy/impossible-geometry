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

  Vector3 startVector;

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

    var currVector = TouchFromCenterVector (inputPos);

    print (currVector);

    if (startVector != currVector) print (currVector);

    var angle = Vector3.SignedAngle (startVector, currVector, transform.up);

    onRotationMove.Invoke (angle);

  }

  Vector3 TouchFromCenterVector (Vector2 inputPos) {

    var center = handleCollider.transform.position;

    var inputWorldPos = Camera.main.ScreenToWorldPoint (inputPos);

    var dir = Vector3.Project (inputWorldPos - center, handleCollider.transform.up).normalized;

    for (int i = 0; i < 3; i++) {

      dir[i] = Mathf.Round (dir[i] / .01f) * .01f;

    }

    return dir;

  }

}