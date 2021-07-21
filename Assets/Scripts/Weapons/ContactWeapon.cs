using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(EventsHandler))]
[RequireComponent(typeof(ImpactEventHandler))]
public class ContactWeapon : PoolGameObject
{
	[Space(5f)]
	[Header("Damage's Attributes:")]
	[SerializeField] private GameObjectTag[] _healthAffectableTags; 	/// <summary>Tags of GameObjects whose Health is affected by this ContactWeapon.</summary>
	[SerializeField] private float _damage; 							/// <summary>Damage that this ContactWeapon applies.</summary>
	[SerializeField] private float[] _damageScales; 					/// <summary>Damage Scale relative to its HitBox [HitCollider2D].</summary>
	[Space(5f)]
	[Header("Impact's Attributes:")]
	[SerializeField] private GameObjectTag[] _impactTags; 				/// <summary>Tags of GameObject affected by impact.</summary>
	[Space(5f)]
	[Header("Trail's References:")]
	[SerializeField] private TrailRenderer[] _trailRenderers; 			/// <summary>TrailRenderers' Component.</summary>
	private HashSet<int> _objectsIDs; 									/// <summary>Set of GameObject's IDs that are already inside of HitBoxes [to avoid repetition of actions].</summary>
	private EventsHandler _eventsHandler; 								/// <summary>EventsHandler's Component.</summary>
	private ImpactEventHandler _impactEventHandler; 					/// <summary>ImpactEventHandler's Component.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets healthAffectableTags property.</summary>
	public GameObjectTag[] healthAffectableTags
	{
		get { return _healthAffectableTags; }
		set { _healthAffectableTags = value; }
	}

	/// <summary>Gets and Sets impactTags property.</summary>
	public GameObjectTag[] impactTags
	{
		get { return _impactTags; }
		set { _impactTags = value; }
	}

	/// <summary>Gets and Sets damage property.</summary>
	public float damage
	{
		get { return _damage; }
		set { _damage = value; }
	}

	/// <summary>Gets and Sets damageScales property.</summary>
	public float[] damageScales
	{
		get { return _damageScales; }
		set { _damageScales = value; }
	}

	/// <summary>Gets and Sets trailRenderers property.</summary>
	public TrailRenderer[] trailRenderers
	{
		get { return _trailRenderers; }
		set { _trailRenderers = value; }
	}

	/// <summary>Gets and Sets objectsIDs property.</summary>
	public HashSet<int> objectsIDs
	{
		get { return _objectsIDs; }
		set { _objectsIDs = value; }
	}

	/// <summary>Gets eventsHandler Component.</summary>
	public EventsHandler eventsHandler
	{ 
		get
		{
			if(_eventsHandler == null) _eventsHandler = GetComponent<EventsHandler>();
			return _eventsHandler;
		}
	}

	/// <summary>Gets impactEventHandler Component.</summary>
	public ImpactEventHandler impactEventHandler
	{ 
		get
		{
			if(_impactEventHandler == null) _impactEventHandler = GetComponent<ImpactEventHandler>();
			return _impactEventHandler;
		}
	}
#endregion

	/// <summary>ContactWeapon's instance initialization.</summary>
	private void Awake()
	{
		objectsIDs = new HashSet<int>();
		ActivateHitBoxes(false);
		eventsHandler.onTriggerEvent += OnTriggerEvent;
		OnAwake();
	}

	/// <summary>Callback internally invoked after Awake.</summary>
	protected virtual void OnAwake(){/*...*/}

	/// <summary>Callback invoked when ContactWeapon's instance is going to be destroyed and passed to the Garbage Collector.</summary>
	private void OnDestroy()
	{
		eventsHandler.onTriggerEvent -= OnTriggerEvent;
	}

	/// <summary>Activates HitBoxes contained within ContactWeapon.</summary>
	/// <param name="_activate">Activate? [true by default].</param>
	public virtual void ActivateHitBoxes(bool _activate = true)
	{
		if(trailRenderers != null)
		foreach(TrailRenderer trailRenderer in trailRenderers)
		{
			trailRenderer.emitting = _activate;
		}

		impactEventHandler.ActivateHitBoxes(_activate);
	}

	/// <summary>Event invoked when a Collision2D intersection is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	public virtual void OnTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0)
	{
		GameObject obj = _info.collider.gameObject;
		int instanceID = obj.GetInstanceID();

/// \TODO Soon to delete (regarding the debugging. I hope at least...)
#region Debug:
		StringBuilder builder = new StringBuilder();

		builder.Append("OnTriggerEvent invoked to class ");
		builder.AppendLine(name);
		builder.Append("Triggered with: ");
		builder.AppendLine(obj.name);
		builder.Append("Tag: ");
		builder.AppendLine(obj.tag);
		builder.Append("Layer: ");
		builder.AppendLine(obj.layer.ToString());
		builder.Append("Event Type: ");
		builder.AppendLine(_eventType.ToString());

		Debug.Log(builder.ToString());
#endregion

		/// \TODO Maybe separate into its own DamageApplier class?
		if(healthAffectableTags != null) foreach(GameObjectTag tag in healthAffectableTags)
		{
			switch(_eventType)
			{
				case HitColliderEventTypes.Enter:
				if(objectsIDs.Contains(instanceID)) return;

				if(obj.CompareTag(tag))
				{
					objectsIDs.Add(instanceID);

					Health health = obj.GetComponentInParent<Health>();
					
					if(health == null)
					{
						HealthLinker linker = obj.GetComponent<HealthLinker>();
						if(linker != null) health = linker.component;
					}

					if(health != null)
					{
						Debug.Log("[ContactWeapon] " + name + " should apply damage to " + health.name);
						float damageScale = damageScales != null ? damageScales[Mathf.Clamp(_ID, 0, damageScales.Length)] : 1.0f;
						float damageApplied = damage * damageScale;
						
						health.GiveDamage(damageApplied);
						OnHealthInstanceDamaged(health);
						return;
					}
				}
				break;

				case HitColliderEventTypes.Exit:
				if(objectsIDs.Contains(instanceID)) objectsIDs.Remove(instanceID);
				break;
			}
		}
		if(impactTags != null) foreach(GameObjectTag tag in impactTags)
		{
			if(obj.CompareTag(tag)) OnImpact(_info, _ID);
		}
	}

	/// <summary>Callback internally invoked when a Health's instance was damaged.</summary>
	/// <param name="_health">Health's instance that was damaged.</param>
	protected virtual void OnHealthInstanceDamaged(Health _health) {/*...*/}

	/// <summary>Callback internally called when there was an impact.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_ID">ID of the HitCollider2D.</param>
	protected virtual void OnImpact(Trigger2DInformation _info, int _ID = 0) { /*...*/ }

	/// <summary>Event invoked when an impact is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	public virtual void OnImpactEvent(Trigger2DInformation _info) {/*...*/}
}
}