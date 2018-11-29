using UnityEngine;

[System.Serializable]
public class EditableBlockData {

  public string blockType;

  public int id;

  public int rotateControllerId = -1;

  public int parent = -1;

  public Vector3 position;

  public Quaternion rotation;

  public Vector3 scale;

}