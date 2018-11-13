using UnityEngine;
using UnityEngine.Events;

public class RotateTouchEmitter : MonoBehaviour {

  public UnityEventBool onCanRotateChange = new UnityEventBool ();

  [HideInInspector]
  public UnityEvent onRotationStart = new UnityEvent ();
  [HideInInspector]
  public UnityEventFloat onRotationMove = new UnityEventFloat ();
  [HideInInspector]
  public UnityEvent onRotationDone = new UnityEvent ();

  public SphereCollider handleCollider;

  public bool canRotate {
    get;
    private set;
  }

  Transform handler;

  Vector3 startVector;

  RotationPhase phase = RotationPhase.Idle;

  void Start () {

    handler = handleCollider.transform;
    canRotate = true;

    var player = Object.FindObjectOfType<PlayerComponent> ();

    if (player == null) return;

    // Enable/disable rotation during player movement.
    player.onTargetReached.AddListener (() => {
      CanRotate (true);
    });
    player.onStartMoving.AddListener (() => {
      CanRotate (false);
    });

  }

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

  public void CanRotate (bool val) {
    canRotate = val;
    onCanRotateChange.Invoke (canRotate);
  }

  void CheckTouchStart () {

    if (!canRotate) return;

    Vector2 inputPos = Utility.getTouchStart ();

    if (inputPos == Vector2.zero) return;

    var ray = Camera.main.ScreenPointToRay (inputPos);

    RaycastHit hit;

    var doesHit = handleCollider.Raycast (ray, out hit, Mathf.Infinity);

    if (!doesHit) return;

    startVector = GetTouchDirFromPlaneCenter (inputPos);

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

    var currVector = GetTouchDirFromPlaneCenter (inputPos);

    var angle = Vector3.SignedAngle (startVector, currVector, -handler.up);

    onRotationMove.Invoke (angle);

  }

  Vector3 GetTouchDirFromPlaneCenter (Vector2 inputPos) {

    var normal = handler.up;

    var center = handler.position + transform.rotation * handleCollider.center;

    var plane = new Plane (normal, center);

    var ray = Camera.main.ScreenPointToRay (inputPos);

    float dist;

    plane.Raycast (ray, out dist);

    return ray.origin + ray.direction * dist - center;

  }

}