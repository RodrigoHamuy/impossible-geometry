using UnityEngine;

[CreateAssetMenu (fileName = "Tutorial Objective", menuName = "MyMenu/Tutorial Objective", order = 1)]

public class TutorialObjective : ScriptableObject {

  public string id;
  public string text;
  public Sprite icon;

}