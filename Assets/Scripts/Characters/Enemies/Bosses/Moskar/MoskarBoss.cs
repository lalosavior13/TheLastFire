using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(SteeringVehicle2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VCameraTarget))]
[RequireComponent(typeof(CircleCollider2D))]
public class MoskarBoss : Boss
{
	private const int ID_TAUNT_1 = 1; 											/// <summary>Taunt 1's ID.</summary>
	private const int ID_TAUNT_2 = 2; 											/// <summary>Taunt 1's ID.</summary>
	private const int ID_LOCOMOTION_IDLE = 1; 									/// <summary>Idle Locomotion's ID.</summary>
	private const int ID_LOCOMOTION_WALK = 2; 									/// <summary>Walk Locomotion's ID.</summary>
	private const int ID_LOCOMOTION_FLY = 3; 									/// <summary>Fly Locomotion's ID.</summary>
	private const int ID_LOCOMOTION_LAND = 4; 									/// <summary>Land Locomotion's ID.</summary>

	[Space(5f)]
	[Header("Moskar's Attributes:")]
	[SerializeField] private int _phases; 										/// <summary>Moskar's Phases [how many times it divides].</summary>
	[SerializeField] private FloatRange _scaleRange; 							/// <summary>Scale's Range.</summary>
	[SerializeField] private FloatRange _sphereColliderSizeRange; 				/// <summary>Size Range for the SphereCollider that acts as the HitBox.</summary>
	[SerializeField] private float _projectionTime; 							/// <summary>Moskar's Projection Time.</summary>
	[Space(5f)]
	[Header("Moskar's Components:")]
	[SerializeField] private FOVSight2D _sightSensor; 							/// <summary>FOVSight2D's Component.</summary>
	[SerializeField] private Transform _tail; 									/// <summary>Moskar's Tail's Transform.</summary>
	[Space(5f)]
	[Header("Warning's Attributes:")]
	[SerializeField] private float _warningSpeed; 								/// <summary>Warning's Steering Speed.</summary>
	[SerializeField] private float _dangerRadius; 								/// <summary>Danger's Radius.</summary>
	[SerializeField] private float _fleeDistance; 								/// <summary>Flee distance between Moskar and Mateo.</summary>
	[Space(5f)]
	[Header("Wander Attributes: ")]
	[SerializeField] private IntRange _waypointsGeneration; 					/// <summary>Waypoints generated per Wander Round.</summary>
	[SerializeField] private float _minDistanceToReachWaypoint; 				/// <summary>Minimum distance to reach Waypoint.</summary>
	[SerializeField] private FloatRange _wanderSpeed; 							/// <summary>Wander's Max Speed's Range.</summary>
	[SerializeField] private FloatRange _wanderInterval; 						/// <summary>Wander interval between each angle change [as a range].</summary>
	[Space(5f)]
	[Header("Evasion Attributes: ")]
	[SerializeField] private FloatRange _evasionSpeed; 							/// <summary>Evasion's Speed's Range.</summary>
	[Space(5f)]
	[Header("Attack's Attributes:")]
	[SerializeField] private CollectionIndex _projectileIndex; 					/// <summary>Projectile's Index.</summary>
	[SerializeField] private FloatRange _shootInterval; 						/// <summary>Shooting Interval's Range.</summary>
	[SerializeField] private IntRange _fireBursts; 								/// <summary>Fire Bursts' Range.</summary>
	[Space(5f)]
	[Header("Falling's Attributes:")]
	[SerializeField] private EulerRotation _fallingRotation; 					/// <summary>Moskar's Rotation when Falling.</summary>
	[SerializeField] private float _rotationDuration; 							/// <summary>Falling Rotation's Duration.</summary>
	[Space(5f)]
	[Header("Mateo's Serenity's Evaluation Attributes:")]
	[SerializeField] private float _maxMovementMagnitude; 						/// <summary>Maximum Movement's Magnitude.</summary>
	[SerializeField] private float _serenityDuration; 							/// <summary>Time that Mateo must have keeping its serenity for Moskar to return to its wander state.</summary>
	[Space(5f)]
	[Header("Sounds FXs:")]
	[SerializeField] private int _sourceIndex; 									/// <summary>Source Index where the SFXs are played.</summary>
	[SerializeField] private CollectionIndex _hurtSoundIndex; 					/// <summary>Hurt SFX's Index.</summary>
	[SerializeField] private CollectionIndex _fallenSoundIndex; 				/// <summary>Fallen SFX's Index.</summary>
	[Space(5f)]
	[Header("Particle Effects' Attributes:")]
	[SerializeField] private CollectionIndex _duplicateParticleEffectIndex; 	/// <summary>Duplication ParticleEffect's Index.</summary>
	[Space(5f)]
	[Header("Animator's Attributes:")]
	[SerializeField] private AnimatorCredential _vitalityIDCredential; 			/// <summary>Vitality ID's Animattor Credential.</summary>
	[SerializeField] private AnimatorCredential _tauntIDCredential; 			/// <summary>Taunt ID's Animattor Credential.</summary>
	[SerializeField] private AnimatorCredential _locomotionIDCredential; 		/// <summary>Locomotion ID's Animattor Credential.</summary>
	[SerializeField] private AnimatorCredential _attackingIDCredential; 		/// <summary>Attacking ID's Animattor Credential.</summary>
	[SerializeField] private int _attackAnimationLayer; 						/// <summary>Attack Animation's Layer.</summary>
	[SerializeField] private int _tauntAnimationLayer; 							/// <summary>Taunt Animation's Layer.</summary>
	private int _currentPhase; 													/// <summary>Current Phase of this Moskar's Reproduction.</summary>
	private float _phaseProgress; 												/// <summary>Phase's Normalized Progress.</summary>
	private SteeringVehicle2D _vehicle; 										/// <summary>SteeringVehicle2D's Component.</summary>
	private Rigidbody2D _rigidbody; 											/// <summary>Rigidbody2D's Component.</summary>
	private CircleCollider2D _hurtBox; 											/// <summary>CircleCollider2D's Component.</summary>
	private VCameraTarget _cameraTarget; 										/// <summary>VCameraTarget's Component.</summary>
	private Coroutine attackCoroutine; 											/// <summary>AttackBehavior's Coroutine reference.</summary>
	private Coroutine serenityEvaluation; 										/// <summary>Serenity's Evaluation Coroutine's reference.</summary>
	private Vector3[] waypoints; 												/// <summary>Allocated the waypoints so it can be visually debuged with Gizmos.</summary>

#region Getters/Setters:
	/// <summary>Gets fallingRotation property.</summary>
	public EulerRotation fallingRotation { get { return _fallingRotation; } }

	/// <summary>Gets phases property.</summary>
	public int phases { get { return _phases; } }

	/// <summary>Gets scaleRange property.</summary>
	public FloatRange scaleRange { get { return _scaleRange; } }

	/// <summary>Gets sphereColliderSizeRange property.</summary>
	public FloatRange sphereColliderSizeRange { get { return _sphereColliderSizeRange; } }

	/// <summary>Gets and Sets projectionTime property.</summary>
	public float projectionTime
	{
		get { return _projectionTime; }
		set { _projectionTime = value; }
	}

	/// <summary>Gets and Sets warningSpeed property.</summary>
	public float warningSpeed
	{
		get { return _warningSpeed; }
		set { _warningSpeed = value; }
	}

	/// <summary>Gets and Sets dangerRadius property.</summary>
	public float dangerRadius
	{
		get { return _dangerRadius; }
		set { _dangerRadius = value; }
	}

	/// <summary>Gets and Sets fleeDistance property.</summary>
	public float fleeDistance
	{
		get { return _fleeDistance; }
		set { _fleeDistance = value; }
	}

	/// <summary>Gets and Sets rotationDuration property.</summary>
	public float rotationDuration
	{
		get { return _rotationDuration; }
		set { _rotationDuration = value; }
	}

	/// <summary>Gets and Sets currentPhase property.</summary>
	public int currentPhase
	{
		get { return _currentPhase; }
		set { _currentPhase = value; }
	}

	/// <summary>Gets attackAnimationLayer property.</summary>
	public int attackAnimationLayer { get { return _attackAnimationLayer; } }

	/// <summary>Gets tauntAnimationLayer property.</summary>
	public int tauntAnimationLayer { get { return _tauntAnimationLayer; } }

	/// <summary>Gets sightSensor property.</summary>
	public FOVSight2D sightSensor { get { return _sightSensor; } }

	/// <summary>Gets tail property.</summary>
	public Transform tail { get { return _tail; } }

	/// <summary>Gets vehicle Component.</summary>
	public SteeringVehicle2D vehicle
	{ 
		get
		{
			if(_vehicle == null) _vehicle = GetComponent<SteeringVehicle2D>();
			return _vehicle;
		}
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

	/// <summary>Gets hurtBox Component.</summary>
	public CircleCollider2D hurtBox
	{ 
		get
		{
			if(_hurtBox == null) _hurtBox = GetComponent<CircleCollider2D>();
			return _hurtBox;
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

	/// <summary>Gets and Sets sourceIndex property.</summary>
	public int sourceIndex
	{
		get { return _sourceIndex; }
		set { _sourceIndex = value; }
	}

	/// <summary>Gets and Sets projectileIndex property.</summary>
	public CollectionIndex projectileIndex
	{
		get { return _projectileIndex; }
		set { _projectileIndex = value; }
	}

	/// <summary>Gets and Sets hurtSoundIndex property.</summary>
	public CollectionIndex hurtSoundIndex
	{
		get { return _hurtSoundIndex; }
		set { _hurtSoundIndex = value; }
	}

	/// <summary>Gets and Sets fallenSoundIndex property.</summary>
	public CollectionIndex fallenSoundIndex
	{
		get { return _fallenSoundIndex; }
		set { _fallenSoundIndex = value; }
	}

	/// <summary>Gets and Sets duplicateParticleEffectIndex property.</summary>
	public CollectionIndex duplicateParticleEffectIndex
	{
		get { return _duplicateParticleEffectIndex; }
		set { _duplicateParticleEffectIndex = value; }
	}

	/// <summary>Gets and Sets wanderSpeed property.</summary>
	public FloatRange wanderSpeed
	{
		get { return _wanderSpeed; }
		set { _wanderSpeed = value; }
	}

	/// <summary>Gets and Sets evasionSpeed property.</summary>
	public FloatRange evasionSpeed
	{
		get { return _evasionSpeed; }
		set { _evasionSpeed = value; }
	}

	/// <summary>Gets and Sets wanderInterval property.</summary>
	public FloatRange wanderInterval
	{
		get { return _wanderInterval; }
		set { _wanderInterval = value; }
	}

	/// <summary>Gets and Sets shootInterval property.</summary>
	public FloatRange shootInterval
	{
		get { return _shootInterval; }
		set { _shootInterval = value; }
	}

	/// <summary>Gets and Sets waypointsGeneration property.</summary>
	public IntRange waypointsGeneration
	{
		get { return _waypointsGeneration; }
		set { _waypointsGeneration = value; }
	}

	/// <summary>Gets and Sets fireBursts property.</summary>
	public IntRange fireBursts
	{
		get { return _fireBursts; }
		set { _fireBursts = value; }
	}

	/// <summary>Gets and Sets minDistanceToReachWaypoint property.</summary>
	public float minDistanceToReachWaypoint
	{
		get { return _minDistanceToReachWaypoint; }
		set { _minDistanceToReachWaypoint = value; }
	}

	/// <summary>Gets and Sets phaseProgress property.</summary>
	public float phaseProgress
	{
		get { return _phaseProgress; }
		set { _phaseProgress = value; }
	}

	/// <summary>Gets and Sets maxMovementMagnitude property.</summary>
	public float maxMovementMagnitude
	{
		get { return _maxMovementMagnitude; }
		set { _maxMovementMagnitude = value; }
	}

	/// <summary>Gets and Sets serenityDuration property.</summary>
	public float serenityDuration
	{
		get { return _serenityDuration; }
		set { _serenityDuration = value; }
	}

	/// <summary>Gets vitalityIDCredential property.</summary>
	public AnimatorCredential vitalityIDCredential { get { return _vitalityIDCredential; } }

	/// <summary>Gets tauntIDCredential property.</summary>
	public AnimatorCredential tauntIDCredential { get { return _tauntIDCredential; } }

	/// <summary>Gets locomotionIDCredential property.</summary>
	public AnimatorCredential locomotionIDCredential { get { return _locomotionIDCredential; } }

	/// <summary>Gets attackingIDCredential property.</summary>
	public AnimatorCredential attackingIDCredential { get { return _attackingIDCredential; } }
#endregion

	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		if(waypoints == null) return;

		foreach(Vector3 waypoint in waypoints)
		{
			Gizmos.DrawWireSphere(waypoint, 0.2f);
		}
	}

	/// <summary>MoskarBoss's instance initialization.</summary>
	protected override void Awake()
	{
		base.Awake();

		animator.SetAllLayersWeight(0.0f);
		animator.SetInteger(vitalityIDCredential, ID_STATE_ALIVE);
		animator.SetInteger(locomotionIDCredential, ID_LOCOMOTION_IDLE);
		animator.SetBool(attackingIDCredential, false);

		Game.AddTargetToCamera(cameraTarget);
		sightSensor.onSightEvent += OnSightEvent;
	}

	/// <summary>Callback invoked when scene loads, one frame before the first Update's tick.</summary>
	protected override void Start()
	{
		base.Start();

		if(currentPhase == 0)
		this.AddStates(ID_STATE_IDLE);
	}

	/*/// <summary>Event triggered when this Collider enters another Collider trigger.</summary>
	/// <param name="col">The other Collider involved in this Event.</param>
	private void OnTriggerEnter2D(Collider2D col)
	{
		GameObject obj = col.gameObject;
		
		if(obj.CompareTag(Game.data.floorTag) && (state | ID_STATE_ALIVE) != state)
		{
			Debug.Log("[MoskarBoss] TU MAMA");

		}
	}*/

	/// <summary>Callback invoked when the health of the character is depleted.</summary>
	protected override void OnHealthEvent(HealthEvent _event, float _amount = 0.0f)
	{
		switch(_event)
		{
			case HealthEvent.Depleted:
			AudioController.PlayOneShot(SourceType.SFX, sourceIndex, hurtSoundIndex);
			break;

			case HealthEvent.FullyDepleted:
			AudioController.PlayOneShot(SourceType.SFX, sourceIndex, hurtSoundIndex);
			BeginDeathRoutine();
			base.OnDeathRoutineEnds();
			this.RemoveStates(ID_STATE_ALIVE);
			this.DispatchCoroutine(ref behaviorCoroutine);
			this.DispatchCoroutine(ref attackCoroutine);
			this.DispatchCoroutine(ref serenityEvaluation);
			break;
		}
	}

	/// <summary>Callback invoked after the Death's routine ends.</summary>
	protected override void OnDeathRoutineEnds()
	{
		OnObjectDeactivation();
	}

	/// <summary>Callback invoked when this FOV Sight leaves another collider.</summary>
	/// <param name="_collider">Collider sighted.</param>
	/// <param name="_eventType">Type of interaction.</param>
	protected virtual void OnSightEvent(Collider2D _collider, HitColliderEventTypes _eventType)
	{
		GameObject obj = _collider.gameObject;

		if(obj.CompareTag(Game.data.playerTag))
		{
			switch(_eventType)
			{
				case HitColliderEventTypes.Enter:
				this.AddStates(ID_STATE_PLAYERONSIGHT);
				eventsHandler.InvokeIDEvent(ID_EVENT_PLAYERSIGHTED_BEGINS);
				break;

				case HitColliderEventTypes.Exit:
				this.RemoveStates(ID_STATE_PLAYERONSIGHT);
				//eventsHandler.InvokeIDEvent(ID_EVENT_PLAYERSIGHTED_ENDS);
				break;
			}
		}
	}

	/// <summary>Callback invoked when new state's flags are added.</summary>
	/// <param name="_state">State's flags that were added.</param>
	public override void OnStatesAdded(int _state)
	{
		if((_state | ID_STATE_IDLE) == _state)
		{ /// Wander Coroutine:
			EnterWanderState();
		
		} else if((_state | ID_STATE_PLAYERONSIGHT) == _state)
		{ /// Warning Coroutine:
			EnterAttackState();
			//EnterWarningState();

		} else if((_state | ID_STATE_ATTACK) == _state)
		{ /// Attack Coroutine:
			EnterAttackState();
		} 
	}

	/// <summary>Callback invoked when new state's flags are removed.</summary>
	/// <param name="_state">State's flags that were removed.</param>
	public override void OnStatesRemoved(int _state)
	{
		Debug.Log("[MoskarBoss] Player Not on Sight: " + ((_state | ID_STATE_PLAYERONSIGHT) == _state));
		if((_state | ID_STATE_PLAYERONSIGHT) == _state
		&& (state | ID_STATE_ATTACK) != state
		&& (state | ID_STATE_IDLE) != state
		&& sightSensor.enabled)
		{ /// If the Player got out of sight, but Moskar is not Attacking and not on Wander:
			Debug.Log("[MoskarBoss] Returning to Wander State because Mateo is out of sight.");
			EnterWanderState();
		}

		if((_state | ID_STATE_ALIVE) == _state)
		{
			Debug.Log("[MoskarBoss] Shush all behaviors");
			this.DispatchCoroutine(ref behaviorCoroutine);
			this.DispatchCoroutine(ref attackCoroutine);
			this.DispatchCoroutine(ref serenityEvaluation);
		}

		if((_state | ID_STATE_ATTACK) == _state)
		{
			animator.SetBool(attackingIDCredential, false);
			animator.SetLayerWeight(attackAnimationLayer, 0.0f);
		}
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		Game.AddTargetToCamera(cameraTarget);
		rigidbody.gravityScale = 0.0f;
		rigidbody.bodyType = RigidbodyType2D.Kinematic;
		animator.SetAllLayersWeight(0.0f);
		animator.SetInteger(vitalityIDCredential, ID_STATE_ALIVE);
		animator.SetInteger(locomotionIDCredential, ID_LOCOMOTION_IDLE);
		animator.SetBool(attackingIDCredential, false);
	}

	/// <summary>Callback invoked when the object is deactivated.</summary>
	public override void OnObjectDeactivation()
	{
		base.OnObjectDeactivation();
		Game.RemoveTargetToCamera(cameraTarget);
	}

	/// <summary>Simulates Rigidbody and resets its Velocity.</summary>
	public void SimulateInteractionsAndResetVelocity()
	{
		rigidbody.simulated = true;
		rigidbody.Sleep();
	}

	/// <summary>Enters Wander State.</summary>
	private void EnterWanderState()
	{
		Debug.Log("[MoskarBoss] Entered Wander State.");

		this.DispatchCoroutine(ref attackCoroutine);
		this.DispatchCoroutine(ref serenityEvaluation);

		vehicle.maxSpeed = wanderSpeed.Lerp(phaseProgress);
		sightSensor.gameObject.SetActive(true);
		animator.SetInteger(locomotionIDCredential, ID_LOCOMOTION_WALK);
		this.StartCoroutine(WanderBehaviour(), ref behaviorCoroutine);
	}

	/// <summary>Enters Warning State.</summary>
	private void EnterWarningState()
	{
		Debug.Log("[MoskarBoss] Entered Warning State.");

		vehicle.maxSpeed = warningSpeed;

		this.DispatchCoroutine(ref attackCoroutine);
		this.DispatchCoroutine(ref serenityEvaluation);

		//this.StartCoroutine(WarningBehavior(), ref behaviorCoroutine);
		this.StartCoroutine(WanderBehaviour(), ref behaviorCoroutine);
		//this.StartCoroutine(SerenityEvaluation(), ref serenityEvaluation);
	}

	/// <summary>Enters Attack State.</summary>
	private void EnterAttackState()
	{
		Debug.Log("[MoskarBoss] Entered Attack State.");

		vehicle.maxSpeed = evasionSpeed.Lerp(phaseProgress);
		sightSensor.gameObject.SetActive(false);
		animator.SetInteger(locomotionIDCredential, ID_LOCOMOTION_FLY);

		this.DispatchCoroutine(ref behaviorCoroutine);

		this.StartCoroutine(ErraticFlyingBehavior(), ref behaviorCoroutine);
		this.StartCoroutine(AttackBehavior(), ref attackCoroutine);
		//this.StartCoroutine(SerenityEvaluation(), ref serenityEvaluation);
	}

	/// <summary>Wander's Steering Beahviour Coroutine.</summary>
	private IEnumerator WanderBehaviour()
	{
		SecondsDelayWait wait = new SecondsDelayWait(wanderInterval.Random());
		Vector3 wanderForce = Vector3.zero;
		float minDistance = 0.5f * 0.5f;

		while(true)
		{
			wanderForce = vehicle.GetWanderForce();
			Vector3 direction = wanderForce - transform.position;
			while(wait.MoveNext())
			{
				if(direction.sqrMagnitude > minDistance)
				{
					Vector3 force = vehicle.GetSeekForce(wanderForce);

					/*if(this.HasState(ID_STATE_PLAYERONSIGHT))
					{
						Vector3 projectedMateoPosition = Game.ProjectMateoPosition(projectionTime * Time.fixedDeltaTime);
						force += (Vector3)vehicle.GetFleeForce(projectedMateoPosition);

						force *= 0.5f;
					}*/

					rigidbody.MoveIn3D(force * Time.fixedDeltaTime);
					//transform.rotation = VQuaternion.RightLookRotation(force);
					transform.rotation = VQuaternion.LookRotation(force);
					direction = wanderForce - transform.position;
				}

				yield return VCoroutines.WAIT_PHYSICS_THREAD;
			}

			wait.ChangeDurationAndReset(wanderInterval.Random());
		}
	}

	/// <summary>Warning's Behavior.</summary>
	private IEnumerator WarningBehavior()
	{
		TransformDeltaCalculator deltaCalculator = Game.mateo.deltaCalculator;
		Vector3 projectedMateoPosition = Vector3.zero;
		Vector3 direction = Vector3.zero;
		Vector3 fleeForce = Vector3.zero;
		float magnitude = 0.0f;

		while(true)
		{
			projectedMateoPosition = Game.ProjectMateoPosition(projectionTime * Time.deltaTime);
			direction = projectedMateoPosition - transform.position;
			magnitude = direction.sqrMagnitude;

			if(magnitude < (fleeDistance * fleeDistance))
			{
				fleeForce = vehicle.GetFleeForce(projectedMateoPosition);
				rigidbody.MoveIn3D(fleeForce * Time.fixedDeltaTime);
				//transform.rotation = VQuaternion.RightLookRotation(fleeForce);
				transform.rotation = VQuaternion.LookRotation(fleeForce);
			}

			if(magnitude <= (dangerRadius * dangerRadius)) this.AddStates(ID_STATE_ATTACK);

			yield return VCoroutines.WAIT_PHYSICS_THREAD;
		}
	}

	/// <summary>Performs Erratic Flying's Behavior.</summary>
	private IEnumerator ErraticFlyingBehavior()
	{
		waypoints = new Vector3[waypointsGeneration.Random()];

		float minDistance = minDistanceToReachWaypoint * minDistanceToReachWaypoint;

		while(true)
		{
			for(int i = 0; i < waypoints.Length; i++)
			{
				waypoints[i] = MoskarSceneController.Instance.moskarBoundaries.Random();
			}

			foreach(Vector3 waypoint in waypoints)
			{
				Vector3 direction = waypoint - transform.position;
				
				while(direction.sqrMagnitude > minDistance)
				{
					Vector3 seekForce = vehicle.GetSeekForce(waypoint);
					rigidbody.MoveIn3D(seekForce * Time.fixedDeltaTime);
					//transform.rotation = VQuaternion.RightLookRotation(seekForce);
					transform.rotation = VQuaternion.LookRotation(seekForce);
					direction = waypoint - transform.position;

					yield return VCoroutines.WAIT_PHYSICS_THREAD;
				}
			}
		}	

		this.AddStates(ID_STATE_IDLE);
	}

	/// <summary>Mateo Serenity's Evaluation.</summary>
	private IEnumerator SerenityEvaluation()
	{
		TransformDeltaCalculator mateoDeltaCalculator = Game.mateo.deltaCalculator;
		float serenityTime = 0.0f;
		float magnitude = 0.0f;
		float squareMagnitude = maxMovementMagnitude * maxMovementMagnitude;

		while(true)
		{
			magnitude = mateoDeltaCalculator.velocity.sqrMagnitude;

			if(magnitude <= squareMagnitude)
			{
				serenityTime += Time.deltaTime;
				if(serenityTime >= serenityDuration)
				{
					this.ChangeState(ID_STATE_ALIVE | ID_STATE_IDLE);
					yield break;
				}
			}
			else serenityTime = 0.0f;

			yield return null;
		}
	}

	/// <summary>Attack Behavior's Coroutine.</summary>
	private IEnumerator AttackBehavior()
	{
		SecondsDelayWait shootWait = new SecondsDelayWait(0.0f);
		int bursts = 0;
		int i = 0;

		while(true)
		{
			bursts = fireBursts.Random();
			shootWait.ChangeDurationAndReset(shootInterval.Random());
			i = 0;

			while(shootWait.MoveNext()) yield return null;

			while(i < bursts)
			{
				Projectile crap = PoolManager.RequestProjectile(Faction.Enemy, projectileIndex, tail.transform.position, Vector3.down);

				shootWait.ChangeDurationAndReset(crap.cooldownDuration);
				animator.SetBool(attackingIDCredential, true);
				animator.SetLayerWeight(attackAnimationLayer, 1.0f);

				while(shootWait.MoveNext()) yield return null;

				animator.SetBool(attackingIDCredential, false);
				animator.SetLayerWeight(attackAnimationLayer, 0.0f);

				i++;
				yield return null;
			}
		}
	}

	/// <summary>Death's Routine.</summary>
	/// <param name="onDeathRoutineEnds">Callback invoked when the routine ends.</param>
	protected override IEnumerator DeathRoutine(Action onDeathRoutineEnds)
	{
		animator.SetAllLayersWeight(0.0f);
		animator.SetInteger(vitalityIDCredential, ID_STATE_DEAD);

		if(currentPhase < (phases - 1) && onDeathRoutineEnds != null)
		{
			onDeathRoutineEnds();
			yield break;
		}

		float t = 0.0f;
		float inverseDuration = 1.0f / rotationDuration;
		Quaternion originalRotation = transform.rotation;
		Quaternion rotation = fallingRotation;
		rigidbody.bodyType = RigidbodyType2D.Dynamic;
		rigidbody.gravityScale = 1.0f;

		AudioController.PlayOneShot(SourceType.SFX, sourceIndex, fallenSoundIndex);

		while(t < 1.0f)
		{
			transform.rotation = Quaternion.Lerp(originalRotation, rotation, t);
			t += (Time.deltaTime * inverseDuration);
			yield return null;
		}

		while(true)
		{
			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, sphereColliderSizeRange.Lerp(phaseProgress));
			
			if(colliders != null && colliders.Length > 0) foreach(Collider2D collider in colliders)
			{
				if(collider.gameObject.CompareTag(Game.data.floorTag))
				{
					if(onDeathRoutineEnds != null) onDeathRoutineEnds();
					yield break;
				}
			}

			yield return null;
		}
	}
}
}