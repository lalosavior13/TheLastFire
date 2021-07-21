using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
/// \TODO Make the F#@*ng SerializableDictionary work with all custom data types...
[Serializable] public struct IntShakeDataPair { public int ID; public ShakeData data; }
[Serializable] public struct TagShakeDataPair { public GameObjectTag tag; public ShakeData data; }
[Serializable] public struct DeactivationCauseShakeDataPair { public DeactivationCause cause; public ShakeData data; }

public class ShakeEventsListener : EventsListener
{
	[SerializeField] private IntShakeDataPair[] _IDEvents; 							/// <summary>ID Events to subscribe.</summary>
	[SerializeField] private TagShakeDataPair[] _impactEvents; 						/// <summary>Impact Events to subscribe.</summary>
	[SerializeField] private DeactivationCauseShakeDataPair[] _deactivationEvents; 	/// <summary>Deactivation Events to Listen.</summary>
	private Dictionary<int, ShakeData> _IDEventsDic; 									/// <summary>Dictionary of ID Events Subscribed.</summary>
	private Dictionary<GameObjectTag, ShakeData> _impactEventsDic; 					/// <summary>Dictionary of Impact Events Subscribed.</summary>
	private Dictionary<DeactivationCause, ShakeData> _deactivationEventsDic; 			/// <summary>Dictionary of Deactivation Events Subscribed.</summary>

	/// <summary>Gets IDEvents property.</summary>
	public IntShakeDataPair[] IDEvents { get { return _IDEvents; } }

	/// <summary>Gets impactEvents property.</summary>
	public TagShakeDataPair[] impactEvents { get { return _impactEvents; } }

	/// <summary>Gets deactivationEvents property.</summary>
	public DeactivationCauseShakeDataPair[] deactivationEvents { get { return _deactivationEvents; } }

	/// <summary>Gets and Sets IDEventsDic property.</summary>
	public Dictionary<int, ShakeData> IDEventsDic
	{
		get { return _IDEventsDic; }
		protected set { _IDEventsDic = value; }
	}

	/// <summary>Gets and Sets impactEventsDic property.</summary>
	public Dictionary<GameObjectTag, ShakeData> impactEventsDic
	{
		get { return _impactEventsDic; }
		protected set { _impactEventsDic = value; }
	}

	/// <summary>Gets and Sets deactivationEventsDic property.</summary>
	public Dictionary<DeactivationCause, ShakeData> deactivationEventsDic
	{
		get { return _deactivationEventsDic; }
		protected set { _deactivationEventsDic = value; }
	}

	/// <summary>EmitParticleEffectEventsListener's instance initialization when loaded [Before scene loads].</summary>
	protected override void Awake()
	{
		base.Awake();
	}

	/// <summary>Callback invoked when an ID event occurs.</summary>
	/// <param name="_ID">Event's ID.</param>
	protected override void OnIDEvent(int _ID)
	{

	}

	/// <summary>Callback invoked when a Collision2D intersection is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	protected override void OnTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0)
	{

	}

	/// <summary>Callback invoked when the GameObject is deactivated.</summary>
	/// <param name="_cause">Cause of the deactivation.</param>
	/// <param name="_info">Additional Trigger2D's information.</param>
	protected override void OnDeactivated(DeactivationCause _cause, Trigger2DInformation _info)
	{

	}
}
}