using UnityEngine;
using UnityEngine.Events;

public class RotateController : MonoBehaviour {

  public UnityEvent onRotationStart = new UnityEvent ();

  public UnityEvent onRotationDone = new UnityEvent ();

  public RotateComponent rotateComponent;

  bool isRotating = false;
  bool isSnapping = false;

  Vector3 startForward;
  Vector3 startUp;

  float currAngle;
  float snapAngle;

  public void OnTouchStart () {

    isRotating = true;
    isSnapping = false;
    currAngle = .0f;
    startForward = transform.forward;
    startUp = transform.up;
    onRotationStart.Invoke ();

  }

  public void OnTouchMove (float angle) {

    print ("angle: " + angle);

    currAngle = Mathf.LerpAngle (currAngle, angle, Time.deltaTime * 10.0f);

    RotateToAngle (currAngle);

  }

  public void OnTouchEnd () {

    snapAngle = Mathf.Round (currAngle / 90.0f) * 90.0f;
    isSnapping = true;

  }

  void Update () {

    if (isSnapping) Snap ();

  }

  void Snap () {

    currAngle += (snapAngle - currAngle) * 0.1f;

    if (Mathf.Abs (currAngle - snapAngle) > 0.01f) {

      RotateToAngle (currAngle);

    } else {

      currAngle = snapAngle;
      RotateToAngle (currAngle);
      isRotating = false;
      isSnapping = false;
      var q = transform.rotation.eulerAngles;

      transform.rotation = Quaternion.Euler (
        Mathf.Round (q.x / 90.0f) * 90.0f,
        Mathf.Round (q.y / 90.0f) * 90.0f,
        Mathf.Round (q.z / 90.0f) * 90.0f
      );

      onRotationDone.Invoke ();

    }

  }

  void RotateToAngle (float angle) {

    angle *= -1.0f;

    var forward = Quaternion.AngleAxis (angle, startUp) * startForward;

    transform.LookAt (transform.position + forward, startUp);

  }

  void Start () {

    BindEvents ();

  }

  void BindEvents () {

    // Set points as rotatable
    var containerComponents = GetComponentsInChildren<PointsContainerComponent> ();
    foreach (var containerComponent in containerComponents) {
      var container = containerComponent.pathContainer;

      SetPointsAsRotatable (container);

      container.onGeneratePathPointsDone.AddListener (() => {
        SetPointsAsRotatable (container);
      });

      // Update points before/after rotation
      onRotationDone.AddListener (container.ResetPoints);
      onRotationStart.AddListener (container.onRotationStart);
    }

    var player = Object.FindObjectOfType<PlayerComponent> ();

    if (player == null) return;

    // Enable/disable rotation during player movement.
    player.onTargetReached.AddListener (() => {
      rotateComponent.canRotate = true;
    });
    player.onStartMoving.AddListener (() => {
      rotateComponent.canRotate = false;
    });

  }

  void SetPointsAsRotatable (PathContainer container) {
    foreach (var point in container.points) {
      point.canMove = true;
    }
  }

}