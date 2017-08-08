using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Microsoft.CSharp;
using System.Reflection;
using System.CodeDom.Compiler;
using UnityEditor;
using System.Collections;
using MyNamespace;
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
		private const string CONTINUUM_VERSION = "a1.3";


		private ContinuumCompiler continuumCompiler = new ContinuumCompiler();
		private ContinuumSense continuumSense = new ContinuumSense();

		// script text
		private string scriptText = string.Empty;
		// cache of last method we compiled so repeat executions only incur a single compilation
		private MethodInfo lastScriptMethod;

		// position of scroll view
		private Vector2 scrollPos;
		private ContinuumAutocompletePopup autocompleteWindow;

		private bool autocompleteWindowWasDisplayed = false;

		private static Continuum_ImmediateWindow continuumWindow;

		#region Async Temp Variables
		private string userGuess = null;
		//TODO: Do I neeed this?
		//private IEnumerable<string> autocompleteSeedForNextOnGui;
		#endregion

		[MenuItem("Continuum/Continuum_Immediate_"+ CONTINUUM_VERSION)]
		static void Init()
		{
			// get the window, show it, and hand it focus
			continuumWindow = EditorWindow.GetWindow<Continuum_ImmediateWindow>("Continuum_Immediate_"+ CONTINUUM_VERSION, false);

			continuumWindow.continuumSense.Init(typeof(GameObject));

			//TODO: If I take this out, I think i have serialization throughout closing and reopening window.
			continuumWindow.scriptText = "";

			continuumWindow.Show();
			continuumWindow.Focus();
			Debug.Log("Continuum Window Initialized");
		}

		private void ProcessCodeViewCommands()
		{
			if (Event.current.type == EventType.ValidateCommand)
			{
				if (Event.current.commandName == "SelectAll")
				{
					Debug.Log("select all");
					Event.current.Use();
				}
				else if (Event.current.commandName == "Copy" || Event.current.commandName == "Cut")
				{
					Debug.Log("Copy or Cut");
					Event.current.Use();
				}
				else if (Event.current.commandName == "Paste")
				{
					Debug.Log("Paste");

					if (!string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))
					{
						Event.current.Use();
					}
				}
				else if (Event.current.commandName == "Delete")
				{
					Debug.Log("Delete");
					Event.current.Use();
				}
			}
		}



		void OnGUI()
		{
			TextEditor editor = (TextEditor)EditorGUIUtility.GetStateObject(typeof(TextEditor), EditorGUIUtility.keyboardControl);

			

			

			KeyEventHandling();
			
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			//This is the reason why we can't copy paste and select all. 
			//EditorGUILayout is said to be exactly the same, plus those features, but then (TextEditor)EditorGUIUtility.GetStateObject(typeof(TextEditor), EditorGUIUtility.keyboardControl)
			//doesn't return the correct object, but an unitialized blank one.
			string newScriptText = GUILayout.TextArea(scriptText, GUILayout.Height(70));
			if (newScriptText != scriptText)
			{
				// if the script changed, update our cached version and null out our cached method
				var tmp = scriptText;
				scriptText = newScriptText;
				OnTextChanged(tmp, scriptText);
			}
			
			// show the execute button
			if (GUILayout.Button("Execute"))
			{
				CompileAndRun();
			}

			if (GUILayout.Button("AutoComplete"))
			{
				OpenAutocompleteAsync();
			}


			var t = EditorGUIUtility.GetStateObject(typeof(TextEditor), EditorGUIUtility.keyboardControl).GetType();

			if (deleteKeyNextFrame > 0) { deleteKeyNextFrame--; }
			if (deleteKeyNextFrame == 0)
			{
				editor.Backspace();
				deleteKeyNextFrame--;
			}

			while (moveForward > 0)
			{
				editor.MoveRight();
				moveForward--;
			}


			// close the scroll view
			EditorGUILayout.EndScrollView();

			//OpenAutocompleteWindowIfPointPressed(editor);

			if (openAutocomplete)
			{
				//IEnumerable<string> seed = autocompleteSeedForNextOnGui;

				//if (seed == null)
				//{
				//	if (continuumSense.initialized == false)
				//	{
				//		Debug.LogError("Continuum was not initialized. Reinitializing");
				//		continuumSense.Init(typeof(GameObject));
				//	}

				//	seed = continuumSense.Guess("");
				//}

				Action openAutoCompletePopup = () =>
				{
					autocompleteWindow = ScriptableObject.CreateInstance<ContinuumAutocompletePopup>();
					var cursorPos = editor.graphicalCursorPos;

					autocompleteWindow.position = new Rect(position.position + cursorPos + new Vector2(5, 18), new Vector2(350, 200));
					
					autocompleteWindow.Continuum_Init();

					autocompleteWindow.ChangeEntries(continuumSense.GuessMemberInfo(GetGuess(scriptText)));

					autocompleteWindow.onEntryChosen += (str) => OnAutocompleteEntryChosen(editor, str);

					autocompleteWindow.ShowPopup();
				};

				openAutoCompletePopup();

				openAutocomplete = false;
			}
			Repaint();
			if(autocompleteWindow != null)
			{
				autocompleteWindow.Repaint();
			}

			
		}

		private void OnTextChanged(string before, string after)
		{
			lastScriptMethod = null;
			autocompleteWindowWasDisplayed = false;

			//This happens for example when Ctrl+A + Del
			if (after.Contains('.') == false)
			{
				continuumSense.ScopeAllTheWayUp();
			}

			string guess = GetGuess(line: after);
			RefreshAutoCompleteWindowGuesses(guess);

			bool wasCharAdded = (after.Length > before.Length) == true;
			bool wasCharRemoved = (after.Length < before.Length) == true;

			char newChar = GetDifferentChar(before, after);
			
			if (wasCharAdded)
			{
				//Debug.Log("New Char is "+newChar);
				if (newChar == '.')
				{
					int newCharIndex = after.Length - guess.Length; //For now it'll do
					var previousMember = new string(after.Substring(0, newCharIndex-1)	//We want the char before the point
						.Reverse()
						.Skip(1)
						.TakeWhile(c => c != '.')
						.Reverse()
						.ToArray());

					//var previousMember = new string(after
					//	.Reverse()
					//	.Skip(1)
					//	.TakeWhile(c => c != '.')
					//	.Reverse()
					//	.ToArray());

					continuumSense.ScopeDown(previousMember);
					autocompleteWindow.ChangeEntries(continuumSense.GuessMemberInfo(""));
					OpenAutocompleteAsync();
				}

				if (char.IsLetter(newChar) == false && newChar != '_' && newChar != '.') 
				{
					CloseAutocompleteWindow();
				}
			}

			if (wasCharRemoved)
			{
				if (newChar == '.')
				{
					continuumSense.ScopeUp();
					//Debug.Log("Current scope is " + continuumSense.CurrentScope);
				}
			}





			try
			{
				//This block is to reacto to "new "
				TextEditor editor = (TextEditor)EditorGUIUtility.GetStateObject(typeof(TextEditor), EditorGUIUtility.keyboardControl);
				var lastFourChars = editor.text.Substring(editor.cursorIndex - 4, 4);
				if (lastFourChars == "new " && autocompleteWindowWasDisplayed == false)
				{
					//I do my shit here
					IEnumerable<string> allTypes = continuumSense.GetAllTypes();
					Debug.Assert(allTypes.Contains("Vector3"));
					allTypes = new string[] { "Vector3" }.Concat(allTypes);
					OpenAutocompleteAsync(allTypes);
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				//It's ok
			}
		}

		private void KeyEventHandling()
		{
			switch (Event.current.type)
			{
				case EventType.KeyDown:
					{
						if (Event.current.keyCode == (KeyCode.Escape))
						{
							
							CloseAutocompleteWindow();
						}
						if (Event.current.keyCode == (KeyCode.Escape))
						{
							if (autocompleteWindow == null) { return; }

							//autocompleteWindow.SimulateSelectFirstEntry();
							CloseAutocompleteWindow();

							Event.current.Use();
							//deleteKeyNextFrame = 1;
						}

						//if (Event.current.keyCode == (KeyCode.Return))
						//{
						//	if (autocompleteWindow == null) { return; }

						//	autocompleteWindow.SimulateSelectFirstEntry();
						//	CloseAutocompleteWindow();

						//	Event.current.Use();
						//	//deleteKeyNextFrame = 1;
						//}
						////TODO: I can't figure out how to ignore the space.
						//if (Event.current.keyCode == (KeyCode.Space) && Event.current.shift)
						//{
						//	OpenAutocompleteAsync();

						//	Event.current.Use();
						//	//deleteKeyNextFrame = 1;
						//}
						break;
					}

			}
		}

		private void OnAutocompleteEntryChosen(TextEditor editor, string chosenEntry)
		{
			scriptText = new string(scriptText
				.Reverse()
				.SkipWhile(c => c != '.')
				.Reverse()
				.ToArray())
				+ chosenEntry;

			//scriptText = scriptText.Insert(editor.cursorIndex, chosenEntry);
			moveForward = chosenEntry.Length;

			CloseAutocompleteWindow();

			//continuumSense.ScopeDown(chosenEntry);
		}

		//private void OpenAutocompleteWindowIfPointPressed(TextEditor editor)
		//{
		//	try
		//	{
		//		if (scriptText[editor.cursorIndex - 1] == '.' && autocompleteWindowWasDisplayed == false)
		//		{
		//			OpenAutocompleteAsync();
		//		}
		//	}
		//	catch (IndexOutOfRangeException)
		//	{
		//		//It's okay
		//	}
		//}
		private void CloseAutocompleteWindow()
		{
			if(autocompleteWindow == null) { return; }

			autocompleteWindow.Close();
			if(continuumWindow != null)
			{
				continuumWindow.Focus();
			}
		}
		private static char GetDifferentChar(string before, string after)
		{
			char newChar = (after.Length > before.Length) ? after.Last() : before.Last();
			for (int i = 0; i < ((after.Length < before.Length) ? after.Length : before.Length); i++)
			{
				if (before[i] != after[i])
				{
					newChar = after[i];
					break;
				}
			}

			return newChar;
		}

		private void RefreshAutoCompleteWindowGuesses(string guess)
		{
			var guesses = continuumSense.GuessMemberInfo(guess);
			if (autocompleteWindow != null)
			{
				autocompleteWindow.ChangeEntries(guesses);
			}
		}

		private string GetGuess(string line)
		{
			string guess = "";

			string reversedLine = new string(line.Reverse().ToArray());
			guess = new string(reversedLine.TakeWhile(c => c != '.').Reverse().ToArray());

			return guess;
		}

		//public Type GetMemberType(Type scope, string memberName)
		//{
		//	//TODO: returns the type from a string, and a scope.
		//}

		//private void CurrentLineChanged(string previous, string current)
		//{

		//	if (initialized == false) { throw new ContinuumNotInitializedException(); }

		//	//TODO: this
		//	//If point was pressed
		//	//set current line
		//	if (current.Contains('.') == false)
		//	{
		//		var allMembersAndMethods = Guess(type_scope_history.Peek(), "");
		//		//TODO: display suggestions
		//		DisplaySuggestionList(allMembersAndMethods);
		//		return;
		//	}

		//	string reversedLine = new string(current.Reverse().ToArray());
		//	string guess = new string(reversedLine.TakeWhile(c => c != '.').Reverse().ToArray());
		//	//string caller = new string(reversedLine.SkipWhile(c => c == '.').TakeWhile(c1 => c1 != '.').Reverse().ToArray());

		//	var suggestions = Guess(type_scope_history.Peek(), guess);
		//	DisplaySuggestionList(suggestions);
		//	//endIf
		//}
		public void OpenAutocompleteAsync(IEnumerable<string> seed = null)
		{
			if (autocompleteWindow != null)
			{
				//CloseAutocompleteWindow();
				return;
			}

			//if(seed != null)
			//{
			//	autocompleteSeedForNextOnGui = seed;
			//}

			openAutocomplete = true;
			autocompleteWindowWasDisplayed = true;
		}

		public const int MAGIC_NUMBER = 14;
		private static bool openAutocomplete;

		private int moveForward;
		private int deleteKeyNextFrame = -1; //-1 means do nothing

		private void CompileAndRun()
		{
			continuumCompiler.CompileAndRun(scriptText);
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

		private void OnFocus()
		{
			if(autocompleteWindow != null)
			{
				var tmp = autocompleteWindow.position;
				autocompleteWindow.position = tmp;
			}
		}

		private void OnDestroy()
		{
			CloseAutocompleteWindow();
		}

	}

















}