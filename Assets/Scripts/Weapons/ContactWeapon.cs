using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(ImpactEventHandler))]
public class ContactWeapon : PoolGameObject
{
	[Space(5f)]
	[Header("Contact Weapon's Attributes:")]
	[SerializeField] private LayerMask _healthAffectableMask; 	/// <summary>Mask that contains GameObjects that may have their Health affected by this Weapon.</summary>
	[SerializeField] private LayerMask _impactAffectableMask; 	/// <summary>Mask that contains GameObjects that may be affected by the impact of this Weapon.</summary>
	[SerializeField] private float _damage; 					/// <summary>Damage that this ContactWeapon applies.</summary>
	[SerializeField] private HitColliderInfo[] _hitBoxesInfo; 	/// <summary>HitBoxes' Information.</summary>
	[SerializeField] private TrailRenderer[] _trailRenderers; 	/// <summary>TrailRenderers' Component.</summary>
	private ImpactEventHandler _impactHandler; 					/// <summary>ImpactEventHandler's Component.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets healthAffectableMask property.</summary>
	public LayerMask healthAffectableMask
	{
		get { return _healthAffectableMask; }
		set { _healthAffectableMask = value; }
	}

	/// <summary>Gets and Sets impactAffectableMask property.</summary>
	public LayerMask impactAffectableMask
	{
		get { return _impactAffectableMask; }
		set { _impactAffectableMask = value; }
	}

	/// <summary>Gets and Sets damage property.</summary>
	public float damage
	{
		get { return _damage; }
		set { _damage = value; }
	}

	/// <summary>Gets and Sets hitBoxesInfo property.</summary>
	public HitColliderInfo[] hitBoxesInfo
	{
		get { return _hitBoxesInfo; }
		set { _hitBoxesInfo = value; }
	}

	/// <summary>Gets and Sets trailRenderers property.</summary>
	public TrailRenderer[] trailRenderers
	{
		get { return _trailRenderers; }
		set { _trailRenderers = value; }
	}

	/// <summary>Gets impactHandler Component.</summary>
	public ImpactEventHandler impactHandler
	{ 
		get
		{
			if(_impactHandler == null) _impactHandler = GetComponent<ImpactEventHandler>();
			return _impactHandler;
		}
	}
#endregion

	/// <summary>ContactWeapon's instance initialization.</summary>
	private void Awake()
	{
		SubscribeToHitCollidersEvents();
		ActivateHitBoxes(false);
		impactHandler.onImpactEvent += OnImpactEvent;
		OnAwake();
	}

	/// <summary>Callback internally invoked after Awake.</summary>
	protected virtual void OnAwake(){/*...*/}

	/// <summary>Callback invoked when ContactWeapon's instance is going to be destroyed and passed to the Garbage Collector.</summary>
	private void OnDestroy()
	{
		impactHandler.onImpactEvent -= OnImpactEvent;
	}

	/// <summary>Subscribes to HitColliders' Events.</summary>
	/// <param name="_subscribe">Subscribe? true by default.</param>
	private void SubscribeToHitCollidersEvents(bool _subscribe = true)
	{
		if(hitBoxesInfo == null) return;

		for(int i = 0; i < hitBoxesInfo.Length; i++)
		{
			HitColliderInfo hitBoxInfo = hitBoxesInfo[i];
		
			switch(_subscribe)
			{
				case true:
				hitBoxInfo.hitCollider.onTriggerEvent2D += OnHitColliderTriggerEvent2D;
				hitBoxInfo.hitCollider.ID = i;
				break;

				case false:
				hitBoxInfo.hitCollider.onTriggerEvent2D -= OnHitColliderTriggerEvent2D;
				break;
			}	
		}
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

		if(hitBoxesInfo != null)
		foreach(HitColliderInfo hitBoxInfo in hitBoxesInfo)
		{
			hitBoxInfo.hitCollider.SetTrigger(true);
			hitBoxInfo.hitCollider.Activate(_activate);
		}
	}

	/// <summary>Event invoked when this Hit Collider2D hits a GameObject.</summary>
	/// <param name="_collider">Collider2D that was involved on the Hit Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_hitColliderID">Optional ID of the HitCollider2D.</param>
	public virtual void OnHitColliderTriggerEvent2D(Collider2D _collider, HitColliderEventTypes _eventType, int _hitColliderID = 0)
	{
#region Debug:
		StringBuilder builder = new StringBuilder();

		builder.Append("OnHitColliderTriggerEvent2D invoked to class ");
		builder.AppendLine(name);
		builder.Append("Triggered with: ");
		builder.AppendLine(_collider.gameObject.name);
		builder.Append("Tag: ");
		builder.AppendLine(_collider.gameObject.tag);
		builder.Append("Layer: ");
		builder.AppendLine(_collider.gameObject.layer.ToString());
		builder.Append("Event Type: ");
		builder.AppendLine(_eventType.ToString());

		Debug.Log(builder.ToString());
#endregion

		HitColliderInfo hitBoxInfo = _hitColliderID >= 0 ? hitBoxesInfo[_hitColliderID] : null;
		int layer = _collider.gameObject.layer;
		int mask = 1 << layer;

		//Debug.Log("[ContactWeapon] Made contact with: " + _collider.name);

		if((healthAffectableMask | mask) == healthAffectableMask)
		{
			Health health = _collider.GetComponentInParent<Health>();
			
			if(health == null)
			{
				HealthLinker linker = _collider.GetComponent<HealthLinker>();
				if(linker != null) health = linker.component;
			}

			if(health != null)
			{
				Debug.Log("[ContactWeapon] " + name + " should apply damage to " + health.name);
				float damageApplied = damage * (hitBoxInfo != null ? hitBoxInfo.damageScale : 1.0f);
				health.GiveDamage(damageApplied);
			}
		}
		if((impactAffectableMask | mask) == impactAffectableMask)
		{
			ImpactEventHandler handler = _collider.GetComponentInParent<ImpactEventHandler>();
			Collider2D collider = hitBoxInfo != null ? hitBoxInfo.hitCollider.collider : GetComponent<Collider2D>();
			Trigger2DInformation info = Trigger2DInformation.CreateTriggerInformation(collider, _collider);
			Trigger2DInformation info2 = Trigger2DInformation.CreateTriggerInformation(_collider, collider);
			
			if(handler != null)
			{
				handler.InvokeImpactEvent(info2);
			}

			impactHandler.InvokeImpactEvent(info);
		}
	}

	/// <summary>Event invoked when an impact is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	public virtual void OnImpactEvent(Trigger2DInformation _info) {/*...*/}
}
}