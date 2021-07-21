using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[Flags]
public enum DeactivationCause
{
	Impacted = 1,
	Destroyed = 2,
	LifespanOver = 4,
	Other = 8,
	LeftBoundaries = Other,

	ImpactedAndDestroyed = Impacted | Destroyed,
	ImpactedAndLifespanOver = Impacted | LifespanOver,
	All = Impacted | Destroyed | LifespanOver
}

/// <summary>Event invoked when an ID event occurs.</summary>
/// <param name="_ID">Event's ID.</param>
public delegate void OnIDEvent(int _ID);

/// <summary>Event invoked when a Collision2D intersection is received.</summary>
/// <param name="_info">Trigger2D's Information.</param>
/// <param name="_eventType">Type of the event.</param>
/// <param name="_ID">Optional ID of the HitCollider2D.</param>
public delegate void OnTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0);

/// <summary>Event invoked when the GameObject is deactivated.</summary>
/// <param name="_cause">Cause of the deactivation.</param>
/// <param name="_info">Additional Trigger2D's information.</param>
public delegate void OnDeactivated(DeactivationCause _cause, Trigger2DInformation _info);

public class EventsHandler : MonoBehaviour
{
	public event OnIDEvent onIDEvent; 					/// <summary>OnIDEvent's delegate.</summary>
	public event OnTriggerEvent onTriggerEvent; 		/// <summary>OnTriggerEvent's delegate.</summary>
	public event OnDeactivated onDeactivated; 			/// <summary>OnDeactivated's delegate.</summary>
	public event OnHealthInstanceEvent onHealthEvent; 	/// <summary>OnHealthInstanceEvent's delegate.</summary>

#if UNITY_EDITOR
	[SerializeField] private bool debug; 				/// <summary>Debug?.</summary>
#endif

	public void InvokeIDEvent(int _ID)
	{
#if UNITY_EDITOR
		if(debug) Debug.Log(
			"[EventsHandler] "
			+ gameObject.name
			+ " invoked ID Event. "
			+ _ID.ToString()
		);
#endif

		if(onIDEvent != null) onIDEvent(_ID);
	}

	/// <summary>Invokes Impact's Event.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	public void InvokeTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0)
	{
#if UNITY_EDITOR
		if(debug) Debug.Log(
			"[EventsHandler] "
			+ gameObject.name
			+ " invoked Impact Event. Interaction Type: "
			+ _eventType.ToString()
			+ ", ID: "
			+ _ID.ToString()
			+ ", "
			+ _info.ToString()
		);
#endif

		if(onTriggerEvent != null) onTriggerEvent(_info, _eventType, _ID);
	}

	/// <summary>Invokes OnDeactivation's Event and Deactivates itself [so it can be a free Pool Object resource].</summary>
	/// <param name="_cause">Cause of the Deactivation.</param>
	/// <param name="_info">Trigger2D's Information.</param>
	public void InvokeDeactivationEvent(DeactivationCause _cause, Trigger2DInformation _info = default(Trigger2DInformation))
	{
#if UNITY_EDITOR
		if(debug) Debug.Log(
			"[EventsHandler] "
			+ gameObject.name
			+ " invoked Deactivation Event. Cause: "
			+ _cause.ToString()
			+ ", "
			+ _info.ToString()
		);
#endif

		if(onDeactivated != null) onDeactivated(_cause, _info);
	}

	/// <summary>Invokes OnHealthInstanceEvent's Event.</summary>
	/// <param name="_health">Health's Instance.</param>
	/// <param name="_event">Type of Health Event.</param>
	/// <param name="_amount">Amount of health that changed [0.0f by default].</param>
	public void InvokeHealthEvent(Health _health, HealthEvent _event, float _amount = 0.0f)
	{
#if UNITY_EDITOR
		if(debug) Debug.Log(
			"[EventsHandler] "
			+ _health.name
			+ " invoked OnHealthInstanceEvent. Event: "
			+ _event.ToString()
			+ ", Amount: "
			+ _amount
			+ ", Health's Data: "
			+ _health.ToString()
		);
#endif

		if(onHealthEvent != null) onHealthEvent(_health, _event, _amount);
	}
}
}