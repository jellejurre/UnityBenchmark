using System;
using System.Linq;
using UnityEngine.UIElements;

public class Util
{
	public static T[] CreateArr<T>(Func<int, T> func, int amount)
	{
		return Enumerable.Range(0, amount).Select(x => func(x)).ToArray();
	}
}