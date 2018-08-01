using UnityEngine;
using UnityEngine.UI;

class ToogleImageColour : MonoBehaviour{

  public Image[] siblings;

  Image image;

  void Start(){

    image = GetComponent<Image>();

  }

  public void ActivateBtn(){

    image.color = Color.white;

    foreach (var sibling in siblings) {
      sibling.color = new Color(.821f, .82f, .82f, .76f);
        
    }

  }

}