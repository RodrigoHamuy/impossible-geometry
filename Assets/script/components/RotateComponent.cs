using UnityEngine;
using UnityEngine.Events;

enum RotationPhase {
  Idle,
  Start,
  Move,
  Snap
}

public class RotateComponent : MonoBehaviour {

  Collider handleCollider;

  public UnityEvent onRotationStart = new UnityEvent ();
  public UnityEvent onCanRotateChange = new UnityEvent ();
  public UnityEvent onRotationDone = new UnityEvent ();

  public bool canRotate = true;

  RotationPhase phase = RotationPhase.Idle;

  Vector2 startVector;

  void Start () {

    BindEvents ();

  }

  private void setPointsAsRotatable (PathContainer container) {
    foreach (var point in container.points) {
      point.canMove = true;
    }
  }

  void Update () {

    switch (phase) {
      case RotationPhase.Idle:
        CheckTouchStart ();
        break;
      case RotationPhase.Start:
        break;
      case RotationPhase.Move:
        break;
      case RotationPhase.Snap:
        break;
    }

  }

  void CheckTouchStart () {

    Vector2 inputPos = Utility.getTouchStart ();

    if (inputPos == Vector2.zero) return;

    var ray = Camera.main.ScreenPointToRay (inputPos);

    RaycastHit hit;

    var doesHit = handleCollider.Raycast (ray, out hit, Mathf.Infinity);

    if (!doesHit) return;

    Vector2 handleScreenPos = Camera.main.WorldToScreenPoint (handleCollider.transform.position);

    startVector = inputPos - handleScreenPos;

    phase = RotationPhase.Start;
    onRotationStart.Invoke ();

  }

  void BindEvents () {

    // Set points as rotatable
    var containerComponents = GetComponentsInChildren<PointsContainerComponent> ();
    foreach (var containerComponent in containerComponents) {
      var container = containerComponent.pathContainer;

      setPointsAsRotatable (container);

      container.onGeneratePathPointsDone.AddListener (() => {
        setPointsAsRotatable (container);
      });

      // Update points before/after rotation
      onRotationDone.AddListener (container.ResetPoints);
      onRotationStart.AddListener (container.onRotationStart);
    }

    var handle = GetComponentsInChildren<RotateHandleComponent> () [0];

    handleCollider = handle.GetComponent<Collider> ();

    var player = Object.FindObjectOfType<PlayerComponent> ();

    if (player == null) return;

    // Enable/disable rotation during player movement.
    player.onTargetReached.AddListener (() => {
      canRotate = true;
      onCanRotateChange.Invoke ();
    });
    player.onStartMoving.AddListener (() => {
      canRotate = false;
      onCanRotateChange.Invoke ();
    });

  }
}