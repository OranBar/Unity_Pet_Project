using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBB : MonoBehaviour {

	public int myInt = 3;

	private bool myBool;

	// Use this for initialization
	//ChangeMaterialColor
	public void CMC () {
		Color color = (myBool) ? Color.blue : Color.yellow;
		myBool = !myBool;
		GetComponent<Renderer>().material.color = color;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
