using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class ContinuumSense : MonoBehaviour {

	bool initialized = false;

	private Dictionary<Type, List<string>> typeToFields;
	private Dictionary<Type, List<string>> typeToProperties;
	private Dictionary<Type, List<string>> typeToMethods;

	private Stack<Type> type_Scope_History;
		
	private string currentLine;

	public void Init()
	{
		type_Scope_History = new Stack<Type>();


		ScanNamespace("FooNamespace");
		//Register to new input event
		
		//////

		ScopeDown(typeof(object));
		initialized = true;
	}

	private void InputListener()
	{
		//TODO: this
		//If point was pressed
			//set current line
			if (currentLine.Contains('.') == false)
			{
				var allMembersAndMethods = GetSuggestions(type_Scope_History.Peek(), "");
				//TODO: display suggestions
				DisplaySuggestionList(allMembersAndMethods);
				return;
			}
		
			string reversedLine = new string(currentLine.Reverse().ToArray());
			string guess = new string(reversedLine.TakeWhile(c => c != '.').Reverse().ToArray());
			//string caller = new string(reversedLine.SkipWhile(c => c == '.').TakeWhile(c1 => c1 != '.').Reverse().ToArray());

			var suggestions = GetSuggestions(type_Scope_History.Peek(), guess);
			DisplaySuggestionList(suggestions);
		//endIf
	}

	private void DisplaySuggestionList(List<string> suggestions)
	{
		throw new NotImplementedException();
	}

	private void ScopeDown(Type type)
	{
		type_Scope_History.Push(type);
	}

	public void ScopeUp()
	{
		Debug.Assert(type_Scope_History.Count >= 1, "Error! Can't scope up anymore.");

		type_Scope_History.Pop();
	}

	private void ScanAllNamespaces()
	{
		var allNamespacesTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
							 where t.IsClass
							 select t;

		foreach (var type in allNamespacesTypes)
		{
			typeToFields[type] = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				.Where(f => f.Name.Contains("k_BackingField") == false)
				.Select(f => f.Name)
				.ToList();

			typeToProperties[type] = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				.Select(f => f.Name)
				.ToList();

			typeToMethods[type] = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				.Select(f => f.Name)
				.ToList();

		}
	}

	private void ScanNamespace(string @namespace)
	{
	
		var namespaceTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
				where t.IsClass && t.Namespace == @namespace
				select t;

		foreach(var type in namespaceTypes)
		{
			typeToFields[type] = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				.Where(f => f.Name.Contains("k_BackingField") == false)
				.Select(f => f.Name)
				.ToList();

			typeToProperties[type] = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				.Select(f => f.Name)
				.ToList();

			typeToMethods[type] = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
				.Select(f => f.Name)
				.ToList();
		
		}
	}

	public List<string> GetSuggestions(Type scope, string guess)
	{
		List<string> result = new List<string>(typeToFields[scope]); //This creates a copy

		//This allows for complete scans of a class
		if (string.IsNullOrEmpty(guess))
		{
			return result;
		}

		for (int i = result.Count - 1; i >= 0; i--)
		{
			string field = result[i].ToLower();	//Let's be case insensitive.
			string inputCopy = "" + guess.ToLower(); //"" + and ToLower() assures we get a copy

			for (int k = inputCopy.Length - 1; k >= 0; k--)
			{
				if (field.Contains(inputCopy[k]) == false)
				{
					result.RemoveAt(i);
					continue;
				}
				inputCopy.Remove(k);
			}
		}

		result = SortResult(result);

		return result;
	}

	private List<string> SortResult(List<string> result)
	{
		//TODO: Sorting. I would start with classic statistical sorting or push-up sorting of the whole list(push up by Mathf.Floor(index/2)).
		return result;
	}
}

namespace FooNamespace {

	public class Foo1
	{
		public int myInt;
		private float myFloat;
		protected string aString;
	}
}