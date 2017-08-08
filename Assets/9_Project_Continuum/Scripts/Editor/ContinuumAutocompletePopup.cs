﻿using System.Collections;
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
	private HashSet<MemberInfo> entriesMemberInfo = new HashSet<MemberInfo>();

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

	public void ChangeEntries(IEnumerable<MemberInfo> newEntries)
	{
		entriesMemberInfo = new HashSet<MemberInfo>(newEntries);
		Repaint();
	}
			
	private Vector2 scrollPos;

	void OnGUI()
	{
		ContinuumAutocompletePopup window = ScriptableObject.CreateInstance<ContinuumAutocompletePopup>();
		scrollPos = GUILayout.BeginScrollView(scrollPos);
				
		foreach (MemberInfo entry in entriesMemberInfo)
		{
			//PropertyInfo entry_propInfo = entry as PropertyInfo;
			//FieldInfo entry_fieldInfo = entry as FieldInfo;
			//MethodInfo entry_methodInfo = entry as MethodInfo;

			var style = new GUIStyle(GUI.skin.button);
			//Color fontColor = style.normal.textColor;

			style.fontStyle = FontStyle.Bold;

			if (entry.MemberType == MemberTypes.Property)
			{
				//style.normal.textColor = Color.blue;
				style.normal.textColor = new Color(10 / 255f, 110 / 255f, 150 / 255f);
				//Debug.Log("magenta");
			}
			else if (entry.MemberType == MemberTypes.Field)
			{
				//style.normal.textColor = Color.white;
				style.normal.textColor = new Color(56 / 255f, 40 / 255f, 0f / 255f);
			}
			else if (entry.MemberType == MemberTypes.Method)
			{
				//style.normal.textColor = Color.green;
				style.normal.textColor = new Color(15 / 255f, 100 / 255f, 0 / 255f);
			}

			//style.normal.textColor = fontColor;

			if (GUILayout.Button(entry.Name, style))
			{
				onEntryChosen(entry.Name);
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
