using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(EventsHandler))]
public abstract class EventsListener : MonoBehaviour
{
	private EventsHandler _eventsHandler;

	/// <summary>Gets eventsHandler Component.</summary>
	public EventsHandler eventsHandler
	{ 
		get
		{
			if(_eventsHandler == null) _eventsHandler = GetComponent<EventsHandler>();
			return _eventsHandler;
		}
	}

	/// <summary>EventsListener's instance initialization.</summary>
	protected virtual void Awake()
	{
		eventsHandler.onIDEvent += OnIDEvent;
		eventsHandler.onTriggerEvent += OnTriggerEvent;
		eventsHandler.onDeactivated += OnDeactivated;
	}

	/// <summary>Callback invoked when an ID event occurs.</summary>
	/// <param name="_ID">Event's ID.</param>
	protected abstract void OnIDEvent(int _ID);

	/// <summary>Callback invoked when a Collision2D intersection is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	protected abstract void OnTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0);

	/// <summary>Callback invoked when the GameObject is deactivated.</summary>
	/// <param name="_cause">Cause of the deactivation.</param>
	/// <param name="_info">Additional Trigger2D's information.</param>
	protected abstract void OnDeactivated(DeactivationCause _cause, Trigger2DInformation _info);
}
}