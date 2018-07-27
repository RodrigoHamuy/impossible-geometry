using UnityEngine;

class FollowMouse : MonoBehaviour{
  
  Plane plane;

  void Start(){

    plane = new Plane(Vector3.up, transform.position);

  }

  public void UpdatePosition(Vector2 pos){

    var ray = Camera.main.ScreenPointToRay(pos);

    float distance;

    plane.Raycast(ray, out distance);

    transform.position = ray.GetPoint(distance);

  }
    
}