using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class DestinoScriptableCoroutine : ScriptableCoroutine<DestinoBoss>
{
	[SerializeField] private bool _run; 	/// <summary>Run this Coroutine?.</summary>

	/// <summary>Gets run property.</summary>
	public bool run { get { return _run; } }

	/// <summary>Coroutine's IEnumerator.</summary>
	/// <param name="boss">Object of type T's argument.</param>
	public override IEnumerator Routine(DestinoBoss boss) { yield return null; }

	/// <summary>Finishes the Routine.</summary>
	/// <param name="boss">Object of type T's argument.</param>
	public override void FinishRoutine(DestinoBoss boss) { /*...*/ }
}
}