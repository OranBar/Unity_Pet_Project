using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class ScriptOrderManagerEditor : UnityEditor.AssetModificationProcessor
{
	static ScriptOrderManagerEditor()
	{
		foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
		{
			if (monoScript.GetClass() != null)
			{
				foreach (var a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptOrder)))
				{
					var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
					var newOrder = ((ScriptOrder)a).order;
					if (currentOrder != newOrder)
						MonoImporter.SetExecutionOrder(monoScript, newOrder);
				}
			}
		}
	}


	//TODO: Oran. Move this to a new class called AutoAttributeManagerEditor
	private static void MakeSureAutoManagerIsInScene()
	{
		//Debug.Log("Mehere");
		var autoManager = GameObject.FindObjectOfType<AutoAttributeManager>();
		if (autoManager == null)
		{
			GameObject autoGo = new GameObject("Auto_Attribute_Manager");
			autoGo.AddComponent<AutoAttributeManager>();
		}
	}


	public static string[] OnWillSaveAssets(string[] paths)
	{
		MakeSureAutoManagerIsInScene();
		return paths;
	}

	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnScriptsReload()
	{
		MakeSureAutoManagerIsInScene();
	}
}
