using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class AutomatonAgent : MonoBehaviour {

	public Transform target;
	public float maxSpeed = 100f;

	private Rigidbody myRigidBody;

	// Use this for initialization
	void Start () {
		myRigidBody = GetComponent<Rigidbody>();
		myRigidBody.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
		Seek();

		this.transform.LookAt(target);
	}

	public void Seek(){
		Vector3 target = Seek_ChooseTarget();
		Vector3 desiredVelocity = Seek_GetSteering(target);
		Seek_Locomotion(desiredVelocity);
	}

	private Vector3 Seek_ChooseTarget(){
		return target.position;
		
	}

	/** <summary Returns desired velocity />
	* 
	*/
	private Vector3 Seek_GetSteering(Vector3 target){
		Vector3 vectorToTarget = target - this.transform.position;
		Vector3 desiredVelocity = vectorToTarget.normalized * maxSpeed;
		Vector3 steer = desiredVelocity - myRigidBody.velocity;

		return steer;
	}

	private void Seek_Locomotion(Vector3 desiredVelocity){
		myRigidBody.AddForce(desiredVelocity);
	}

}
