using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Switch : MonoBehaviour {

    bool pressed = false;

	public SlideComponent slideBlock;
    
	void Start () {
        var point = Utility.getPointsAtWorldPos(
            transform.position,
            Utility.CleanNormal(transform.up)
        ).OrderBy(p => {
            return (transform.position - p.position).sqrMagnitude;
        }).ElementAt(0);

        point.switchButton = this;

    }

    public void Press(){
        if( ! pressed ) {
            pressed = true;
            print("pressed");
        }
    }
}
