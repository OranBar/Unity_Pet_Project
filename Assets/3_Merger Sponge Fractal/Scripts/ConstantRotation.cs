using UnityEngine;
using System.Collections;

public class ConstantRotation : MonoBehaviour {

	public Vector3 rotation = new Vector3(1f,1f,1f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Rotate(rotation);
	}
}
