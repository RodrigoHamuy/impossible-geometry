using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Btn", menuName = "MyMenu/MakerBtnList", order = 1)]

public class MakerBtnListDataType : ScriptableObject {

  public MakerBtnDataType[] btns;

}