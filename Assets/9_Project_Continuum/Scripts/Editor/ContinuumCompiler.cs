using System;
using System.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = TonRan.Continuum.Continuum_ImmediateDebug;


public class ContinuumCompiler {

#region COMPILER_VARIABLES
	// script we wrap around user entered code
	private string scriptFormat_Base = @" 
			using UnityEngine; //0
			using UnityEditor; //1
			using System.Collections; //2
			using System.Collections.Generic; //3
			using System.Text; //4
			using System.Linq; //5
			using MyNamespace; //6
			//7
			public static class ImmediateWindowCodeWrapper //8
			{{ //9
				public static void PerformAction() //10
				{{ //11
					// user code goes here //12
					{0}; //13 <<== THIS NUMBER + 1 = MAGIC NUMBER
				}}
			}}";

	private string scriptFormat_Selection = @" 
			using UnityEngine; //0
			using UnityEditor; //1
			using System.Collections; //2
			using System.Collections.Generic; //3
			using System.Text; //4
			using System.Linq; //5
			using MyNamespace; //6
			//7
			public static class ImmediateWindowCodeWrapper //8
			{{ //9
				public static void PerformAction() //10
				{{ //11
					// user code goes here //12
					Selection.activeTransform.gameObject.{0}; //13 <<== THIS NUMBER + 1 = MAGIC NUMBER
				}}
			}}";

	public const int MAGIC_NUMBER = 14;

	private bool showResults;
	private bool logErrors;
	private bool logWarnings;
	private bool logNormalMessages;
	#endregion

	// cache of last method we compiled so repeat executions only incur a single compilation
	private MethodInfo lastScriptMethod;

	public ContinuumCompiler(bool logErrors = true, bool logWarnings = true, bool logNormalMessages = true)
	{
		this.logErrors = logErrors;
		this.logWarnings = logWarnings;
		this.logNormalMessages = logNormalMessages;
	}


	public void CompileAndRun(string code)
	{
		// compile an assembly from our source code
		Debug.Log("Compiling");
		MethodInfo result = Compile(code);
		Debug.Log("Run");
		// If NO errors : run
		Run(result);
		
	}

	public MethodInfo Compile(string code, CompilerParameters parameters = null)
	{
		// create and configure the code provider
		CSharpCodeProvider codeProvider = new CSharpCodeProvider();
		CompilerParameters options = (parameters != null) ? parameters : new CompilerParameters();
		options.GenerateInMemory = true;
		options.GenerateExecutable = false;

		// bring in system libraries
		options.ReferencedAssemblies.Add("System.dll");
		options.ReferencedAssemblies.Add("System.Core.dll");

		// bring in Unity assemblies
		options.ReferencedAssemblies.Add(typeof(EditorWindow).Assembly.Location);
		options.ReferencedAssemblies.Add(typeof(Transform).Assembly.Location);
		options.ReferencedAssemblies.Add(typeof(UnityEngine.Object).Assembly.Location);
		//TODO: reference to something more secure... To import project code.
		options.ReferencedAssemblies.Add(typeof(ZDontTouch_Continuum).Assembly.Location);

		CompilerResults result = codeProvider.CompileAssemblyFromSource(options, string.Format(scriptFormat_Selection, code));
		//CompilerResults result = codeProvider.CompileAssemblyFromSource(options, string.Format(scriptFormat_Base, code));

		if (HasErrors(result))
		{
			if (logErrors)
			{
				LogErrors(result);
			}
			if (logWarnings)
			{
				LogWarning(result);
			}
			if (logNormalMessages)
			{
				//TODO
			}
		}

		//Method compiled. Return method to invoke the compiled code
		var type = result.CompiledAssembly.GetType("ImmediateWindowCodeWrapper");
		lastScriptMethod = type.GetMethod("PerformAction", BindingFlags.Public | BindingFlags.Static);

		return lastScriptMethod;
	}

	public object Run(MethodInfo methodToInvoke)
	{
		Debug.Assert(lastScriptMethod != null);
		object result = lastScriptMethod.Invoke(null, null);
		Debug.Log("Method run. Result was: "+result);
		return result;
	}

	//private string Run(MethodInfo methodToInvoke)
	//{
	//	Debug.Assert(lastScriptMethod != null);
	//	string result = lastScriptMethod.Invoke(null, null) as string;
	//	if(result == null)
	//	{
	//		throw new ContinuumInternalError("Error Code 1: Method didn't return string");
	//	}
	//	return result;
	//}

	private bool HasErrors(CompilerResults compiledCode)
	{
		return compiledCode.Errors.Count > 0;
	}
	
	private static void LogErrors(CompilerResults compiledCode)
	{
		foreach (CompilerError error in compiledCode.Errors)
		{
			if (error.IsWarning == false)
			{
				Debug.LogError(string.Format("Immediate Compiler Error ({0}): {1}", error.Line - MAGIC_NUMBER, error.ErrorText));
			}
		}
	}

	private static void LogWarning(CompilerResults compiledCode)
	{
		foreach (CompilerError error in compiledCode.Errors)
		{
			if (error.IsWarning)
			{
				Debug.LogWarning(string.Format("Immediate Compiler Warning ({0}): {1}", error.Line - MAGIC_NUMBER, error.ErrorText));
			}
		}
	}

	
}
