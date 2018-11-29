using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchRotate : MonoBehaviour {

    public UnityEvent onMoveDone = new UnityEvent();

	float _current = 0;
	float _target = 90.0f;
    bool _isMoving = false;

	Quaternion _originalRotation;

	Transform handle;

	private void Start() {
		handle = transform.Find("Handle");
		InitPoints();
	}

	public void StartMoving( float target ){
		_target = target;
		_isMoving = true;
		_originalRotation = transform.rotation;
	}

	private void Update() {
        if (_isMoving) MoveStep();
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

	private void MoveStep() {
        if (_current < _target - 0.001f) {
            _current += Mathf.Max(_target - _current, .2f) * Time.deltaTime * 1.5f;
        } else {
            _current = _target;
            _isMoving = false;
            Utility.canPlayerMove = true;
        }

		transform.rotation = _originalRotation;

		transform.RotateAround( 
			handle.position,
			handle.up,
			_current
		);

        if ( ! _isMoving ) {
            onMoveDone.Invoke();
        }
    }
	
}
