﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OranUnityUtils;

/**<summary>This class doesn't really do anything, but Coroutines need to be ran from a MonoBehaviour. So when a GameObject needs to run a coroutine, I just add this Object to it.</summary>
 */ 
public class CoroutineHelper : MonoBehaviour {

	public StoppableCoroutine StartStoppableCoroutine(IEnumerator routine)
	{
		return routine.MakeStoppable();
	}
 
}
