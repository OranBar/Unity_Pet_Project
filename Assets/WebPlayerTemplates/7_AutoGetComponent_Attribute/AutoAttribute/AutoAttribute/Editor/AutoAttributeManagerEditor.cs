/*======= Copyright (c) Immerxive Srl, All rights reserved. ===================

Author: Oran Bar

Purpose: 

Notes:

=============================================================================*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using IMX.ExtensionMethods;


[InitializeOnLoad]
public class AutoAttributeManagerEditor : UnityEditor.AssetModificationProcessor
{
	private static void MakeSureAutoManagerIsInScene()
	{
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
