using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public enum ProjectileType
{
	Normal,
	Parabola,
	Homing
}

public enum SpeedMode
{
	Lineal,
	Accelerating
}

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

/// <summary>Event invoked when the projectile is deactivated.</summary>
/// <param name="_cause">Cause of the deactivation.</param>
/// <param name="_info">Additional Trigger2D's information.</param>
public delegate void OnDeactivated(DeactivationCause _cause, Trigger2DInformation _info);

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : ContactWeapon
{
	public event OnDeactivated onDeactivated; 				/// <summary>OnDeactivated's Event Delegate.</summary>

	[Space(5f)]
	[Header("Projectile's Attributes:")]
	[SerializeField] private SpeedMode _speedMode; 			/// <summary>Speed's Mode.</summary>
	[SerializeField] private float _speed; 					/// <summary>Projectile's Speed.</summary>
	[SerializeField] private float _lifespan; 				/// <summary>Projectile's Lifespan.</summary>
	[SerializeField] private float _cooldownDuration; 		/// <summary>Cooldown's Duration.</summary>
	[SerializeField] private bool _rotateTowardsDirection; 	/// <summary>Make Projectile Rotate towards direction?.</summary>
	[SerializeField] private bool dontDeactivateOnImpact; 	/// <summary>TEMP.</summary>
	private bool _activated; 								/// <summary>Can the projectile be activated?.</summary>
	private float _currentLifeTime; 						/// <summary>Current Life Time.</summary>
	private Rigidbody2D _rigidbody; 						/// <summary>Rigidbody2D's Component.</summary>
	private Vector3 _direction; 							/// <summary>Projectilwe's direction that determines its displacement.</summary>
	private Vector3 _accumulatedVelocity; 					/// <summary>Accumulated Velocity.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets speedMode property.</summary>
	public SpeedMode speedMode
	{
		get { return _speedMode; }
		set { _speedMode = value; }
	}

	/// <summary>Gets and Sets speed property.</summary>
	public float speed
	{
		get { return _speed; }
		set { _speed = value; }
	}

	/// <summary>Gets and Sets lifespan property.</summary>
	public float lifespan
	{
		get { return _lifespan; }
		set { _lifespan = value; }
	}

	/// <summary>Gets and Sets cooldownDuration property.</summary>
	public float cooldownDuration
	{
		get { return _cooldownDuration; }
		set { _cooldownDuration = value; }
	}

	/// <summary>Gets and Sets currentLifeTime property.</summary>
	public float currentLifeTime
	{
		get { return _currentLifeTime; }
		set { _currentLifeTime = value; }
	}

	/// <summary>Gets and Sets rotateTowardsDirection property.</summary>
	public bool rotateTowardsDirection
	{
		get { return _rotateTowardsDirection; }
		set { _rotateTowardsDirection = value; }
	}

	/// <summary>Gets and Sets activated property.</summary>
	public bool activated
	{
		get { return _activated; }
		set { _activated = value; }
	}

	/// <summary>Gets rigidbody Component.</summary>
	public Rigidbody2D rigidbody
	{
		get
		{
			if(_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
			return _rigidbody;
		}
	}

	/// <summary>Gets and Sets direction property.</summary>
	public Vector3 direction
	{
		get { return _direction; }
		set
		{
			_direction = value.normalized;
			if(rotateTowardsDirection) transform.rotation = VQuaternion.RightLookRotation(_direction);
		}
	}

	/// <summary>Gets and Sets accumulatedVelocity property.</summary>
	public Vector3 accumulatedVelocity
	{
		get { return _accumulatedVelocity; }
		set { _accumulatedVelocity = value; }
	}
#endregion

#region UnityCallbacks:
	/// <summary>Updates Projectile's instance at each frame.</summary>
	private void Update()
	{
		OnUpdate();
	}

	/// <summary>Callback called each Physics' Time Step.</summary>
	private void FixedUpdate()
	{
		OnFixedUpdate();
	}

	/*/// <summary>Callback when a trigger event happens between a trigger attached to this GameObject and another one.</summary>
	/// <param name="_collider">Collider that this trigger intersected with.</param>
	private void OnTriggerEnter2D(Collider2D _collider)
	{
		GameObject obj = _collider.gameObject;
		int layerMask = 1 << obj.layer;

		if((affectable.value & layerMask) == layerMask)
		{
			Health health = obj.GetComponent<Health>();
			if(health != null) health.GiveDamage(damage);
			InvokeDeactivationEvent(DeactivationCause.Impacted, _collider);
		}
	}*/
#endregion

#region Callbacks:
	/// <summary>Callback internally invoked insided Update.</summary>
	protected virtual void OnUpdate()
	{
		TickLifespan();
	}

	/// <summary>Callback internally invoked insided FixedUpdate.</summary>
	protected virtual void OnFixedUpdate()
	{
		if(!activated) return;

		rigidbody.MovePosition(rigidbody.position + CalculateDisplacement());
	}

	/// <summary>Event invoked when this Hit Collider2D hits a GameObject.</summary>
	/// <param name="_collider">Collider2D that was involved on the Hit Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_hitColliderID">Optional ID of the HitCollider2D.</param>
	public override void OnHitColliderTriggerEvent2D(Collider2D _collider, HitColliderEventTypes _eventType, int _hitColliderID = 0)
	{
		base.OnHitColliderTriggerEvent2D(_collider, _eventType, _hitColliderID);

		GameObject obj = _collider.gameObject;
		int layerMask = 1 << obj.layer;

		/*if((healthAffectableMask | layerMask) == healthAffectableMask)
		{
			Trigger2DInformation info = Trigger2DInformation.CreateTriggerInformation(hitBoxesInfo[_hitColliderID].hitCollider.collider, _collider);
			InvokeDeactivationEvent(DeactivationCause.Impacted, info);
		}*/
 		if((impactAffectableMask | layerMask) == impactAffectableMask)
		{
			Trigger2DInformation info = Trigger2DInformation.CreateTriggerInformation(hitBoxesInfo[_hitColliderID].hitCollider.collider, _collider);
			InvokeDeactivationEvent(DeactivationCause.Impacted, info);
		}
	}

	/// <summary>Event invoked when an impact is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	public override void OnImpactEvent(Trigger2DInformation _info)
	{
		Debug.Log("[Projectile] " + gameObject.name + "  Impact Event invoked...");
		InvokeDeactivationEvent(DeactivationCause.Destroyed, _info);
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		ActivateHitBoxes();
		accumulatedVelocity = Vector3.zero;
		activated = true;
		currentLifeTime = 0.0f;
	}
#endregion

	/// <summary>Ticks Lifespan.</summary>
	protected virtual void TickLifespan()
	{
		if(!activated || lifespan <= 0.0f) return;

		if(currentLifeTime < lifespan)
		{
			currentLifeTime += Time.deltaTime;

		} else InvokeDeactivationEvent(DeactivationCause.Impacted);
	}

	/// <returns>Displacement acoording to the Projectile's Type.</returns>
	protected virtual Vector2 CalculateDisplacement()
	{
		Vector3 displacement = Vector3.zero;

		switch(speedMode)
		{
			case SpeedMode.Lineal:
			displacement = (direction * speed);
			break;

			case SpeedMode.Accelerating:
			accumulatedVelocity += (direction * speed * Time.fixedDeltaTime);
			displacement = accumulatedVelocity;
			break;
		}

		return displacement * Time.fixedDeltaTime;
	}

	/// <summary>Invokes OnDeactivation's Event and Deactivates itself [so it can be a free Pool Object resource].</summary>
	/// <param name="_cause">Cause of the Deactivation.</param>
	/// <param name="_info">Trigger2D's Information.</param>
	public virtual void InvokeDeactivationEvent(DeactivationCause _cause, Trigger2DInformation _info = default(Trigger2DInformation))
	{
		Debug.Log("[Projectile] InvokeDeactivationEvent invoked...");
		if(onDeactivated != null) onDeactivated(_cause, _info);
		if(!dontDeactivateOnImpact) OnObjectDeactivation();
	}
}
}