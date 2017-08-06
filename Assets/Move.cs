using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.transform.position = new Vector3(5, 0, 0);
		}
	}
}
