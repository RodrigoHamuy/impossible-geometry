using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Door : MonoBehaviour {

    public Door conn;

    PathPoint _point;

    public PathPoint point{ 
        get{
            return _point;
        }
    }
	
	void Start () {
		_point = Utility.getPointsAtWorldPos(
            transform.position,
            Utility.CleanNormal(transform.up)
        ).OrderBy(p =>
        {
            return (transform.position - p.position).sqrMagnitude;
        }).ElementAt(0);

		_point.door = this;
		
	}
}
