﻿using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Tester_AutoMulti : MonoBehaviour {

	[AutoChildren] private Light myLight = null;
	[AutoChildren] private Collider[] myChildrenCollidersArr = null;
	[AutoChildren] public Collider[] MyChildrenCollidersArr { get; set; } = null;
	[AutoParent] private Collider[] myParentCollidersArr = null;

	[AutoChildren] private List<Collider> myChildrenCollidersList = null;

	// Use this for initialization
	[Button]
	void RunTest () {
		AutoAttributeManager.AutoReference(this, out int succ, out int fail);

		Assert.IsNotNull(myChildrenCollidersArr);
		Assert.IsNotNull(MyChildrenCollidersArr); 
		Assert.AreEqual(1, myParentCollidersArr.Length); //Parent of this object has one collider
		Assert.IsNotNull(myChildrenCollidersList);
		Assert.IsNotNull(myLight);   //Child has light component
		Assert.AreEqual(2, myChildrenCollidersArr.Length);   //This gameobject has 2 children colliders
		Assert.AreEqual(2, MyChildrenCollidersArr.Length);   //This gameobject has 2 children colliders
		Assert.AreEqual(2, myChildrenCollidersList.Count);	
	}

}
