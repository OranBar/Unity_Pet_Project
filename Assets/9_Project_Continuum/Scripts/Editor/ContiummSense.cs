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

		public bool initialized { get; private set; }	//Starts false!

		private Dictionary<Type, List<MemberInfo>> typeToMembers;
		
		//private Dictionary<Type, List<string>> typeToFields;
		//private Dictionary<Type, List<string>> typeToProperties;
		//private Dictionary<Type, List<string>> typeToMethods;

		private Stack<Type> type_scope_history;

		private Type baseType = null;

		public Type CurrentScope {
			get {
				if (initialized == false) { return null; }
				else return type_scope_history.Peek();
			}
		}

		public void Init(/*TODO: Add parameters: Namespaces to analyze*/)
		{
			type_scope_history = new Stack<Type>();
			//typeToFields = new Dictionary<Type, List<string>>();
			//typeToProperties = new Dictionary<Type, List<string>>();
			//typeToMethods = new Dictionary<Type, List<string>>();
			typeToMembers = new Dictionary<Type, List<MemberInfo>>();

			/*
			 * using UnityEngine; //0
			using UnityEditor; //1
			using System.Collections; //2
			using System.Collections.Generic; //3
			using System.Text; //4
			using System.Linq; //5
			using MyNamespace; //6
			*/
			ScanNamespace("UnityEditor", false);
			//ScanNamespace("UnityEngine", false);
			ScanAssembly(typeof(ZDontTouch_Continuum).Assembly, false);
			ScanAssembly(typeof(UnityEngine.GameObject).Assembly, false);
			ScanNamespace("System.Collections", false);
			ScanNamespace("System.Collections.Generic", false);
			ScanNamespace("System.Text", false);
			ScanNamespace("System.Linq", false);
			ScanNamespace("MyNamespace", false);

			//ScanType(typeof(UnityEngine.GameObject), false);
			//ScanAllAssembly(false);
			//Register to new input event

			//////

			type_scope_history.Push(null);
			initialized = true;
		}

		public void Init(Type baseType) 
		{
			Init();
			this.baseType = baseType;
			type_scope_history.Push(baseType);
		}

		private void CurrentLineChanged(string previous, string current)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			//TODO: this
			//If point was pressed
			//set current line
			if (current.Contains('.') == false)
			{
				var allMembersAndMethods = Guess(type_scope_history.Peek(), "");
				//TODO: display suggestions
				DisplaySuggestionList(allMembersAndMethods);
				return;
			}

			string reversedLine = new string(current.Reverse().ToArray());
			string guess = new string(reversedLine.TakeWhile(c => c != '.').Reverse().ToArray());
			//string caller = new string(reversedLine.SkipWhile(c => c == '.').TakeWhile(c1 => c1 != '.').Reverse().ToArray());

			var suggestions = Guess(type_scope_history.Peek(), guess);
			DisplaySuggestionList(suggestions);
			//endIf
		}

		public void ScopeDown(Type type)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }
			if (type_scope_history.Peek() == null) { throw new ContinuumNotInitializedException("Current type is NULL. Please use ScopeDown at least once in the initialization"); }


			type_scope_history.Push(type);
			Debug.Log("Scoped Down: Type is "+type);
		}

		public void ScopeDown(string memberName)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }
			if (type_scope_history.Peek() == null){	throw new ContinuumNotInitializedException("Current type is NULL. Please use ScopeDown at least once in the initialization");	}

			Type type = GetGuessType(memberName);

			ScopeDown(type);
		}

		public void ScopeUp()
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			Debug.Assert(type_scope_history.Count >= 1, "Error! Can't scope up anymore.");

			type_scope_history.Pop();
		}

		public void ScopeAllTheWayUp()
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			Debug.Assert(type_scope_history.Count >= 1, "Error! Can't scope up anymore.");

			type_scope_history.Clear();
			type_scope_history.Push(baseType);
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

		private void ScanType(Type type, bool includePrivateVariables)
		{
			ScanTypes(new Type[] { type }, includePrivateVariables);
		}


		private void ScanTypes(IEnumerable<Type> types, bool includePrivateVariables)
		{
			BindingFlags reflectionOptions = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
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

		private void ScanAssembly(Assembly a, bool includePrivateVariables) 
		{
			ScanTypes(a.GetTypes(), includePrivateVariables);
		}

		private void ScanNamespace(string @namespace, bool includePrivateVariables)
		{
			var namespaceTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
								 where t.IsClass && t.Namespace == @namespace
								 select t;

			//var namespaceTypes = AppDomain.CurrentDomain.GetAssemblies()
			//		   .SelectMany(t => t.GetTypes())
			//		   .Where(t => t.IsClass && t.Namespace == @namespace);

			ScanTypes(namespaceTypes, includePrivateVariables);


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

		}

		/*
		public List<MemberInfo> GuessMember(Type typeScope, string guess)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			List<MemberInfo> result = new List<MemberInfo>();

			if (typeScope == null)
			{
				foreach (var membersList in typeToMembers.Values)
				{
					result.AddRange(membersList);
				}
			}
			else
			{
				result.AddRange(typeToMembers[typeScope]);
			}


			//Special Case: We list everything we got. If this happens, the programmer needs all the help he can get.
			if (string.IsNullOrEmpty(guess))
			{
				return result;
			}

			//Filter all symbols SHORTER than the guess. 
			result = result
				.Where(symbol => symbol.Name.Length >= guess.Length)
				.ToList();

			outer: for (int i = result.Count - 1; i >= 0; i--)
			{
				string field = result[i].Name.ToLower(); //Let's be case insensitive.
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
		*/

		public List<string> GetAllTypes()
		{
			return typeToMembers.Keys.Select(k => k.Name).ToList();
		}

		public Type GetGuessType(string guess)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }
			
			string predictedType = Guess(guess)[0];
			Type typeScope = type_scope_history.Peek();
			return GetGuessType(typeToMembers[typeScope].First(typeName => typeName.Name == predictedType));
		}

		public Type GetGuessType(MemberInfo guess)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }
			Type type = null;
			
			Debug.Assert(guess != null);
			if (guess is PropertyInfo)
			{
				type = ((PropertyInfo)guess).PropertyType;
			}
			if (guess is FieldInfo)
			{
				type = ((FieldInfo)guess).FieldType;
			}
			if (guess is MethodInfo)
			{
				type = ((MethodInfo)guess).ReturnType;
			}
			return type;
		}



		/// <summary>
		/// Uses the current scope to guess the next symbol.
		/// </summary>
		/// <param name="guess"></param>
		/// <returns></returns>
		public List<string> Guess(Type typeScope, string guess)
		{
			if (initialized == false) { throw new ContinuumNotInitializedException(); }

			try
			{
				Debug.Log("Guessing. Scope is "+typeScope.Name);
			}
			catch
			{
				Debug.Log("Guessing. Scope is null");
			}
			List<string> result = new List<string>();

			if(typeScope == null)
			{
				foreach (var membersList in typeToMembers.Values)
				{
					result.AddRange(membersList.Select(mi => mi.Name));
				}
			}
			else
			{
				result.AddRange(typeToMembers[typeScope].Select(mi => mi.Name));
			}
			

			//Special Case: We list everything we got. If this happens, the programmer needs all the help he can get.
			if (string.IsNullOrEmpty(guess))
			{
				return result;
			}

			//Filter all symbols SHORTER than the guess. 
			result = result.Where(symbol => symbol.Length >= guess.Length).ToList();

		
outer:      for (int i = result.Count - 1; i >= 0; i--)
			{
				string field = result[i].ToLower(); //Let's be case insensitive.
				string inputCopy = "" + guess.ToLower();

				int fuzzyMinIndex = field.Length-1;

				//Loop InputCopy (reversed). For each character, either delete that character in field, or continue to next field if that character isn't contained in the field
				int k = inputCopy.Length-1;
				while(k >= 0)
				{
					char currChar = inputCopy[k];

					while (field[fuzzyMinIndex] != currChar)
					{
						fuzzyMinIndex--;
						if(fuzzyMinIndex < 0)
						{
							result.RemoveAt(i);
							goto outer;
						}
					}

					k--;

					//if (field.Contains(currChar) == false)
					//{
					//	//This is the only place where we modify result. So, it starts with all options, and then we remove the ones that don't match. The rest stays.
					//	result.RemoveAt(i);
					//	goto outer;
					//}

					//If we skipped the previous if, we can proceed to remove the currChar from both field and inputcopy (ordering is important, we take out the last one)
					//field = field.Remove(
					//	field.LastIndexOf(currChar),
					//	1
					//);

					//inputCopy = inputCopy.Remove(k, 1);
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

		private List<MemberInfo> SortResult(List<MemberInfo> result)
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
 