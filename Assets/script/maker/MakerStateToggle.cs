using UnityEngine;

public class MakerStateToggle : MonoBehaviour{

  public MakerStateManager stateManager;

  public MakerState enableOn;

  void Start(){
    stateManager.OnStateChange.AddListener( state => { CheckState(); } );
    CheckState();
  }

  void CheckState(){
    if(stateManager.state == enableOn) gameObject.SetActive(true);
    else gameObject.SetActive(false);
  }

}