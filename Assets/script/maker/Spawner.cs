using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maker {

  public class Spawner : MonoBehaviour {

    public GameObject block;

    Camera _camera;

    void Start () {
      _camera = Camera.main;
    }

    public void AddBlock (Vector3 pos) {

      block.transform.position = pos;
      print (pos);

    }

  }

}