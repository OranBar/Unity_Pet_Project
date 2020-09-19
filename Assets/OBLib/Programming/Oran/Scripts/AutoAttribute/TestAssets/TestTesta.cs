using UnityEngine;
using System.Collections;

public class TestTesta : MonoBehaviour {

	[AutoChildren] public Collider[] colliders;
	[Auto] public Rigidbody myRb;
	[AutoParent] public Collider[] parentColliders;
	[AutoChildren] public Rigidbody childRigidbody;

}
