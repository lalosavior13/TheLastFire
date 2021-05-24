using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[Serializable]
public abstract class Command<T, V> where V : ScriptableCoroutine<T>
{
	[SerializeField] private V[] _routines; 						/// <summary>Command's Routines.</summary>
	[SerializeField] private FloatRange _probabilityInterval; 		/// <summary>Probability of the command to happen.</summary>
	[SerializeField] private FloatRange _cooldownInterval; 			/// <summary>Cooldown's interval after the routines are done.</summary>

	/// <summary>Gets and Sets routines property.</summary>
	public V[] routines
	{
		get { return _routines; }
		set { _routines = value; }
	}

	/// <summary>Gets and Sets probabilityInterval property.</summary>
	public FloatRange probabilityInterval
	{
		get { return _probabilityInterval; }
		set { _probabilityInterval = value; }
	}

	/// <summary>Gets and Sets cooldownInterval property.</summary>
	public FloatRange cooldownInterval
	{
		get { return _cooldownInterval; }
		set { _cooldownInterval = value; }
	}

	/// <summary>Command default constructor.</summary>
	/// <param name="_routines">Command's Routines.</param>
	/// <param name="_probabilityInterval">Probability of the command to happen.</param>
	/// <param name="_cooldownInterval">Cooldown's interval after the routines are done.</param>
	public Command(V[] _routines, FloatRange _probabilityInterval, FloatRange _cooldownInterval)
	{
		routines = _routines;
		probabilityInterval = _probabilityInterval;
		cooldownInterval = _cooldownInterval;
	}
}
}