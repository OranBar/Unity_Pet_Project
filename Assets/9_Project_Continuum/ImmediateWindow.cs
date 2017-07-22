// Oran Bar™

using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Microsoft.CSharp;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Text;

public class ImmediateWindow : MonoBehaviour {

	
	public static string ConvertPrivateInvocations_ToReflectionInvokes(string source)
	{
		string result = "";

		source = source.Replace("..", "!");
		//source += ";";
		var splitSource = source.Split('!');
		
		//Early out
		if(splitSource.Length == 0)
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
			
			Debug.Log("caller <color=red>" + caller+"</color>\n"
				+ "invocation <color=red>" + invocation + "</color>\n"
				+ "charAfterInvocation is "+ charAfterInvocation);

			string replacement = "";
			string before, after;

			switch (charAfterInvocation)
			{
				//This is a set
				case '=':
					
					string value = source.Substring( source.IndexOf(afterOperator) + afterOperator.IndexOf(charAfterInvocation)+1).Trim();
					value = value.Remove(value.Length-1);	//Let's take out the ";"
					replacement = string.Format("{0}.GetType().SetField(\"{1}\", {2});", caller, invocation, value);
					Debug.Log(replacement);

					before = new string(beforeOperator.Take(callerStartIndex).ToArray());
					result += before + replacement;

					break;
				//This is a Get
				case '.':
					replacement = string.Format("{0}.GetType().GetField(\"{1}\")", caller, invocation);
					Debug.Log(replacement);

					before = new string(beforeOperator.Take(callerStartIndex).ToArray());
					after = new string(afterOperator.Skip(invocationEndIndex).ToArray());
					result += before + replacement + after;

					break;
			}
			Debug.Log("result is "+result);
		}

		return null;
	}

}


