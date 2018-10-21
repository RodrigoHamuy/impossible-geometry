using UnityEngine;

public class Marker : MonoBehaviour {

  MeshRenderer myRenderer;

  void Start () {

    myRenderer = GetComponent<MeshRenderer> ();
    myRenderer.enabled = false;

  }

  public void OnBlockAdded (Vector3 pos, Vector3 dir) {
    
    transform.position = pos;

  }

  public void OnBlockRemoved (Vector3 pos, Vector3 dir) {

    transform.position = pos - dir;

  }

}