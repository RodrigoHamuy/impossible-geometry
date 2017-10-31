using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection {
	public Vector3 position;
	public Vector3 normal;
	public bool enabled = true;

	public Connection(Vector3 pos, Vector3 up, bool enable = true) {
		position = pos;
		normal = up;
		enabled = enable;
	}
}
