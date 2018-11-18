using UnityEngine;

[CreateAssetMenu (fileName = "BlockTypeConfig", menuName = "MyMenu/BlockTypeConfig", order = 1)]
public class MakerBlockType : ScriptableObject {

  public Transform prefab;
  public string prefabName;
  public bool addOnHold;
  public bool addOnTop;
  public bool isRotationHandle;
  public bool isUnique;

}