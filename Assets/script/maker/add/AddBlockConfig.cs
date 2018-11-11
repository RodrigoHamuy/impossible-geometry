using UnityEngine;

[CreateAssetMenu (fileName = "BlockTypeConfig", menuName = "MyMenu/BlockTypeConfig", order = 1)]
public class AddBlockConfig : ScriptableObject {

  public Transform prefab;
  public bool addOnHold;
  public bool addOnTop;
  public bool isRotationHandle;
  public bool isUnique;

}