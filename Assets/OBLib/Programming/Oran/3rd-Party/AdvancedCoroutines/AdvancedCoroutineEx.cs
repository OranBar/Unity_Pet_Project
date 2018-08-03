using AdvancedCoroutines;
using System.Collections;
using UnityEngine;

public static class AdvancedCoroutinesEx
{
	public static Routine StartAdvCoroutine(this MonoBehaviour mb, IEnumerator routine)
	{
		Routine result = CoroutineManager.StartCoroutine(routine, mb.gameObject);
		return result;
	}

	public static void Stop(this Routine r)
	{
		CoroutineManager.StopCoroutine(r);
	}

	public static Routine StartAdvCoroutine_Standalone(this object obj, IEnumerator routine)
	{
		Routine result = CoroutineManager.StartStandaloneCoroutine(routine);
		return result;
	}
}