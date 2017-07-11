using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticJoint2D : MonoBehaviour {

	//Rotates on only 1 axis
	public Vector3 axis;
	public Vector3 startOffset;
	
	[Tooltip("Delta angle per second")]
	public float moveSpeed = 0.25f;

	[Tooltip("Delta angle per second")]
	public float turnSpeed = 5f;

	private float moveIncrement;
	private float angleIncrement;

	public void Start()
	{
		startOffset = this.transform.localPosition;
		angleIncrement = 60f / turnSpeed;
		moveIncrement = 60f / moveIncrement;
	}


	public void MoveToTargetRotation(Quaternion targetRotation)
	{
		print("Start rotation coroutine");
		this.StartCoroutine(MoveToTargetRotation_Coro(targetRotation));
	}

	private IEnumerator MoveToTargetRotation_Coro(Quaternion targetRotation)
	{ 
		float angleToTarget = Quaternion.Angle(this.transform.rotation, targetRotation);
		while (angleToTarget > angleIncrement * 2)
		{
			this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, angleIncrement);

			angleToTarget = Quaternion.Angle(this.transform.rotation, targetRotation);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
	}

	public void MoveToTargetPosition(Vector3 targetPoint)
	{
		print("Start move coroutine");
		this.StartCoroutine(MoveToTargetPosition_Coro(targetPoint));
	}

	public IEnumerator MoveToTargetPosition_Coro(Vector3 targetPoint)
	{
		float distanceToTarget = Vector3.Distance(this.transform.position, targetPoint);
		while (distanceToTarget > moveIncrement * 2)
		{
			this.transform.position += Vector3.MoveTowards(this.transform.position, targetPoint, moveIncrement);

			distanceToTarget = Vector3.Distance(this.transform.position, targetPoint);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
	}
}
