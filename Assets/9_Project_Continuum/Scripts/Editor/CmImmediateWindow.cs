using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Debug = TonRan.Continuum.Continuum_ImmediateDebug;
using TonRan.Continuum;
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

namespace TonRan.Continuum {

	#region Helper Classes
	public class TonRanVersion
	{
		private const string _CONTINUUM_VERSION = "a1.3";

		public static string CONTINUUM_VERSION {
			get {
				return _CONTINUUM_VERSION;
			}
		}
	}

	public class Continuum_ImmediateDebug
	{
		private UnityEngine.Debug debug;

		public static bool enabled = false;

		public static void Log(object o)
		{
			if (enabled)
			{
				UnityEngine.Debug.Log(o);
			}
		}

		public static void LogFormat(string format, params object[] args)
		{
			if (enabled)
			{
				UnityEngine.Debug.LogFormat(format, args);
			}
		}

		public static void LogError(object o)
		{
			if (enabled)
			{
				UnityEngine.Debug.LogError(o);
			}
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			if (enabled)
			{
				UnityEngine.Debug.LogErrorFormat(format, args);
			}
		}

		public static void LogWarning(object o)
		{
			if (enabled)
			{
				UnityEngine.Debug.LogWarning(o);
			}
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
			if (enabled)
			{
				UnityEngine.Debug.LogErrorFormat(format, args);
			}
		}

		public static void Assert(bool condition)
		{
			if (enabled)
			{
				UnityEngine.Debug.Assert(condition);
			}
		}

		public static void Assert(bool condition, object message)
		{
			if (enabled)
			{
				UnityEngine.Debug.Assert(condition, message);
			}
		}

		public static void AssertFormat(bool condition, string format, params object[] args)
		{
			if (enabled)
			{
				UnityEngine.Debug.AssertFormat(condition, format, args);
			}
		}
	}
	#endregion
}

// Modified by Oran Bar™
namespace TonRan.Continuum
{
	public class CmImmediateWindow : EditorWindow
	{
		private CmCompiler continuumCompiler = new CmCompiler();
		private CmSense continuumSense = new CmSense();

		// script text
		private string scriptText = string.Empty;
		// cache of last method we compiled so repeat executions only incur a single compilation
		//private MethodInfo lastScriptMethod;

		// position of scroll view
		private Vector2 scrollPos;
		private CmAutocompletePopup autocompleteWindow;

		private bool showAutocomplete;
		private bool autocompleteWindowWasDisplayed = false;

		private static CmImmediateWindow continuumWindow;

		public static bool autocompletionEnabled;

		private bool lastAutocompletionEnabled;
		

		[MenuItem("Continuum/Continuum_Immediate {AlphaVersion}")]
		static void Init()
		{
			// get the window, show it, and hand it focus
			continuumWindow = EditorWindow.GetWindow<CmImmediateWindow>("Continuum_Immediate_"+ TonRanVersion.CONTINUUM_VERSION, false);

			Debug.enabled = AssetDatabase.LoadAssetAtPath<DebugOptions>("Assets/9_Project_Continuum/Config/DebugOptions.asset").enabled;
			
			continuumWindow.continuumSense.Init(typeof(GameObject));
			
			//TODO: If I take this out, I think i have serialization throughout closing and reopening window.
			continuumWindow.scriptText = "";

			//This line brings back the value from the last session.
			//TODO: change this to use a scriptable object like debug.
			continuumWindow.lastAutocompletionEnabled = autocompletionEnabled;

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

		private int removeKeyFlag = -1;

		void OnGUI()
		{
			KeyEventHandling();

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			//This is the reason why we can't copy paste and select all. 
			//EditorGUILayout is said to be exactly the same, plus those features, but then (TextEditor)EditorGUIUtility.GetStateObject(typeof(TextEditor), EditorGUIUtility.keyboardControl)
			//doesn't return the correct object, but an unitialized blank one.
			
			string newScriptText = EditorGUILayout.TextArea(scriptText, GUILayout.Height(70));
			TextEditor editor = GetTextEditor();

			if (editor == null) { EditorGUILayout.EndScrollView(); return; }

			if(removeKeyFlag >= 0) { removeKeyFlag--; }
			if(removeKeyFlag == 0){
				editor.Backspace();
				removeKeyFlag--;
			}


			// if the script changed, update our cached version and null out our cached method
			if (newScriptText != scriptText)
			{
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

			//-----------------------------------------------------------------------------
			if (GUILayout.Button("Complete!"))
			{
				string guess = GetGuess(scriptText);
				string firstContinuumSensePrediction = continuumSense.Guess(guess).FirstOrDefault();

				if (firstContinuumSensePrediction != null)
				{
					Debug.Log("adding " + firstContinuumSensePrediction);
					RemoveLastUserGuessFromTextArea();
					InsertTextInScriptAtCursorPosition(firstContinuumSensePrediction);
				}
				else
				{
					Debug.LogWarning("I have no guesses");
				}
			}
			//-----------------------------------------------------------------------------

			BeginWindows();
			//Debug.Log(showAutocomplete);
			if (showAutocomplete)
			{
				//Debug.Log("OpenAutocomplete");

				var cursorPos = editor.graphicalCursorPos;



				windowRect = new Rect(cursorPos + new Vector2(5, 18), new Vector2(350, 200));
				//windowRect = new Rect(100, 100, 300, 200);
				windowRect = GUILayout.Window(137, windowRect, DoWindow, "ContinuumSense");

				string userGuess = GetGuess(scriptText);
				//List<CmEntry> continuumSenseGuesses = continuumSense.GuessCmEntry(userGuess);
				//ChangeEntries(continuumSenseGuesses);

				//openAutocomplete = false;



				//autocompleteWindow = ScriptableObject.CreateInstance<CmAutocompletePopup>();

				//autocompleteWindow.position = new Rect(position.position + cursorPos + new Vector2(5, 18), new Vector2(350, 200));

				//autocompleteWindow.Continuum_Init();


				//autocompleteWindow.ChangeEntries(continuumSenseGuesses);

				//autocompleteWindow.onEntryChosen += (str) => OnSuggestionChosen(str, true);

				//autocompleteWindow.ShowPopup();
				//Action openAutoCompletePopup = () =>
				//{
				//	autocompleteWindow = ScriptableObject.CreateInstance<CmAutocompletePopup>();
				//	var cursorPos = editor.graphicalCursorPos;

				//	autocompleteWindow.position = new Rect(position.position + cursorPos + new Vector2(5, 18), new Vector2(350, 200));

				//	autocompleteWindow.Continuum_Init();

				//	string userGuess = GetGuess(scriptText);
				//	List<MemberInfo> continuumSenseGuesses = continuumSense.GuessMemberInfo(userGuess);
				//	autocompleteWindow.ChangeEntries(continuumSenseGuesses);

				//	autocompleteWindow.onEntryChosen += (str) => OnSuggestionChosen(str, true);

				//	//autocompleteWindow.;
				//	//autocompleteWindow.ShowPopup();
				//};

				//openAutoCompletePopup();
			}
			EndWindows();
			EditorGUILayout.EndScrollView();



			autocompletionEnabled = GUILayout.Toggle(autocompletionEnabled, "Enable Autocomplete");
			if (autocompletionEnabled != lastAutocompletionEnabled)
			{
				lastAutocompletionEnabled = autocompletionEnabled;
				if (autocompletionEnabled)
				{
					//Debug.Log("1");
					OpenAutocompleteAsync();
				}
				else
				{
					CloseAutocompleteWindow();
					//Debug.Log("2");
				}
			}

			

			//Repaint();
			//if (autocompleteWindow != null)
			//{
			//	autocompleteWindow.Repaint();
			//}
		}

		#region AutocompleteWindow

		Rect windowRect = new Rect(100, 100, 300, 200);

		//public event Action<string> onEntryChosen;

		//I'm gonna make it a hashset for now to ignore duplicates. Those duplicates, though, are important: They are overloaded methods. We need to consider those.
		private HashSet<string> entries = new HashSet<string>();
		private HashSet<CmEntry> entriesMemberInfo = new HashSet<CmEntry>();

		private void DoWindow(int id)
		{
			
			scrollPos = GUILayout.BeginScrollView(scrollPos);

			foreach (CmEntry entry in entriesMemberInfo)
			{
				var style = new GUIStyle(GUI.skin.button);

				style.fontStyle = FontStyle.Bold;

				if (entry.memberType == MemberTypes.Property)
				{
					//style.normal.textColor = Color.blue;
					style.normal.textColor = new Color(10 / 255f, 110 / 255f, 150 / 255f);
					//Debug.Log("magenta");
				}
				else if (entry.memberType == MemberTypes.Field)
				{
					//style.normal.textColor = Color.white;
					style.normal.textColor = new Color(56 / 255f, 40 / 255f, 0f / 255f);
				}
				else if (entry.memberType == MemberTypes.Method)
				{
					//style.normal.textColor = Color.green;
					style.normal.textColor = new Color(17 / 255f, 153 / 255f, 0 / 255f);
				}

				//style.normal.textColor = fontColor;

				if (GUILayout.Button(entry.name, style))
				{
					//onEntryChosen(entry.Name);
					OnSuggestionChosen(entry.name, false);
				}
			}
			GUILayout.EndScrollView();
			Repaint();
		}

		public void ChangeEntries(IEnumerable<CmEntry> newEntries)
		{
			entriesMemberInfo = new HashSet<CmEntry>(newEntries);
			//Repaint();
		}

		internal void SimulateSelectFirstEntry()
		{
			//onEntryChosen(entries.First());
			CmEntry suggestion = entriesMemberInfo.FirstOrDefault();
			//string suggestion = entries.FirstOrDefault();

			if (suggestion == null) { Debug.LogWarning("Null Suggestion :D"); return; }

			OnSuggestionChosen(suggestion.name, false);
		}

		#endregion

		private TextEditor GetTextEditor()
		{
			TextEditor tEditor = typeof(EditorGUI)
				.GetField("activeEditor", BindingFlags.Static | BindingFlags.NonPublic)
				.GetValue(null) as TextEditor;

			return tEditor;
		}

		private void OnTextChanged(string before, string after)
		{
			autocompleteWindowWasDisplayed = false;

			//This happens for example when Ctrl+A + Del
			if (after.Contains('.') == false)
			{
				//We're gonna have to implement scoping up in a different manner.....
				//continuumSense.ScopeAllTheWayUp();
			}

			string guess = GetGuess(line: after);
			RefreshAutoCompleteWindowGuesses(guess);

			bool wasCharAdded = (after.Length > before.Length) == true;
			bool wasCharRemoved = (after.Length < before.Length) == true;

			char newChar = GetDifferentChar(before, after);
			
			if (wasCharAdded)
			{
				if(string.IsNullOrEmpty(before))
				{
					if (IsValidMemberSymbol(newChar))
					{
						OpenAutocompleteAsync();
					}
				}

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
					
					continuumSense.ScopeDown(previousMember);
					OpenAutocompleteAsync();
					
					////ChangeEntries(continuumSense.GuessCmEntry(""));

					//if(autocompleteWindow != null)
					//{
					//	autocompleteWindow.ChangeEntries(continuumSense.GuessMemberInfo(""));
					//}
				}

				if (IsValidMemberSymbol(newChar) == false && newChar != '.') 
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


			TextEditor editor = GetTextEditor();
			if(editor.cursorIndex >= 4) { 
				//This block is to react to "new "

				var lastFourChars = editor.text.Substring(editor.cursorIndex - 4, 4);
				if (lastFourChars == "new " && autocompleteWindowWasDisplayed == false)
				{
					//This will bring up all Classes available in namespace
					continuumSense.ScopeDown(CmSense.AllClasses);
					OpenAutocompleteAsync();
					
					////I do my shit here
					//IEnumerable<string> allTypes = continuumSense.GetAllTypes();
					//Debug.Assert(allTypes.Contains("Vector3"));
					//allTypes = new string[] { "Vector3" }.Concat(allTypes);
					//OpenAutocompleteAsync(allTypes);
				}
			}
		}

		private bool IsValidMemberSymbol(char c)
		{
			return char.IsLetter(c) || c == '_' || char.IsNumber(c);
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
							
							CloseAutocompleteWindow();

							Event.current.Use();
						}

						if (Event.current.keyCode == (KeyCode.Return))
						{
							//if (autocompleteWindow == null) { return; }

							//autocompleteWindow.SimulateSelectFirstEntry();
							//CloseAutocompleteWindow();

							Event.current.Use();

							TextEditor txtEditor = GetTextEditor();
							string userGuess = GetGuess();
							string suggestion = continuumSense.Guess(userGuess).FirstOrDefault();
							if(suggestion == null)
							{
								Debug.LogWarning("Ehm... No guesses");
							}
							else
							{
								Event.current.Use();

								//Debug.Log("Text was /n"+txtEditor.text);
								//GetTextEditor().Backspace();    //Remove the return
								//Debug.Log("Return removed. Text is /n" + txtEditor.text);

								Debug.Log("Suggestion chosen: " + suggestion);

								OnSuggestionChosen(suggestion);
								RemoveLastKeyAsync();
							}

						}

						////TODO: I can't figure out how to ignore the space.
						if (Event.current.keyCode == (KeyCode.Space) && Event.current.shift)
						{
							Debug.Log("Shit+Space");
							OpenAutocompleteAsync();
							RemoveLastKeyAsync();

							Event.current.Use();
						}
						break;
					}

			}
		}

		/// <summary>
		/// waits for 3 OnGui with a custom hooked in system, then deletes (Deletion code is in OnGui)
		/// </summary>
		private void RemoveLastKeyAsync()
		{
			//This usually happens.
			if (removeKeyFlag < 0) { removeKeyFlag = 3; }
			else { removeKeyFlag += 3; }	//This means we need to do it twice... I don't know if it will ever happen, but I'll cover my ass right here and now.
		}


		private void OnSuggestionChosen(string chosenEntry, bool addPointAtEnd=false)
		{
			TextEditor editor = GetTextEditor();

			RemoveLastUserGuessFromTextArea();

			InsertTextInScriptAtCursorPosition(chosenEntry);

			try
			{
				continuumSense.ScopeDown(chosenEntry);
				ChangeEntries(continuumSense.GuessCmEntry(""));
				//if(autocompleteWindow != null)
				//{
				//	autocompleteWindow.ChangeEntries(continuumSense.GuessMemberInfo(""));
				//}
			}
			catch (KeyNotFoundException)
			{
				continuumSense.ScopeUp();
				CloseAutocompleteWindow();
				return;
			}

			if (addPointAtEnd)
			{
				InsertTextInScriptAtCursorPosition("."); 
			}
			
		}

		private void RemoveLastUserGuessFromTextArea()
		{
			

			TextEditor editor = GetTextEditor();

			string currentText = editor.text;
			int currentIndex = editor.cursorIndex;

			while (currentText.Length != 0 && currentText.Last() != '.')
			{
				editor.Backspace();
				currentText = currentText.Remove(currentText.Length - 1, 1);
			}

		}

		private void InsertTextInScriptAtCursorPosition(string textToAppend)
		{
			TextEditor editor = GetTextEditor();
			for (int i = 0; i < textToAppend.Length; i++)
			{
				editor.Insert(textToAppend[i]);
			}
		}
		
		private void CloseAutocompleteWindow()
		{
			showAutocomplete = false;

			//if (autocompleteWindow == null) { return; }

			//autocompleteWindow.Close();
			//if(continuumWindow != null)
			//{
			//	continuumWindow.Focus();
			//}
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
			var guesses = continuumSense.GuessCmEntry(guess);
			ChangeEntries(guesses);
			
			//if (autocompleteWindow != null)
			//{
			//	//autocompleteWindow.ChangeEntries(guesses);
			//}
		}

		private string GetGuess(string line)
		{
			string guess = "";

			string reversedLine = new string(line.Reverse().ToArray());
			guess = new string(reversedLine.TakeWhile(c => c != '.' && c !=' ').Reverse().ToArray());

			return guess;
		}

		private string GetGuess()
		{
			TextEditor txtEditor = GetTextEditor();
			return GetGuess(txtEditor.text.Substring(0, txtEditor.cursorIndex));
		}

		public void OpenAutocompleteAsync()
		{
			//if (autocompleteWindow != null || autocompletionEnabled == false)
			//{
			//	return;
			//}

			if(autocompletionEnabled == false) { return; }

			//string userGuess = GetGuess(scriptText);
			//List<CmEntry> continuumSenseGuesses = continuumSense.GuessCmEntry(userGuess);
			//ChangeEntries(continuumSenseGuesses);

			showAutocomplete = true;
			autocompleteWindowWasDisplayed = true;
		}
		
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