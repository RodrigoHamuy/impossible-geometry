using UnityEngine;

public class PlayerComponent : MonoBehaviour {

	public Player controller;

	public bool isMoving = false;

	void Start() {
		controller = new Player(this);
	}

}
