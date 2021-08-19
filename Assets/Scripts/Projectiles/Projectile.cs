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

public enum MotionType
{
	Default,
	Radial,
	Sinusoidal
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
	[SerializeField] private ProjectileType _projectileType; 					/// <summary>Projectile's Type.</summary>
	[SerializeField] private GameObjectTag[] _repelTags; 						/// <summary>Tags of GameObjects that can repel this projectile on impact.</summary>
	[SerializeField] private SpeedMode _speedMode; 								/// <summary>Speed's Mode.</summary>
	[SerializeField] private float _speed; 										/// <summary>Projectile's Speed.</summary>
	[SerializeField] private float _lifespan; 									/// <summary>Projectile's Lifespan.</summary>
	[SerializeField] private float _cooldownDuration; 							/// <summary>Cooldown's Duration.</summary>
	[SerializeField] private bool _rotateTowardsDirection; 						/// <summary>Make Projectile Rotate towards direction?.</summary>
	[Space(5f)]
	[Header("Steering Attributes:")]
	[SerializeField] private float _maxSteeringForce; 							/// <summary>Maximum's Steering Force.</summary>
	[SerializeField] private float _distance; 									/// <summary>Distance from Parent's Projectile.</summary>
	[Space(5f)]
	[Header("Motion Projectile's Attributes:")]
	[SerializeField] private MotionType _motionType; 							/// <summary>Motion's Type.</summary>
	[SerializeField] private NormalizedVector3 _axes; 							/// <summary>Axes of motion displacement.</summary>
	[SerializeField] private Space _space; 										/// <summary>Space's Relativeness.</summary>
	[SerializeField] private float _radius; 									/// <summary>Motion's Radius.</summary>
	[SerializeField] private float _motionSpeed; 								/// <summary>Motion's Speed.</summary>
	[Space(5f)]
	[Header("Particle Effects' Attributes:")]
	[SerializeField] private CollectionIndex _impactParticleEffectIndex; 		/// <summary>Index of ParticleEffect to emit when the projectile impacts.</summary>
	[SerializeField] private CollectionIndex _destroyedParticleEffectIndex; 	/// <summary>Index of ParticleEffect to emit when the projectile is destroyed.</summary>
	[Space(5f)]
	[Header("Sound Effects' Attributes:")]
	[SerializeField] private CollectionIndex _impactSoundEffectIndex; 			/// <summary>Index of Sound Effect to emit when the projectile impacts.</summary>
	[SerializeField] private CollectionIndex _destroyedSoundEffectIndex; 		/// <summary>Index of Sound Effect to emit when the projectile is destroyed.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[SerializeField] private Color gizmosColor; 								/// <summary>Gizmos' Color.</summary>
#endif
	private bool _activated; 													/// <summary>Can the projectile be activated?.</summary>
	private float _currentLifeTime; 											/// <summary>Current Life Time.</summary>
	private float _time; 														/// <summary>Motion's Time.</summary>
	private Vector3 _lastPosition; 												/// <summary>Last Position reference [for the Steering Snake].</summary>
	private Vector3 _direction; 												/// <summary>Projectilwe's direction that determines its displacement.</summary>
	private Vector3 _accumulatedVelocity; 										/// <summary>Accumulated Velocity.</summary>
	private Func<Vector2> _target; 												/// <summary>Target's Position function.</summary>
	private Rigidbody2D _rigidbody; 											/// <summary>Rigidbody2D's Component.</summary>
	private VCameraTarget _cameraTarget; 										/// <summary>VCameraTarget's Component.</summary>
	private ProjectileEventsHandler _projectileEventsHandler; 					/// <summary>ProjectileEventsHandler's Component.</summary>
	private Projectile _parentProjectile; 										/// <summary>Parent Projectile.</summary>
	protected Vector2 velocity; 												/// <summary>Velocity's Vector.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets projectileType property.</summary>
	public ProjectileType projectileType
	{
		get { return _projectileType; }
		set { _projectileType = value; }
	}

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

	/// <summary>Gets and Sets motionType property.</summary>
	public MotionType motionType
	{
		get { return _motionType; }
		set { _motionType = value; }
	}

	/// <summary>Gets and Sets parentProjectile property.</summary>
	public Projectile parentProjectile
	{
		get { return _parentProjectile; }
		set { _parentProjectile = value; }
	}

	/// <summary>Gets and Sets space property.</summary>
	public Space space
	{
		get { return _space; }
		set { _space = value; }
	}

	/// <summary>Gets and Sets axes property.</summary>
	public NormalizedVector3 axes
	{
		get { return _axes; }
		set { _axes = value; }
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

	/// <summary>Gets and Sets maxSteeringForce property.</summary>
	public float maxSteeringForce
	{
		get { return _maxSteeringForce; }
		set { _maxSteeringForce = value; }
	}

	/// <summary>Gets and Sets distance property.</summary>
	public float distance
	{
		get { return _distance; }
		set { _distance = value; }
	}

	/// <summary>Gets and Sets currentLifeTime property.</summary>
	public float currentLifeTime
	{
		get { return _currentLifeTime; }
		set { _currentLifeTime = value; }
	}

	/// <summary>Gets and Sets radius property.</summary>
	public float radius
	{
		get { return _radius; }
		set { _radius = value; }
	}

	/// <summary>Gets and Sets motionSpeed property.</summary>
	public float motionSpeed
	{
		get { return _motionSpeed; }
		set { _motionSpeed = value; }
	}

	/// <summary>Gets and Sets time property.</summary>
	public float time
	{
		get { return _time; }
		protected set { _time = value; }
	}

	/// <summary>Gets and Sets lastPosition property.</summary>
	public Vector3 lastPosition
	{
		get { return _lastPosition; }
		set { _lastPosition = value; }
	}

	/// <summary>Gets and Sets direction property.</summary>
	public Vector3 direction
	{
		get { return _direction; }
		set
		{
			_direction = value.normalized;
			//if(rotateTowardsDirection) transform.rotation = VQuaternion.RightLookRotation(_direction);
		}
	}

	/// <summary>Gets and Sets accumulatedVelocity property.</summary>
	public Vector3 accumulatedVelocity
	{
		get { return _accumulatedVelocity; }
		set { _accumulatedVelocity = value; }
	}

	/// <summary>Gets and Sets target property.</summary>
	public Func<Vector2> target
	{
		get { return _target; }
		set { _target = value; }
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
#endregion

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = gizmosColor;

		if(parentProjectile != null) Gizmos.DrawWireSphere(transform.position, distance);
		if(motionType != MotionType.Default) Gizmos.DrawWireSphere(transform.position, radius);
		if(projectileType == ProjectileType.Homing) Gizmos.DrawRay(transform.position, velocity);
	}
#endif

	/// <summary>Resets Projectile's instance to its default values.</summary>
	protected virtual void Reset()
	{
#if UNITY_EDITOR
		gizmosColor = Color.white;
#endif
	}

	/// <summary>Updates Projectile's instance at each frame.</summary>
	protected virtual void Update()
	{
		TickLifespan();
	}

	/// <summary>Callback called each Physics' Time Step.</summary>
	protected virtual void FixedUpdate()
	{
		if(!activated) return;

		Vector3 displacement = CalculateDisplacement();

		rigidbody.MoveIn3D(displacement);
		if(rotateTowardsDirection) rigidbody.MoveRotation(VQuaternion.RightLookRotation(displacement));

		if(projectileType  == ProjectileType.Homing) HomingProjectileUpdate();
		if(motionType != MotionType.Default) MotionProjectileUpdate();
	}

#region Callbacks:
	/// <summary>Event invoked when a Collision2D intersection is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	public override void OnTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0)
	{
		base.OnTriggerEvent(_info, _eventType, _ID);

		GameObject obj = _info.collider.gameObject;

/* OutOfBoundsShiat:
		int layerMask = 1 << obj.layer;
		int outOfBoundsMask = Game.data.outOfBoundsLayer.ToLayerMask();
		
		/// \TODO Make an Out of Bounds Module...
		if((outOfBoundsMask | layerMask) == outOfBoundsMask)
		{
			Trigger2DInformation info = default(Trigger2DInformation);
			InvokeDeactivationEvent(DeactivationCause.LeftBoundaries, info);
		}
*/

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
		/*Debug.Log
		(
			"[Projectile] "
			+ gameObject.name
			+ " OnImpact Invoked with ID "
			+ _ID.ToString()
			+ ". "
			+ _info.ToString()
		);*/
		InvokeDeactivationEvent(DeactivationCause.Impacted, _info);
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		ActivateHitBoxes();
		accumulatedVelocity = Vector3.zero;
		activated = true;
		currentLifeTime = 0.0f;
		lastPosition = transform.position;
		velocity = Vector2.zero;
		target = null;
		time = 0.0f;
	}
#endregion

	/// <returns>Projectile's Position.</returns>
	public Vector2 GetPosition()
	{
		return rigidbody.position;
	}

	/// <summary>Update  Method for Homing Projectile.</summary>
	protected virtual void HomingProjectileUpdate()
	{
		if(parentProjectile != null)
		{
			Vector2 direction = parentProjectile.rigidbody.position - rigidbody.position;

			if(direction.sqrMagnitude > (distance * distance))
			rigidbody.MovePosition(rigidbody.position + (direction.normalized * speed * Time.fixedDeltaTime));
		}
	}

	/// <summary>Update  Method for Motion Projectile.</summary>
	protected virtual void MotionProjectileUpdate()
	{
		if(impactEventHandler.hitBoxes == null) return;

		time = time < (360.0f * motionSpeed) ? time + (Time.fixedDeltaTime * motionSpeed) : 0.0f;

		Vector3 position = (Vector3)rigidbody.position;
		Vector3 childPosition = position;
		Vector3 motionAxis = axes.normalized;
		Vector3 motion = space == Space.Self ? Vector3.Cross(direction, Vector3.forward) : motionAxis;
		
		foreach(HitCollider2D hitBox in impactEventHandler.hitBoxes)
		{
			if(hitBox == null) continue;

			childPosition = position;

			switch(motionType)
			{
				case MotionType.Radial:
				Quaternion rotation = Quaternion.Euler(motion * time);
				childPosition += (rotation * (direction * radius));
				break;

				case MotionType.Sinusoidal:
				float sinusoidalDisplacement = Mathf.Sin(time);
				motion *= sinusoidalDisplacement;
				childPosition += (Vector3)rigidbody.position + (motion * radius);
				break;
			}

			hitBox.transform.position = childPosition;																
		}
	}

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
	protected virtual Vector3 CalculateDisplacement()
	{
		Vector3 displacement = Vector3.zero;

		switch(projectileType)
		{
			case ProjectileType.Normal:
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
			break;

			case ProjectileType.Parabola:
			switch(speedMode)
			{
				case SpeedMode.Lineal:
				accumulatedVelocity += (Physics.gravity * Time.fixedDeltaTime);
				break;

				case SpeedMode.Accelerating:
				accumulatedVelocity = Physics.gravity;
				break;
			}

			displacement = ((direction * speed) + accumulatedVelocity);
			break;

			case ProjectileType.Homing:
			Vector3 steeringForce = target != null ? SteeringVehicle2D.GetSeekForce(rigidbody.position, target(), ref velocity, speed, maxSteeringForce) : rigidbody.position;

			switch(speedMode)
			{
				case SpeedMode.Lineal:
				displacement = steeringForce;
				break;

				case SpeedMode.Accelerating:
				accumulatedVelocity += (steeringForce * Time.fixedDeltaTime);
				displacement = accumulatedVelocity;
				break;
			}
			break;
		}

		return displacement * Time.fixedDeltaTime;
	}

	/// <summary>Invokes OnDeactivation's Event and Deactivates itself [so it can be a free Pool Object resource].</summary>
	/// <param name="_cause">Cause of the Deactivation.</param>
	/// <param name="_info">Trigger2D's Information.</param>
	public virtual void InvokeDeactivationEvent(DeactivationCause _cause, Trigger2DInformation _info = default(Trigger2DInformation))
	{
		//Debug.Log("[Projectile] " + gameObject.name + " Deactivation Event. Cause: " + _cause.ToString());

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

		OnObjectDeactivation();
		projectileEventsHandler.InvokeProjectileDeactivationEvent(this, _cause, _info);
	}
}
}