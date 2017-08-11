using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
using UnityEditor;

public class TestTesta : EditorWindow
{

	private string scriptText = string.Empty;
	// cache of last method we compiled so repeat executions only incur a single compilation
	//private MethodInfo lastScriptMethod;

	// position of scroll view
	private Vector2 scrollPos;

	//[MenuItem("Continuum/Continuum_Immediate {AlphaVersion}")]
	static void Init()
	{
		// get the window, show it, and hand it focus
		var continuumWindow = EditorWindow.GetWindow<TestTesta>("blabla", false);
		//var window = ScriptableObject.CreateInstance<ContinuumTextEditor>();
		//window.position

		//var cursorPos = editor.graphicalCursorPos;

		//autocompleteWindow.position = new Rect(position.position + cursorPos + new Vector2(5, 18), new Vector2(350, 200));

		//autocompleteWindow.Continuum_Init();

		//string userGuess = GetGuess(scriptText);
		//List<MemberInfo> continuumSenseGuesses = continuumSense.GuessMemberInfo(userGuess);
		//autocompleteWindow.ChangeEntries(continuumSenseGuesses);

		//autocompleteWindow.onEntryChosen += (str) => OnAutocompleteEntryChosen(editor, str);

		//autocompleteWindow.ShowPopup();

		//continuumWindow.continuumSense.Init(typeof(GameObject));

		////TODO: If I take this out, I think i have serialization throughout closing and reopening window.
		//continuumWindow.scriptText = "";

		////This line brings back the value from the last session.
		////TODO: change this to use a scriptable object like debug.

		//continuumWindow.Show();
		//continuumWindow.Focus();
		//Debug.Log("Continuum Window Initialized");
	}
}
