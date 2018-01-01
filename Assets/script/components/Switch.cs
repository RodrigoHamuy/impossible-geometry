using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Switch : MonoBehaviour {

    bool pressed = false;
    public UnityEvent onPress = new UnityEvent();

    private void Start () {
        var container = InitPoint();
        container.onGeneratePathPointsDone.AddListener( () => {
            InitPoint();
        });
    }

    private PathContainer InitPoint() {
        var point = Utility.getPointsAtWorldPos(
            transform.position,
            Utility.CleanNormal(transform.up)
        ).OrderBy(p =>
        {
            return (transform.position - p.position).sqrMagnitude;
        }).ElementAt(0);

        point.switchButton = this;

        return point.container;
    }

    public void Press(){
        if( ! pressed ) {
            pressed = true;
            onPress.Invoke();
        }
    }
}
