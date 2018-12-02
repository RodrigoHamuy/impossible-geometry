using UnityEngine;

[CreateAssetMenu (fileName = "BlockTypeConfig", menuName = "MyMenu/BlockTypeConfig", order = 1)]
public class MakerBlockType : ScriptableObject {

  public Transform prefab;
  public string prefabName;
  public bool addOnHold;
  public bool addOnTop;
  public bool isRotationHandle;
  public bool isUnique;

  public static class Names {

    public static string CubeMaker = "CubeMaker";
    public static string HalfCubeMaker = "HalfCubeMaker";
    public static string PlayerMaker = "PlayerMaker";
    public static string TargetMaker = "TargetMaker";
    public static string RotateHandleMaker = "RotateHandleMaker";
    public static string EmptyRotateControllerMaker = "EmptyRotateControllerMaker";

  }

}