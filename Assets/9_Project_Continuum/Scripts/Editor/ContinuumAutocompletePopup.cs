using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

public class ContinuumAutocompletePopup : EditorWindow
{
	public event Action<string> onEntryChosen;
			
	//I'm gonna make it a hashset for now to ignore duplicates. Those duplicates, though, are important: They are overloaded methods. We need to consider those.
	private HashSet<string> entries = new HashSet<string>();
			
	static void Init()
	{
		ContinuumAutocompletePopup window = ScriptableObject.CreateInstance<ContinuumAutocompletePopup>();
		window.ShowPopup();
	}

	public void Continuum_Init()
	{
		onEntryChosen = (s) => { };
	}

	public void Continuum_Init(IEnumerable<string> entries)
	{
		onEntryChosen = (s) => { };
		this.entries = new HashSet<string>(entries);
	}

	public void ChangeEntries(IEnumerable<string> newEntries)
	{
		entries = new HashSet<string>(newEntries);
		Repaint();
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
		Repaint();
	}

	internal void SimulateSelectFirstEntry()
	{
		onEntryChosen(entries.First());
	}
}
