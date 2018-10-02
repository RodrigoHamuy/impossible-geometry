using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRotator : MonoBehaviour {

  public GameObject rotateController;
  public GameObject rotateHolder;

  public bool canChangeObject = true;

  void Start(){
    rotateController.SetActive(false);
  }

  public void CanChangeObject(bool value){
    canChangeObject = value;
  }

	public void OnTouchEnd(Vector2 touchPos) {

    // already in use
    if(!canChangeObject) return;

    if(rotateHolder.transform.childCount > 0) {
      rotateHolder.GetComponentInChildren<Transform>().parent = null;
    }

		var block = GetBlocksOnTapPos(touchPos);

		if(block){

      rotateController.SetActive(true);
      rotateController.transform.position = block.position;
      block.parent = rotateHolder.transform;

		} else {
      rotateController.SetActive(false);
    }

	}


  public static Transform GetBlocksOnTapPos (Vector3 tapPos) {

    var points = new List<PathPoint> ();
    var ray = Camera.main.ScreenPointToRay (tapPos);
    var layerMask = LayerMask.GetMask ("Block");
    var hits = Physics.RaycastAll (ray, 100.0f, layerMask);
    
    var hitsOrdered = hits.OrderBy( h => h.distance);

    foreach (var hit in hitsOrdered) {

      var block = hit.collider.transform.GetComponent<Transform> ();

			return block;

    }

		return null;

  }
}
