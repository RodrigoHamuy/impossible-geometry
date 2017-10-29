using UnityEngine;

public class PathPointComponent : MonoBehaviour {

	public Transform upPrefab;

  public PathPoint point;

  void Start() {

    point.normals.ForEach( (Vector3 normal) => {

      Instantiate(
      upPrefab,
      point.position + normal*0.15f,
      Quaternion.identity
      );

    });

  }

  public void OnTap(){

    Renderer rend = GetComponent<Renderer>();

    var color = rend.material.color;

    color.a = 1.0f;

    rend.material.color = color;

  }

}
