
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
namespace TonRan.Continuum
{
	public class Continuum_ImmediateWindow : EditorWindow
	{
		private ContinuumCompiler continuumCompiler = new ContinuumCompiler();

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

		public const int MAGIC_NUMBER = 14;

		private void CompileAndRun()
		{
			continuumCompiler.CompileAndRun(scriptText);
		}

		[MenuItem("Continuum/Continuum_Immediate_a1.0")]
		static void Init()
		{
			// get the window, show it, and hand it focus
			var window = EditorWindow.GetWindow<Continuum_ImmediateWindow>("Continuum_Immediate_a1.0", false);
			window.Show();
			window.Focus();
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

















}