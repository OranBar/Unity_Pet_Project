using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMultiple_Tester : MonoBehaviour {

	[AutoChildren] public Transform[] myChildren;

	// Use this for initialization
	void Start () {
		Debug.Log(myChildren[1].name);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
