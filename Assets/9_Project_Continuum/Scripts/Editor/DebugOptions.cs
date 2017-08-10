using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TonRan.Continuum
{
	public class DebugOptions : ScriptableObject
	{
		public bool enabled;

		void OnValidate()
		{

			//EditorWindow.GetWindow<Continuum_ImmediateWindow>("Continuum_Immediate_" + TonRanVersion.CONTINUUM_VERSION, false).enableLogging = enabled;

			//continuumWindow.EnableDebug(enabled);

			if (Continuum_ImmediateDebug.enabled != enabled)
			{
				Continuum_ImmediateDebug.enabled = enabled;
			}
			//int x = 5;
			//Debug.Log("Validate");
		}

	}

	public class DebugOptionsCreator
	{
		[MenuItem("Assets/Create/Continuum/DebugOptions")]
		public static void CreateDebugOptions()
		{
			DebugOptions newStyle = ScriptableObject.CreateInstance<DebugOptions>();

			AssetDatabase.CreateAsset(newStyle, "Assets/9_Project_Continuum/Config/DebugOptions.asset");
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = newStyle;
		}
	}
}
