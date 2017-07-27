
using System.Linq;
using UnityEngine;
using Microsoft.CSharp;
using System.Reflection;
using System.CodeDom.Compiler;
using UnityEditor;
/*
* ImmediateWindow.cs
* Copyright (c) 2012 Nick Gravelyn
*
* Permission is hereby granted, free of charge, to any person obtaining a copy of 
* this software and associated documentation files (the "Software"), to deal in 
* the Software without restriction, including without limitation the rights to use, 
* copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
* Software, and to permit persons to whom the Software is furnished to do so, 
* subject to the following conditions:

* The above copyright notice and this permission notice shall be included in all 
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
* OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

// Modified by Oran Bar™

public class Continuum_ImmediateWindow : EditorWindow
{
	// script text
	private string scriptText = string.Empty;

	// cache of last method we compiled so repeat executions only incur a single compilation
	private MethodInfo lastScriptMethod;

	// position of scroll view
	private Vector2 scrollPos;

	void OnGUI()
	{
		// start the scroll view
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		// show the script field
		string newScriptText = EditorGUILayout.TextArea(scriptText);
		if (newScriptText != scriptText)
		{
			// if the script changed, update our cached version and null out our cached method
			scriptText = newScriptText;
			lastScriptMethod = null;
		}

		// store if the GUI is enabled so we can restore it later
		bool guiEnabled = GUI.enabled;

		// disable the GUI if the script text is empty
		GUI.enabled = guiEnabled && !string.IsNullOrEmpty(scriptText);

		// show the execute button
		if (GUILayout.Button("Execute"))
		{
			CompileAndRun();
		}

		// restore the GUI
		GUI.enabled = guiEnabled;

		// close the scroll view
		EditorGUILayout.EndScrollView();
	}

	private void CompileAndRun()
	{
		// if our script method needs compiling
		if (lastScriptMethod == null)
		{
			// create and configure the code provider
			var codeProvider = new CSharpCodeProvider();
			var options = new CompilerParameters();
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
			options.ReferencedAssemblies.Add(typeof(ContinuumTesterA).Assembly.Location);
			Debug.Log("Mehere");

			// compile an assembly from our source code
			var result = codeProvider.CompileAssemblyFromSource(options, string.Format(scriptFormat, scriptText));

			// log any errors we got
			if (result.Errors.Count > 0)
			{
				foreach (CompilerError error in result.Errors)
				{
					// the magic -13 on the line is to compensate for usings and class wrapper around the user script code.
					// subtracting 13 from it will give the user the line numbers in their code.
					if (error.IsWarning)
					{
						Debug.LogWarning(string.Format("Immediate Compiler Warning ({0}): {1}", error.Line - 13, error.ErrorText));
					}
					else
					{
						Debug.LogError(string.Format("Immediate Compiler Error ({0}): {1}", error.Line - 13, error.ErrorText));
					}
				}
			}

			// If NO errors : use reflection to pull out our action method so we can invoke it
			if (result.Errors.HasErrors == false)
			{

				var type = result.CompiledAssembly.GetType("ImmediateWindowCodeWrapper");
				lastScriptMethod = type.GetMethod("PerformAction", BindingFlags.Public | BindingFlags.Static);

				Debug.Log("Immediate Window: Script successfully Executed");
			}

		}

		// if we have a compiled method, invoke it
		if (lastScriptMethod != null)
			lastScriptMethod.Invoke(null, null);
	}

	[MenuItem("Continuum/Continuum_Immediate_a1.0")]
	static void Init()
	{
		// get the window, show it, and hand it focus
		var window = EditorWindow.GetWindow<Continuum_ImmediateWindow>("Continuum_Immediate_a1.0", false);
		window.Show();
		window.Focus();
	}

	// script we wrap around user entered code
	static readonly string scriptFormat = @"
using UnityEngine; 
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public static class ImmediateWindowCodeWrapper
{{
    public static void PerformAction()
    {{
        // user code goes here
        {0};
    }}
}}";











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
















