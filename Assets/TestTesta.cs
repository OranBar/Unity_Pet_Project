using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;

public class TestTesta : MonoBehaviour
{
	[ContextMenu("Foo")]
	public void Foo()
	{
		Debug.LogFormat("<color=blue>{0} test. Input is {1}</color>", 0, "myVar..myPrivateVar;");
		Continuum_ImmediateWindow.ConvertPrivateInvocations_ToReflectionInvokes("myVar..myPrivateVar;");

		Debug.LogFormat("<color=blue>{0} test. Input is {1}</color>", 1, "myVar..myPrivateMethod(myFloat, 5);");
		Continuum_ImmediateWindow.ConvertPrivateInvocations_ToReflectionInvokes("myVar..myPrivateMethod(myFloat, 5);");

		Debug.LogFormat("<color=blue>{0} test. Input is {1}</color>", 2, "myVar..myPrivateVar = 5;");
		Continuum_ImmediateWindow.ConvertPrivateInvocations_ToReflectionInvokes("myVar..myPrivateVar = 5;");

		Debug.LogFormat("<color=blue>{0} test. Input is {1}</color>", 3, "myA.myVar..myPrivateVars;");
		Continuum_ImmediateWindow.ConvertPrivateInvocations_ToReflectionInvokes("myA.myVar..myPrivateVar;");

		Debug.LogFormat("<color=blue>{0} test. Input is {1}</color>", 4, "b.Do(a..myVar, b..myVar, b..MyMethod().myVar;");
		Continuum_ImmediateWindow.ConvertPrivateInvocations_ToReflectionInvokes("b.Do(a..myVar, b..myVar, b..MyMethod().myVar;");

	}




}
