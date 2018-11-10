using UnityEngine;
using UnityEngine.Events;

public class RotateController : MonoBehaviour {

  public UnityEvent onRotationStart = new UnityEvent ();

  public UnityEvent onRotationDone = new UnityEvent ();

  bool isSnapping = false;

  Vector3 startForward;
  Vector3 startUp;

  float currAngle;
  float snapAngle;

  void Start () {

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

    var emitter = GetComponent<RotateTouchEmitter> ();

    if (emitter) AddRotateTouchEmitter (emitter);

  }

  public void AddRotateTouchEmitter (RotateTouchEmitter emitter) {

    emitter.onRotationStart.AddListener (OnTouchStart);
    emitter.onRotationMove.AddListener (OnTouchMove);
    emitter.onRotationDone.AddListener (OnTouchEnd);

  }

  void OnTouchStart () {

    isSnapping = false;
    currAngle = .0f;
    startForward = transform.forward;
    startUp = transform.up;
    onRotationStart.Invoke ();

  }

  void OnTouchMove (float angle) {

    // print ("angle: " + angle);

    currAngle = Mathf.LerpAngle (currAngle, angle, Time.deltaTime * 10.0f);

    RotateToAngle (currAngle);

  }

  void OnTouchEnd () {

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
      isSnapping = false;
      transform.rotation = Utility.Round (transform.rotation);
      print (transform.rotation.eulerAngles);
      onRotationDone.Invoke ();

    }

  }

  void RotateToAngle (float angle) {

    angle *= -1.0f;

    var forward = Quaternion.AngleAxis (angle, startUp) * startForward;

    transform.LookAt (transform.position + forward, startUp);

  }

  void SetPointsAsRotatable (PathContainer container) {
    foreach (var point in container.points) {
      point.canMove = true;
    }
  }

}