using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;


namespace DictionaryTest
{
	[TestFixture]
	public class DictionaryDifferenceTest
	{
		Dictionary<string, object> dict1, dict2;

		[SetUp]
		public void Init()
		{
			dict1 = new Dictionary<string, object>();
			dict2 = new Dictionary<string, object>();
			
			dict1["1"] = 1;
			dict1["2"] = 3.14f;
			dict1["3"] = 'f';
			dict1["4"] = true;
			dict1["5"] = "myString";
		}

		[Test]
		public void SameDictionary_Test() {

			dict2["1"] = 1;
			dict2["2"] = 3.14f;
			dict2["3"] = 'f';
			dict2["4"] = true;
			dict2["5"] = "myString";

			dict1.Difference(dict2);

			Assert.AreEqual(0, dict1.Count);
		}
		
		[Test]
		public void AllDifferent_Test() {

			dict2["1"] = 2;
			dict2["2"] = 1.14f;
			dict2["3"] = 's';
			dict2["4"] = false;
			dict2["5"] = "mg";

			dict1.Difference(dict2);

			Assert.AreEqual(5, dict1.Count);
		}

		[Test]
		public void MixedDictionary_Test()
		{
			//First, middle, last different
			dict2["1"] = 2;
			dict2["2"] = 3.14f;
			dict2["3"] = 'c';
			dict2["4"] = true;
			dict2["5"] = "mring";

			dict1.Difference(dict2);

			Assert.IsTrue(dict1.ContainsKey("1"));
			Assert.IsTrue(dict1.ContainsKey("2") == false);
			Assert.IsTrue(dict1.ContainsKey("3"));
			Assert.IsTrue(dict1.ContainsKey("4") == false);
			Assert.IsTrue(dict1.ContainsKey("5"));
		}



	}
}
