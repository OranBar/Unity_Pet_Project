using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TonRan.Continuum
{
	public class ContinuumEditorWindow : EditorWindow {

		public GUIStyle myStyle = null;

		private static ContinuumSense _cSense;
		public static ContinuumSense cSense {
			get {
				if(_cSense == null){
					_cSense = new ContinuumSense();
					_cSense.Init();
				}
				return _cSense;
			}
		}

		public string code = "Hello World\nI've got 2 lines...";

		[MenuItem("Continuum/Continuum Window")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow<ContinuumEditorWindow>("Continuum").Show();
		}

		void OnGUI()
		{
			#region VerticalGroup
			//GUILayout.BeginVertical("Background");

			code = EditorGUILayout.TextArea(code, GUILayout.Width(350), GUILayout.Height(100));

			string parsedCode = code.Replace("..", "!");
			if (GUILayout.Button("Guess!"))
			{
				string userInput = GetLastMember(parsedCode);
				cSense.Guess(userInput).ForEach(Debug.Log);
			}
			if (GUILayout.Button("Show All Guesses"))
			{
				cSense.GetAllGuesses().ForEach(Debug.Log);
			}

			//GUILayout.EndVertical();
#endregion
		}


		private string GetLastMember(string line)
		{
			string[] orderedMembersInLine = line.Split('.', '!');
			return orderedMembersInLine.Last();
		}
		
	}
}
