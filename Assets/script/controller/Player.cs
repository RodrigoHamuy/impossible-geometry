// ﻿using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class Player {

	// PlayerComponent component;

	public Vector3 normal;

	public Player( PlayerComponent c ) {
		// component = c;
		normal = PathPoint.CleanNormal( c.transform.up );
	}
}
