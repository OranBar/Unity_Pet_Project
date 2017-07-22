using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryEx {

	/// <summary>
	/// For performance reasons, this method works inplace. The dictionary calling this method will be modified.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="V"></typeparam>
	/// <param name="first"></param>
	/// <param name="second"></param>
	/// <returns></returns>
	public static void Difference<T, V>(this Dictionary<T, V> first, Dictionary<T, V> second)
	{
		foreach (var kvp in second)
		{
			V valueFirst;

			if (first.TryGetValue(kvp.Key, out valueFirst))
			{
				if (kvp.Value.Equals(valueFirst))
				{
					first.Remove(kvp.Key);
				}
			}
		}
	}
}
