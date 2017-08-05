using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformEx {

	public static void SetX(this Transform t, float x)
	{
		Vector3 tmp = t.position;
		tmp.x = x;
		t.position = tmp;
	}
}
