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


public class CmCompiler {

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

	private string scriptFormat_Selection_This = @" 
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
					GameObject @this = Selection.activeTransform.gameObject; //12
					{0}; //13 <<== THIS NUMBER + 1 = MAGIC NUMBER
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

	public CmCompiler(bool logErrors = true, bool logWarnings = true, bool logNormalMessages = true)
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
		code = code.Replace("this", "@this");

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

		//CompilerResults result = codeProvider.CompileAssemblyFromSource(options, string.Format(scriptFormat_Base, code));
		//CompilerResults result = codeProvider.CompileAssemblyFromSource(options, string.Format(scriptFormat_Selection, code));
		CompilerResults result = codeProvider.CompileAssemblyFromSource(options, string.Format(scriptFormat_Selection_This, code));

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
	


	public static string ConvertPrivateInvocations_ToReflectionInvokes(string source)
	{
		string result = "";

		source = source.Replace("..", "!");
		//source += ";";
		var splitSource = source.Split('!');

		//Early out
		if (splitSource.Length == 0)
		{ return source; }

		for (int i = 1; i < splitSource.Length; i++)
		{
			string beforeOperator = splitSource[i - 1];
			string afterOperator = splitSource[i];

			Debug.Log("beforeOperator " + beforeOperator);


			string caller = new string(
				beforeOperator
				.Reverse()
				.TakeWhile(c => c != '=' && c != '(' && c != ',' && c != '.' && c != '!')
				.Reverse()
				.ToArray()
			);

			int callerStartIndex = beforeOperator.IndexOf(caller);


			string invocation = new string(
				afterOperator
				.TakeWhile(c1 => c1 != '=' && c1 != '(' && c1 != ',' && c1 != '.' && c1 != '!')
				.ToArray()
			);

			int invocationEndIndex = afterOperator.IndexOf(invocation) + invocation.Length;

			char charAfterInvocation = afterOperator
				.Skip(1)
				.FirstOrDefault(c1 => c1 == ';' || c1 == '=' || c1 == '(' || c1 == ',' || c1 == '.' || c1 == '!');

			Debug.Log("caller <color=red>" + caller + "</color>\n"
				+ "invocation <color=red>" + invocation + "</color>\n"
				+ "charAfterInvocation is " + charAfterInvocation);

			string replacement = "";
			string before, after;

			switch (charAfterInvocation)
			{
				//This is a Set
				case '=':

					string value = source.Substring(source.IndexOf(afterOperator) + afterOperator.IndexOf(charAfterInvocation) + 1).Trim();
					value = value.Remove(value.Length - 1); //Let's take out the ";"
					replacement = string.Format("{0}.GetType().SetField(\"{1}\", {2});", caller, invocation, value);
					Debug.Log(replacement);

					before = new string(beforeOperator.Take(callerStartIndex).ToArray());
					result += before + replacement;

					break;

				case '(':

					//GetType().GetMethod("Foo", BindingFlags.NonPublic, BindingFlags.Instance, BindingFlags.Static).ReturnParameter.ParameterType;
					//foo..Do(myInt, myString);

					string parameters = new string(afterOperator.SkipWhile(c => c != '(').Skip(1).TakeWhile(c => c != ')').ToArray());

					Debug.Log("Parameters are " + parameters);

					replacement = string.Format("( {0}.GetType().GetMethod(\"{1}\").Invoke({2}) );",
						caller,
						invocation,
						parameters);

					//Cast to correct type
					replacement = string.Format("({0}.GetType().GetMethod(\"{1}\").ReturnParameter.ParameterType) " + replacement,
						caller,
						invocation);

					Debug.Log(replacement);

					before = new string(beforeOperator.Take(callerStartIndex).ToArray());
					//after = new string(afterOperator.TakeWhile(c => c != ')').ToArray());
					result += before + replacement;
					break;

				//This is a Get
				default:
					replacement = string.Format("{0}.GetType().GetField(\"{1}\")", caller, invocation.Replace(";", ""));
					Debug.Log(replacement);

					before = new string(beforeOperator.Take(callerStartIndex).ToArray());
					after = new string(afterOperator.Skip(invocationEndIndex).ToArray());
					result += before + replacement + after + ";";

					break;
			}
			Debug.Log("result is " + result);
		}

		return result;
	}



}
