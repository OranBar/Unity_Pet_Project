using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class ContinuumSense_BasicCasesTests {

	ContinuumSense cs;


	[SetUp]
	public void Init()
	{
		cs = new ContinuumSense();
		cs.Init();
		cs.ScopeDown(typeof(FooNamespace.Foo1));
	}

	[Test]
	public void ListAllPossibilities()
	{
		var result = cs.Guess("");
		Assert.True(result.Contains("myInt"));
		Assert.True(result.Contains("myFloat"));
		Assert.True(result.Contains("aString"));
		Assert.True(result.Contains("property"));
		Assert.True(result.Contains("Method"));
		Assert.AreEqual(5, result.Count);
	}

	[Test]
	public void NoMatch()
	{
		var result = cs.Guess("aaaa");
		Assert.AreEqual(0, result.Count);
	}

	[Test]
	public void SomeMatches_Prefix()
	{
		var result = cs.Guess("my");
		Assert.AreEqual(2, result.Count);
		Assert.True(result.Contains("myInt"));
		Assert.True(result.Contains("myFloat"));
	}

	[Test]
	public void SomeMatches_Fuzzy()
	{
		var result = cs.Guess("in");
		Assert.LessOrEqual(2, result.Count);
		Assert.True(result.Contains("myInt"));
		Assert.True(result.Contains("aString"));
	}

	[Test]
	public void MatchField()
	{
		var result = cs.Guess("myIn");
		Assert.AreEqual(1, result.Count);
		Assert.True(result.Contains("myInt"));

		result = cs.Guess("myInt");
		Assert.AreEqual(1, result.Count);
		Assert.True(result.Contains("myInt"));
	}

	[Test]
	public void MatchProperty()
	{
		var result = cs.Guess("propert");
		Assert.AreEqual(1, result.Count);
		Assert.True(result.Contains("property"));

		result = cs.Guess("property");
		Assert.AreEqual(1, result.Count);
		Assert.True(result.Contains("property"));
	}

	[Test]
	public void MatchMethod()
	{
		var result = cs.Guess("Metho");
		Assert.AreEqual(1, result.Count);
		Assert.True(result.Contains("Method()"));

		result = cs.Guess("Method");
		Assert.AreEqual(1, result.Count);
		Assert.True(result.Contains("Method()"));

		result = cs.Guess("Method(");
		Assert.AreEqual(1, result.Count);
		Assert.True(result.Contains("Method()"));
	}

}


/*
 * 
 * public class Foo1
	{
		public int myInt;
		private float myFloat;
		protected string aString;

		public int property { get; set; }

		public void Method()
		{

		}
	}
*/