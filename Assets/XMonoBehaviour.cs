using System;
using UnityEngine;


public abstract class XMonoBehaviour : MonoBehaviour
{

	//bool initialized_awake = false;

	public void Awake()
	{
		

		//initialized_awake = true;
		XAvake();
	}

	public void Start()
	{

		XStart();
	}

	protected virtual void XAvake() { }

	protected virtual void XStart() { }



}