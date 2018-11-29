using System.Collections.Generic;
using UnityEngine;

public struct MakerAction {
  public MakerActionType type;
  public Transform target;
  public MakerBlockType blockType;
  public Vector3 position;
  public Vector3 scale;
  public Quaternion rotation;
  public Transform parent;
  public int id;

  public MakerAction (
    MakerActionType type,
    Transform target,
    MakerBlockType blockType,
    Vector3 position,
    Vector3 scale,
    Quaternion rotation,
    Transform parent
  ) {

    this.type = type;
    this.target = target;
    this.blockType = blockType;
    this.position = Utility.Round (position, 1.0f);
    this.scale = scale;
    this.rotation = Utility.Round (rotation);
    this.parent = parent;

    id = -1;

  }
}