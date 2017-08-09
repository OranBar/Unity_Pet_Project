using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace TonRan.Continuum
{
	public class Debug {

		public static bool enabled = false;

		private UnityEngine.Debug debug;

		public static void Log(object o)
		{
			if (enabled)
			{
				Debug.Log(o); 
			}
		}

		public static void LogFormat(object o)
		{
			if (enabled)
			{
				Debug.LogFormat(o); 
			}
		}

		public static void LogError(object o)
		{
			if (enabled)
			{
				Debug.LogError(o); 
			}
		}

		public static void LogErrorFormat(object o)
		{
			if (enabled)
			{
				Debug.LogErrorFormat(o); 
			}
		}

		public static void LogWarning(object o)
		{
			if (enabled)
			{
				Debug.LogWarning(o); 
			}
		}

		public static void LogWarningFormat(object o)
		{
			if (enabled)
			{
				Debug.LogErrorFormat(o); 
			}
		}

		public static void Assert(object o)
		{
			if (enabled)
			{
				Debug.Assert(o);
			}
		}

		public static void Assert(object o, string msg)
		{
			if (enabled)
			{
				Debug.Assert(o, msg);
			}
		}


	}

	[CreateAssetMenu(fileName = "CurrStyle", menuName = "Continuum/CreateStyle", order = 1)]
	public class DebugOptions : ScriptableObject
	{
		public bool enabled;

		public bool enable;


	}

	public class DebugOptionsCreator
	{
		[MenuItem("Assets/Create/DebugOptions")]
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
#endif