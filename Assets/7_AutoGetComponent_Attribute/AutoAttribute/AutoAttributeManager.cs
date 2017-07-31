#define DEB

/* Author: Oran Bar
 * Summary: 
 * 
 * This class executes the code to automatically set the references of the variables with the Auto attribute.
 * The code is executed every time Scripts are reloaded, and before playmode begins, ensuring null references always throw an exception before play even begins.
 * 
 * The Define DEB on top of the script can be commented out to remove console logs for performance.
 * 
 * Copyrights to Oran Bar™
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

[ScriptOrder(-1000)]
public class AutoAttributeManager : MonoBehaviour
{
	private void Awake()
	{
		//Debug.Log("Awaking", this.gameObject);
		print("[Auto]: Start Scene Sweep");
		SweeepScene();
		print("[Auto]: All Variables Referenced!");
	}

	private static void AutoReference(MonoBehaviour targetMb)
	{
		//Fields
		IEnumerable<FieldInfo> fields = GetFieldsWithAuto(targetMb);

		foreach (var field in fields)
		{
			foreach (AutoAttribute autofind in field.GetCustomAttributes(typeof(AutoAttribute), true))
			{
				var currentReferenceValue = field.GetValue(targetMb);
				if (currentReferenceValue == null || currentReferenceValue.Equals(null))
				{
					autofind.Execute(targetMb, field);
				}
			}
		}

		//Properties
		IEnumerable<PropertyInfo> properties = GetPropertiesWithAuto(targetMb);

		foreach (var prop in properties)
		{
			foreach (AutoAttribute autofind in prop.GetCustomAttributes(typeof(AutoAttribute), true))
			{
				var currentReferenceValue = prop.GetValue(targetMb, null);
				if (currentReferenceValue == null || currentReferenceValue.Equals(null))
				{
					autofind.Execute(targetMb, prop);
				}
			}
		}
	}

	private static void SweeepScene()
	{
#if DEB
		//Debug
		Stopwatch sw = new Stopwatch();

		sw.Start();
		//////////////////
#endif

		var activeScene = SceneManager.GetActiveScene();

		IEnumerable<MonoBehaviour> monoBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
			.Where(mb => mb.gameObject.scene == activeScene);


		foreach (var mb in monoBehaviours)
		{
			AutoReference(mb);
		}

#if DEB
		//Debug
		sw.Stop();

		int variablesAnalized = monoBehaviours
			.Select(mb => mb.GetType())
			.Aggregate(0, (agg, mbType) => 
				agg = agg + mbType.GetFields().Count() + mbType.GetProperties().Count()
			);

		int variablesWithAuto = monoBehaviours
			.Aggregate(0, (agg, mb) => 
				agg = agg + GetFieldsWithAuto(mb).Count() + GetPropertiesWithAuto(mb).Count()
			);

		//Debug.Log("Elapsed "+sw.ElapsedMilliseconds+" milliseconds.");
		Debug.LogFormat("[Auto] Scan Time - {3} Milliseconds. \nAnalized {0} MonoBehaviours and {1} variables. {2}/{1} variables had [Auto]", monoBehaviours.Count(), variablesAnalized, variablesWithAuto, sw.ElapsedMilliseconds);
		/////////////////////
#endif
	}

	private static IEnumerable<FieldInfo> GetFieldsWithAuto(MonoBehaviour mb)
	{
		return mb.GetType()
			.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
			.Where(prop => prop.FieldType.IsClass && Attribute.IsDefined(prop, typeof(AutoAttribute)));
	}

	private static IEnumerable<PropertyInfo> GetPropertiesWithAuto(MonoBehaviour mb)
	{
		return mb.GetType()
			.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
			.Where(prop => prop.PropertyType.IsClass && Attribute.IsDefined(prop, typeof(AutoAttribute)));
	}
}