using UnityEngine;

public struct MakerAction {
  public MakerActionType type;
  public Transform target;
  public Vector3 position;
  public Vector3 scale;
  public Quaternion rotation;
  public int historyIndex;

  public MakerAction (
    MakerActionType type,
    Transform target,
    Vector3 position,
    Vector3 scale,
    Quaternion rotation,
    int index
  ) {

    this.type = type;
    this.target = target;
    this.position = position;
    this.scale = scale;
    this.rotation = rotation;
    this.historyIndex = index;

  }
}