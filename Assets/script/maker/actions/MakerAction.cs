using UnityEngine;

public struct MakerAction {
  public MakerActionType type;
  public Transform target;
  public Transform prefab;
  public Vector3 position;
  public Vector3 scale;
  public Quaternion rotation;
  public Transform parent;

  public MakerAction (
    MakerActionType type,
    Transform target,
    Transform prefab,
    Vector3 position,
    Vector3 scale,
    Quaternion rotation,
    Transform parent
  ) {

    this.type = type;
    this.target = target;
    this.prefab = prefab;
    this.position = Utility.Round (position, .5f);
    this.scale = scale;
    this.rotation = Utility.Round (rotation);
    this.parent = parent;

  }
}