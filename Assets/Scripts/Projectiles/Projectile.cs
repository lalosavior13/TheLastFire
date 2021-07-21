using System.Collections;
using System;
using System.Text;
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

/*
Events:
 - Deactivated
 - Inverted
 - Impacted
 - etc.
*/


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VCameraTarget))]
[RequireComponent(typeof(ProjectileEventsHandler))]
public class Projectile : ContactWeapon
{
	public const int ID_EVENT_REPELLED = 0; 									/// <summary>Repelled's Event ID.</summary>

	public event OnDeactivated onDeactivated; 									/// <summary>OnDeactivated's Event Delegate.</summary>

	[Space(5f)]
	[Header("Projectile's Attributes:")]
	[SerializeField] private GameObjectTag[] _repelTags; 						/// <summary>Tags of GameObjects that can repel this projectile on impact.</summary>
	[SerializeField] private SpeedMode _speedMode; 								/// <summary>Speed's Mode.</summary>
	[SerializeField] private float _speed; 										/// <summary>Projectile's Speed.</summary>
	[SerializeField] private float _lifespan; 									/// <summary>Projectile's Lifespan.</summary>
	[SerializeField] private float _cooldownDuration; 							/// <summary>Cooldown's Duration.</summary>
	[SerializeField] private bool _rotateTowardsDirection; 						/// <summary>Make Projectile Rotate towards direction?.</summary>
	[SerializeField] private bool dontDeactivateOnImpact; 						/// <summary>TEMP.</summary>
	[Space(5f)]
	[Header("Particle Effects' Attributes:")]
	[SerializeField] private CollectionIndex _impactParticleEffectIndex; 		/// <summary>Index of ParticleEffect to emit when the projectile impacts.</summary>
	[SerializeField] private CollectionIndex _destroyedParticleEffectIndex; 	/// <summary>Index of ParticleEffect to emit when the projectile is destroyed.</summary>
	[Space(5f)]
	[Header("Sound Effects' Attributes:")]
	[SerializeField] private CollectionIndex _impactSoundEffectIndex; 			/// <summary>Index of Sound Effect to emit when the projectile impacts.</summary>
	[SerializeField] private CollectionIndex _destroyedSoundEffectIndex; 		/// <summary>Index of Sound Effect to emit when the projectile is destroyed.</summary>
	private bool _activated; 													/// <summary>Can the projectile be activated?.</summary>
	private float _currentLifeTime; 											/// <summary>Current Life Time.</summary>
	private Rigidbody2D _rigidbody; 											/// <summary>Rigidbody2D's Component.</summary>
	private VCameraTarget _cameraTarget; 										/// <summary>VCameraTarget's Component.</summary>
	private ProjectileEventsHandler _projectileEventsHandler; 					/// <summary>ProjectileEventsHandler's Component.</summary>
	private Vector3 _direction; 												/// <summary>Projectilwe's direction that determines its displacement.</summary>
	private Vector3 _accumulatedVelocity; 										/// <summary>Accumulated Velocity.</summary>

#region Getters/Setters:
	/// <summary>Gets type property.</summary>
	public virtual ProjectileType type { get { return ProjectileType.Normal; } }

	/// <summary>Gets and Sets repelTags property.</summary>
	public GameObjectTag[] repelTags
	{
		get { return _repelTags; }
		set { _repelTags = value; }
	}

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

	/// <summary>Gets and Sets impactParticleEffectIndex property.</summary>
	public CollectionIndex impactParticleEffectIndex
	{
		get { return _impactParticleEffectIndex; }
		set { _impactParticleEffectIndex = value; }
	}

	/// <summary>Gets and Sets destroyedParticleEffectIndex property.</summary>
	public CollectionIndex destroyedParticleEffectIndex
	{
		get { return _destroyedParticleEffectIndex; }
		set { _destroyedParticleEffectIndex = value; }
	}

	/// <summary>Gets and Sets impactSoundEffectIndex property.</summary>
	public CollectionIndex impactSoundEffectIndex
	{
		get { return _impactSoundEffectIndex; }
		set { _impactSoundEffectIndex = value; }
	}

	/// <summary>Gets and Sets destroyedSoundEffectIndex property.</summary>
	public CollectionIndex destroyedSoundEffectIndex
	{
		get { return _destroyedSoundEffectIndex; }
		set { _destroyedSoundEffectIndex = value; }
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

	/// <summary>Gets projectileEventsHandler Component.</summary>
	public ProjectileEventsHandler projectileEventsHandler
	{ 
		get
		{
			if(_projectileEventsHandler == null) _projectileEventsHandler = GetComponent<ProjectileEventsHandler>();
			return _projectileEventsHandler;
		}
	}

	/// <summary>Gets cameraTarget Component.</summary>
	public VCameraTarget cameraTarget
	{ 
		get
		{
			if(_cameraTarget == null) _cameraTarget = GetComponent<VCameraTarget>();
			return _cameraTarget;
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

	/// <summary>Updates Projectile's instance at each frame.</summary>
	protected virtual void Update()
	{
		TickLifespan();
	}

	/// <summary>Callback called each Physics' Time Step.</summary>
	protected virtual void FixedUpdate()
	{
		if(!activated) return;

		rigidbody.MovePosition(rigidbody.position + CalculateDisplacement());
	}

#region Callbacks:
	/// <summary>Event invoked when a Collision2D intersection is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	public override void OnTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0)
	{
#region Debug:
		StringBuilder builder = new StringBuilder();

		builder.Append("OnTriggerEvent invoked to class ");
		builder.Append(name);

		Debug.Log(builder.ToString());
#endregion

		base.OnTriggerEvent(_info, _eventType, _ID);

		GameObject obj = _info.collider.gameObject;

#region OutOfBoundsShiat:
		/*int layerMask = 1 << obj.layer;
		int outOfBoundsMask = Game.data.outOfBoundsLayer.ToLayerMask();
		
		/// \TODO Make an Out of Bounds Module...
		if((outOfBoundsMask | layerMask) == outOfBoundsMask)
		{
			Trigger2DInformation info = default(Trigger2DInformation);
			InvokeDeactivationEvent(DeactivationCause.LeftBoundaries, info);
		}*/
#endregion

		/// Evaluate for repelment:
		if(repelTags != null) foreach(GameObjectTag tag in repelTags)
		{
			if(obj.CompareTag(tag))
			{
				float inversion = -1.0f;

				direction *= inversion;
				accumulatedVelocity *= inversion;
				//InvokeProjectileEvent(ID_EVENT_REPELLED);
			}
		}
	}

	/// <summary>Callback internally called when there was an impact.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_ID">ID of the HitCollider2D.</param>
	protected override void OnImpact(Trigger2DInformation _info, int _ID = 0)
	{
		InvokeDeactivationEvent(DeactivationCause.Impacted, _info);
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

		} else InvokeDeactivationEvent(DeactivationCause.LifespanOver);
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
		Debug.Log("[Projectile] " + gameObject.name + " Deactivation Event. Cause: " + _cause.ToString());

		switch(_cause)
		{
			case DeactivationCause.Impacted:
			PoolManager.RequestParticleEffect(impactParticleEffectIndex, transform.position, Quaternion.identity);
			AudioController.PlayOneShot(SourceType.SFX, 0, impactSoundEffectIndex);
			break;

			case DeactivationCause.Destroyed:
			PoolManager.RequestParticleEffect(destroyedParticleEffectIndex, transform.position, Quaternion.identity);
			AudioController.PlayOneShot(SourceType.SFX, 0, destroyedSoundEffectIndex);
			break;
		}

		if(!dontDeactivateOnImpact) OnObjectDeactivation();
		projectileEventsHandler.InvokeProjectileDeactivationEvent(this, _cause, _info);
	}
}
}