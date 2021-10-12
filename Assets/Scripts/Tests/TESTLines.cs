using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;
using Flamingo;

public class TESTLines : MonoBehaviour
{
	//[SerializeField] private Line line; 			/// <summary>Line.</summary>
	[SerializeField] private Transform movable; 	/// <summary>Transform To Move.</summary>
	[SerializeField] private LinePath linePath; 	/// <summary>LinePath.</summary>
	[SerializeField] private float duration; 		/// <summary>Traverse Duration.</summary>

	/// <summary>Callback invoked when scene loads, one frame before the first Update's tick.</summary>
	private IEnumerator Start()
	{
		float t = 0.0f;
		float inverseDuration = 1.0f / duration;
		Line[] lines = linePath.lines;

		for(int i = 0; i < lines.Length; i++)
		{
			t = 0.0f;

			while(t < 1.0f)
			{
				movable.position = linePath.GetLinePoint(i, t);
				t += (Time.deltaTime * inverseDuration);
				yield return null;
			}
		}
	}
}