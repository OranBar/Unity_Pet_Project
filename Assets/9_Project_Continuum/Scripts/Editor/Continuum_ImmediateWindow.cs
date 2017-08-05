using System;
using System.Collections.Generic;
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
			//GUILayout.BeginScrollView(scrollPos);
			// show the script field
			string newScriptText = GUILayout.TextArea(scriptText);
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

			if (GUILayout.Button("AutoComplete"))
			{
				openAutocomplete = true; 
			}

			TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

			//GUI.Label(new Rect(216, 8, 200, 200), string.Format("Selected text: {0}\nPos: {1}\nSelect pos: {2}",
			//	editor.SelectedText,
			//	editor.position,
			//	editor.graphicalCursorPos));

			//if (GUILayout.Button(/*new Rect(8, 216, 400, 20),*/ "Insert Tab"))
			//	scriptText = scriptText.Insert(editor.cursorIndex, "\t");

			// restore the GUI
			GUI.enabled = guiEnabled;

			// close the scroll view
			EditorGUILayout.EndScrollView();

			if (openAutocomplete)
			{

				//var charSize = styles.normalStyle.CalcSize(new GUIContent("W"));
				var cursorPos = editor.graphicalCursorPos;

				ContinuumAutocompletePopup window = ScriptableObject.CreateInstance<ContinuumAutocompletePopup>();
				window.position = new Rect(position.position + cursorPos + new Vector2(5,18), new Vector2(450, 150));

				IEnumerable<string> seed = Enumerable.Range(97, 3).Select(i => (char)i + "Boo");
				seed.ToList().ForEach(s => Debug.Log(s));
				window.Continuum_Init(seed);

				window.ShowPopup();

				//GUILayout.BeginArea(new Rect(new Vector2(100, 100), new Vector2(50, 25));
				//if (GUILayout.Button("MyNewButton"))
				//{
				//	Debug.Log("Mehere");
				//}
				//GUILayout.EndArea();
				openAutocomplete = false;
			}
		}

		public const int MAGIC_NUMBER = 14;
		private bool openAutocomplete;
		
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

		
		
		public class ContinuumAutocompletePopup : EditorWindow
		{
			public event Action<string> onEntryChosen;

			private List<string> entries = new List<string>();

			static void Init()
			{
				ContinuumAutocompletePopup window = ScriptableObject.CreateInstance<ContinuumAutocompletePopup>();
				//window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
				window.ShowPopup();
			}

			public void Continuum_Init(IEnumerable<string> entries)
			{
				onEntryChosen = (s) => { };
				this.entries = entries.ToList();
			}

			private Vector2 scrollPos;

			void OnGUI()
			{
				EditorGUILayout.LabelField("Welcome to the Continuum Window Autocomplete. We'll give it a cooler name at some point", EditorStyles.wordWrappedLabel);
				//GUILayout.Space(70);

				scrollPos = GUILayout.BeginScrollView(scrollPos);

				Debug.Log("3> " + entries.Count);
				foreach (string entry in entries)
				{
					if (GUILayout.Button(entry))
					{
						Debug.Log("2> "+entry);
						onEntryChosen(entry);
					}
				}

				//if (GUILayout.Button(entries[0]))
				//{
				//	onEntryChosen(entries[0]);
				//}


				if (GUILayout.Button("Agree!")) this.Close();

				GUILayout.EndScrollView();
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

















}