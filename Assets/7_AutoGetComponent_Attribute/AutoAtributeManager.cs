#define DEB
//#undefine DEB

/* Author: Oran Bar
 * Summary: 
 * The Define DEB on top of the script can be commented out to remove console logs for performance.
 * This class executes the code needed to set the references of the variables with the Auto attribute.
 * The code is executed every time Scripts are reloaded, and before playmode begins.
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
using UnityEditor;

[InitializeOnLoad]
public class AutoAtributeManager : MonoBehaviour
{

	static AutoAtributeManager()
	{
		EditorApplication.playmodeStateChanged += ModeChanged;
	}
	
	static void ModeChanged()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode &&
			 EditorApplication.isPlaying == false)
		{
			OnScriptsReloaded();
		}
	}

	//static string[] OnWillSaveAssets(string[] paths)
	//{
	//	Debug.Log("OnWillSaveAssets");
	//	foreach (string path in paths)
	//		Debug.Log(path);
	//	return paths;
	//}

	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnScriptsReloaded()
	{
#if DEB
		//Debug
		Stopwatch sw = new Stopwatch();

		sw.Start();
		//////////////////
#endif
		IEnumerable<MonoBehaviour> monoBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
			.Where(mb => mb.gameObject.scene == SceneManager.GetActiveScene());

		foreach (var mb in monoBehaviours)
		{
			IEnumerable<FieldInfo> fields = GetFieldsWithAuto(mb);

			foreach (var field in fields)
			{
				foreach (AutoAttribute autofind in field.GetCustomAttributes(typeof(AutoAttribute), true))
				{
					if (field.GetValue(mb).Equals(null)) 
					{
						autofind.Execute(mb, field);
					}
				}
			}

			IEnumerable<PropertyInfo> properties = GetPropertiesWithAuto(mb);

			foreach (var prop in properties)
			{
				foreach (AutoAttribute autofind in prop.GetCustomAttributes(typeof(AutoAttribute), true))
				{
					if (prop.GetValue(mb, null).Equals(null))
					{
						autofind.Execute(mb, prop);
					}
				}
			}
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

		Debug.Log("Elapsed "+sw.ElapsedMilliseconds+" milliseconds.");
		Debug.LogFormat("Analized {0} MonoBehaviours and {1} variables. {2}/{1} had [Auto]", monoBehaviours.Count(), variablesAnalized, variablesWithAuto);
		/////////////////////
#endif
	}

	private static IEnumerable<FieldInfo> GetFieldsWithAuto(MonoBehaviour mb)
	{
		return mb.GetType()
			.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
			.Where(prop => Attribute.IsDefined(prop, typeof(AutoAttribute)));
	}

	private static IEnumerable<PropertyInfo> GetPropertiesWithAuto(MonoBehaviour mb)
	{
		return mb.GetType()
			.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
			.Where(prop => Attribute.IsDefined(prop, typeof(AutoAttribute)));
	}
}