using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
