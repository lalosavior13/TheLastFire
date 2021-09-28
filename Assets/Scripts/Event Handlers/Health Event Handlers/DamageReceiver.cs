using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
/// \TODO Think it through before developing this class.
[RequireComponent(typeof(Health))]
public class DamageReceiver : MonoBehaviour
{
	private Health _health; 	/// <summary>Health's Component.</summary>

	/// <summary>Gets health Component.</summary>
	public Health health
	{ 
		get
		{
			if(_health == null) _health = GetComponent<Health>();
			return _health;
		}
	}

	/// <summary>DamageReceiver's instance initialization.</summary>
	private void Awake()
	{
		health.onHealthEvent += OnHealthEvent;
	}

	/// <summary>Event invoked when a Health's event has occured.</summary>
	/// <param name="_event">Type of Health Event.</param>
	/// <param name="_amount">Amount of health that changed [0.0f by default].</param>
	/// <param name="_object">GameObject that caused the event, null be default.</param>
	private void OnHealthEvent(HealthEvent _event, float _amount = 0.0f, GameObject _object = null)
	{

	}
}
}