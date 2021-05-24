using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamingo
{
public class TESTDestino : MonoBehaviour
{
	[SerializeField] private ChariotBehavior chariotBehavior; 	/// <summary>Chariot's Behavior.</summary>

	/// <summary>TESTDestino's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		StartCoroutine(chariotBehavior.Routine(null));
	}
}
}