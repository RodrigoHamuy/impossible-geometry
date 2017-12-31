using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchSlide : MonoBehaviour {

	public UnityEvent onMoveDone = new UnityEvent();

    public Vector3 dir = Vector3.up;

	float _currPos = 0;
	float _target = 0;
    bool _isMoving = false;

	Vector3 origin;

	public void MoveTo(float step) {
        _target = _currPos + step;
		_isMoving = true;
        Utility.canPlayerMove = false;
    }

	private void Start(){
		origin = transform.position;
		InitPoints();
    }

    private void InitPoints() {

		var containerComponents = new List<PointsContainerComponent>(
            GetComponentsInChildren<PointsContainerComponent>()
        );

		foreach (var containerComponent in containerComponents) {
            var container = containerComponent.pathContainer;

            SetContainerAsMovable(container);

			onMoveDone.AddListener(() => {
                container.ResetPoints();
                SetContainerAsMovable(container);
            });

        }

	}

	private void SetContainerAsMovable(PathContainer container) {
        foreach (var point in container.points) {
            point.canMove = true;
        }
    }

	private void Update(){
		if( _isMoving ) MoveStep();
	}

	private void MoveStep(){
		if (_currPos < _target - 0.001f) {
            _currPos += Mathf.Max(_target - _currPos, .2f) * Time.deltaTime * 1.5f;
        } else {
            _currPos = _target;
			_isMoving = false;
			Utility.canPlayerMove = true;
        }
        transform.position = origin + dir * _currPos;

		if( ! _isMoving ) {
			onMoveDone.Invoke();
		}
	}

	
}
