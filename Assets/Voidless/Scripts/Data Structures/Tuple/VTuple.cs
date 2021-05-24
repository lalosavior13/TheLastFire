using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
[System.Serializable]
public class VTuple<T1, T2>
{
	public T1 Item1; 	/// <summary>Item 1.</summary>
	public T2 Item2; 	/// <summary>Item 2.</summary>

	/// <summary>Tuple default constructor.</summary>
	/// <param name="_Item1">Item 1.</param>
	/// <param name="_Item2">Item 2.</param>
	public VTuple(T1 _Item1, T2 _Item2)
	{
		Item1 = _Item1;
		Item2 = _Item2;
	}
}
}