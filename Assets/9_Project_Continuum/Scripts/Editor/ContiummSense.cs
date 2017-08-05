using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace TonRan.Continuum
{
	public class ContinuumSense
	{

		bool initialized = false;

		private Dictionary<Type, List<MemberInfo>> typeToMembers;


		//private Dictionary<Type, List<string>> typeToFields;
		//private Dictionary<Type, List<string>> typeToProperties;
		//private Dictionary<Type, List<string>> typeToMethods;

		private Stack<Type> type_scope_history;

		private string currentLine;


		public void Init(/*TODO: Add parameters: Namespaces to analyze*/)
		{
			type_scope_history = new Stack<Type>();
			//typeToFields = new Dictionary<Type, List<string>>();
			//typeToProperties = new Dictionary<Type, List<string>>();
			//typeToMethods = new Dictionary<Type, List<string>>();
			typeToMembers = new Dictionary<Type, List<MemberInfo>>();


			//ScanNamespace("FooNamespace", true);
			ScanAllAssembly(false);
			//Register to new input event

			//////

			type_scope_history.Push(null);
			initialized = true;
		}

		private void InputListener()
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			//TODO: this
			//If point was pressed
			//set current line
			if (currentLine.Contains('.') == false)
			{
				var allMembersAndMethods = Guess(type_scope_history.Peek(), "");
				//TODO: display suggestions
				DisplaySuggestionList(allMembersAndMethods);
				return;
			}

			string reversedLine = new string(currentLine.Reverse().ToArray());
			string guess = new string(reversedLine.TakeWhile(c => c != '.').Reverse().ToArray());
			//string caller = new string(reversedLine.SkipWhile(c => c == '.').TakeWhile(c1 => c1 != '.').Reverse().ToArray());

			var suggestions = Guess(type_scope_history.Peek(), guess);
			DisplaySuggestionList(suggestions);
			//endIf
		}

		public void ScopeDown(Type type)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			type_scope_history.Push(type);
		}

		public void ScopeUp()
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			Debug.Assert(type_scope_history.Count >= 1, "Error! Can't scope up anymore.");

			type_scope_history.Pop();
		}

		private void DisplaySuggestionList(List<string> suggestions)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			throw new NotImplementedException();
		}

		private void ScanAllAssembly(bool includePrivateVariables)
		{
			var allAssemblyTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
									 where t.IsClass
									 select t;

			ScanTypes(allAssemblyTypes, includePrivateVariables);
		}

		private void ScanTypes(IEnumerable<Type> types, bool includePrivateVariables)
		{
			BindingFlags reflectionOptions = BindingFlags.Public | BindingFlags.Instance;
			if (includePrivateVariables)
			{
				reflectionOptions = reflectionOptions | BindingFlags.NonPublic;
			}

			foreach (var type in types)
			{
				typeToMembers[type] = new List<MemberInfo>();

				typeToMembers[type].AddRange((type.GetFields(reflectionOptions)
					.Where(f => f.Name.Contains("k__BackingField") == false)
					.Cast<MemberInfo>()
					.ToList()
				));

				typeToMembers[type].AddRange((type.GetProperties(reflectionOptions)
					.Cast<MemberInfo>()
					.ToList()
				));

				typeToMembers[type].AddRange((type.GetMethods(reflectionOptions)
					.Where(method => method.Name != "Finalize" && method.Name != "obj_address" && method.Name != "MemberwiseClone")
					//Discard methods such as get_propertyName and set_propertyName
					.Where(m => (m.Name.StartsWith("get_") == false && m.Name.StartsWith("set_") == false) || typeToMembers[type].Select(mi => mi.Name).Contains(m.Name.Substring(4)) == false)
					.Cast<MemberInfo>()
					.ToList()
				));

			}
		}

		private void ScanNamespace(string @namespace, bool includePrivateVariables)
		{
			var namespaceTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
								 where t.IsClass && t.Namespace == @namespace
								 select t;


			//Scan object, but don't allow access to its nonpublic members
			//typeToMembers[typeof(UnityEngine.Object)] = (typeof(UnityEngine.Object).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
			//	.Where(f => f.Name.Contains("k__BackingField") == false)
			//	.Cast<MemberInfo>()
			//	.ToList()
			//);

			//typeToMembers[typeof(UnityEngine.Object)] = (typeof(UnityEngine.Object).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
			//	.Cast<MemberInfo>()
			//	.ToList()
			//);

			//typeToMembers[typeof(UnityEngine.Object)] = (typeof(UnityEngine.Object).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
			//	.Cast<MemberInfo>()
			//	.ToList()
			//);
			//-----------------------------------------------------------------

			BindingFlags reflectionOptions = BindingFlags.Public | BindingFlags.Instance;
			if (includePrivateVariables)
			{
				reflectionOptions = reflectionOptions | BindingFlags.NonPublic;
			}

			foreach (var type in namespaceTypes)
			{
				typeToMembers[type] = new List<MemberInfo>();

				typeToMembers[type].AddRange((type.GetFields(reflectionOptions)
					.Where(f => f.Name.Contains("k__BackingField") == false)
					.Cast<MemberInfo>()
					.ToList()
				));

				typeToMembers[type].AddRange((type.GetProperties(reflectionOptions)
					.Cast<MemberInfo>()
					.ToList()
				));

				typeToMembers[type].AddRange((type.GetMethods(reflectionOptions)
					.Where(method => method.Name != "Finalize" && method.Name != "obj_address" && method.Name != "MemberwiseClone")
					//Discard methods such as get_propertyName and set_propertyName
					.Where(m => (m.Name.StartsWith("get_") == false && m.Name.StartsWith("set_") == false) || typeToMembers[type].Select(mi => mi.Name).Contains(m.Name.Substring(4)) == false)
					.Cast<MemberInfo>()
					.ToList()
				));

			}
		}

		/// <summary>
		/// Uses the current scope to guess the next symbol.
		/// </summary>
		/// <param name="guess"></param>
		/// <returns></returns>
		public List<string> Guess(Type typeScope, string guess)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			List<string> result = new List<string>();
			foreach (var membersList in typeToMembers.Values)
			{
				result.AddRange(membersList.Select(mi => mi.Name));
			}

			//Special Case: We list everything we got. If this happens, the programmer needs all the help he can get.
			if (string.IsNullOrEmpty(guess))
			{
				return result;
			}

			//Filter all symbols SHORTER than the guess. 
			result = result.Where(symbol => symbol.Length >= guess.Length).ToList();

			outer: for (int i = result.Count - 1; i >= 0; i--)
			{
				string field = result[i].ToLower(); //Let's be case insensitive.
				string inputCopy = "" + guess.ToLower();

				//Loop InputCopy (reversed). For each character, either delete that character in field, or continue to next field if that character isn't contained in the field
				for (int k = inputCopy.Length - 1; k >= 0; k--)
				{
					char currChar = inputCopy[k];

					if (field.Contains(currChar) == false)
					{
						//This is the only place where we modify result. So, it starts with all options, and then we remove the ones that don't match. The rest stays.
						result.RemoveAt(i);
						goto outer;
					}

					//If we skipped the previous if, we can proceed to remove the currChar from both field and inputcopy (ordering is important, we take out the last one)
					field = field.Remove(
						field.LastIndexOf(currChar),
						1
					);

					inputCopy = inputCopy.Remove(k, 1);
				}
			}

			result = SortResult(result);

			return result;
		}

		public List<string> GetAllGuesses()
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			return Guess("");
		}

		/// <summary>
		/// Uses the current scope to guess the next symbol.
		/// </summary>
		/// <param name="guess"></param>
		/// <returns></returns>
		public List<string> Guess(string guess)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			return Guess(type_scope_history.Peek(), guess);
		}

		private List<string> SortResult(List<string> result)
		{
			//TODO: Sorting. I would start with classic statistical sorting or push-up sorting of the whole list(push up by Mathf.Floor(index/2)).
			return result;
		}
	}
}

namespace FooNamespace
{

	public class Foo1
	{
		public int myInt;
		private float myFloat;
		protected string aString;

		public int Property { get; set; }

		public void Method()
		{

		}
	}
}