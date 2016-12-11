using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class AutomatonAgent_SeekAndStop : MonoBehaviour {

	public Transform targetEntity;
	public float maxSpeed = 100f;

	private Rigidbody myRigidBody;
	private Vector3 previousTargetPosition;

	void Start () {
		myRigidBody = GetComponent<Rigidbody>();
		myRigidBody.useGravity = false;
		previousTargetPosition = targetEntity.position;
	}

	void Update () {
		Seek();

		this.transform.LookAt(targetEntity);
	}

	public void Seek(){
		Vector3 target = Seek_ChooseTarget();
		Vector3 desiredVelocity = Seek_GetSteering(target);
		Seek_Locomotion(desiredVelocity);
		previousTargetPosition = this.targetEntity.position;
	}

	private Vector3 Seek_ChooseTarget(){
		return targetEntity.position + (targetEntity.position - previousTargetPosition);
	}

	private Vector3 Seek_GetSteering(Vector3 target){
		Vector3 vectorToTarget = target - this.transform.position;

		//Take the min between the max speed, or the magnitude vector from my position in the next frame to my target
		Vector3 desiredVelocity = vectorToTarget.normalized * 
			Mathf.Min((vectorToTarget-myRigidBody.velocity).magnitude, maxSpeed);

		Vector3 steer = desiredVelocity - myRigidBody.velocity;

		return steer;
	}

	private void Seek_Locomotion(Vector3 desiredVelocity){
		myRigidBody.AddForce(desiredVelocity);
	}

}
