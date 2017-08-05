using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class ContinuumAutocompletePopup : EditorWindow
{
	public event Action<string> onEntryChosen;
			
	private List<string> entries = new List<string>();
			
	static void Init()
	{
		ContinuumAutocompletePopup window = ScriptableObject.CreateInstance<ContinuumAutocompletePopup>();
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
		ContinuumAutocompletePopup window = ScriptableObject.CreateInstance<ContinuumAutocompletePopup>();
		scrollPos = GUILayout.BeginScrollView(scrollPos);
				
		foreach (string entry in entries)
		{
			if (GUILayout.Button(entry))
			{
				onEntryChosen(entry);
			}
		}
		GUILayout.EndScrollView();
	}
			
}
