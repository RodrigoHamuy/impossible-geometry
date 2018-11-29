using UnityEngine;

public class Connection {
	public Vector3 position;
	public bool enabled = true;

	public Connection(Vector3 pos, bool enable = true) {
		position = pos;
		enabled = enable;
	}
}
