using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Door : MonoBehaviour {
	
	void Start () {
		var point = Utility.getPointsAtWorldPos(
            transform.position,
            Utility.CleanNormal(transform.up)
        ).OrderBy(p =>
        {
            return (transform.position - p.position).sqrMagnitude;
        }).ElementAt(0);

		point.door = this;
		
	}
}
