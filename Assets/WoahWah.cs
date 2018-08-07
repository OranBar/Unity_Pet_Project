using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoahWah : MonoBehaviour
{

	private Camera myCamera;
	
	// Use this for initialization
	void Start ()
	{
		myCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.LeftAlt) && Input.mouseScrollDelta.y != 0)
		{
			int sign = Math.Sign(Input.mouseScrollDelta.y);
			myCamera.orthographicSize += -sign;
			if (myCamera.orthographicSize <= 0)
			{
				myCamera.orthographicSize = 1;
			}
		}	
	}
}
