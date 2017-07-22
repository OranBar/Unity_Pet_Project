using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttributeTest : MonoBehaviour {

	[Auto] public Rigidbody rb;
	[Auto] public AAAA script;
	[Auto] public Collider myCollider { get; set; } 

	[Auto(true)] public Camera myCamera;
	[Auto(true)] public Light myLight;


	[Auto]
	[SerializeField]
	//[MyOtherAttribute]
	private string myString;
}
