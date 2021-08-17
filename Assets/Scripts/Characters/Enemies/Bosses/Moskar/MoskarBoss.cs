using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

/*
	2 ^ 0 = 1
	2 ^ 1 = 2
	2 ^ 2 = 4
	2 ^ 3 = 8
	2 ^ 4 = 16
	Total = 31
*/

namespace Flamingo
{
[RequireComponent(typeof(SteeringVehicle2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VCameraTarget))]
public class MoskarBoss : Boss
{
	[Space(5f)]
	[Header("Moskar's Attributes:")]
	[SerializeField] private int _phases; 							/// <summary>Moskar's Phases [how many times it divides].</summary>
	[SerializeField] private FloatRange _scaleRange; 				/// <summary>Scale's Range.</summary>
	[SerializeField] private FloatRange _sphereColliderSizeRange; 	/// <summary>Size Range for the SphereCollider that acts as the HitBox.</summary>
	[Space(5f)]
	[SerializeField] private FOVSight2D _sightSensor; 				/// <summary>FOVSight2D's Component.</summary>
	[SerializeField] private Transform _tail; 						/// <summary>Moskar's Tail's Transform.</summary>
	[Space(5f)]
	[Header("Wander Attributes: ")]
	[SerializeField] private float _wanderSpeed; 					/// <summary>Wander's Max Speed.</summary>
	[SerializeField] private FloatRange _wanderInterval; 			/// <summary>Wander interval between each angle change [as a range].</summary>
	[Space(5f)]
	[Header("Evasion Attributes: ")]
	[SerializeField] private float _evasionSpeed; 					/// <summary>Evasion's Speed.</summary>
	[Space(5f)]
	[Header("Attack's Attributes:")]
	[SerializeField] private CollectionIndex _projectileIndex; 		/// <summary>Projectile's Index.</summary>
	[SerializeField] private FloatRange _shootInterval; 			/// <summary>Shooting Interval's Range.</summary>
	[SerializeField] private IntRange _fireBursts; 					/// <summary>Fire Bursts' Range.</summary>
	private int _currentPhase; 										/// <summary>Current Phase of this Moskar's Reproduction.</summary>
	private Dictionary<int, GameObject> _obstacles; 				/// <summary>Obstacles that Moskar must evade.</summary>
	private SteeringVehicle2D _vehicle; 							/// <summary>SteeringVehicle2D's Component.</summary>
	private Rigidbody2D _rigidbody; 								/// <summary>Rigidbody2D's Component.</summary>
	private VCameraTarget _cameraTarget; 							/// <summary>VCameraTarget's Component.</summary>
	private Coroutine attackCoroutine; 								/// <summary>AttackBehavior's Coroutine reference.</summary>
	Vector3[] waypoints;

#region Getters/Setters:
	/// <summary>Gets phases property.</summary>
	public int phases { get { return _phases; } }

	/// <summary>Gets scaleRange property.</summary>
	public FloatRange scaleRange { get { return _scaleRange; } }

	/// <summary>Gets sphereColliderSizeRange property.</summary>
	public FloatRange sphereColliderSizeRange { get { return _sphereColliderSizeRange; } }

	/// <summary>Gets and Sets currentPhase property.</summary>
	public int currentPhase
	{
		get { return _currentPhase; }
		set { _currentPhase = value; }
	}

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

	/// <summary>Gets cameraTarget Component.</summary>
	public VCameraTarget cameraTarget
	{ 
		get
		{
			if(_cameraTarget == null) _cameraTarget = GetComponent<VCameraTarget>();
			return _cameraTarget;
		}
	}

	/// <summary>Gets and Sets projectileIndex property.</summary>
	public CollectionIndex projectileIndex
	{
		get { return _projectileIndex; }
		set { _projectileIndex = value; }
	}

	/// <summary>Gets and Sets wanderSpeed property.</summary>
	public float wanderSpeed
	{
		get { return _wanderSpeed; }
		set { _wanderSpeed = value; }
	}

	/// <summary>Gets and Sets evasionSpeed property.</summary>
	public float evasionSpeed
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

	/// <summary>Gets and Sets fireBursts property.</summary>
	public IntRange fireBursts
	{
		get { return _fireBursts; }
		set { _fireBursts = value; }
	}

	/// <summary>Gets and Sets obstacles property.</summary>
	public Dictionary<int, GameObject> obstacles
	{
		get { return _obstacles; }
		set { _obstacles = value; }
	}
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

		obstacles = new Dictionary<int, GameObject>();

		Game.AddTargetToCamera(cameraTarget);
		sightSensor.onSightEvent += OnSightEvent;

		this.ChangeState(ID_STATE_IDLE);
	}

	/*/// <summary>Event triggered when this Collider/Rigidbody begun having contact with another Collider/Rigidbody.</summary>
	/// <param name="col">The Collision data associated with this collision Event.</param>
	private void OnCollisionEnter(Collision col)
	{
		GameObject obj = col.gameObject;
		int layer = 1 << obj.layer;
		
		if(layer == Game.data.surfaceLayer)
		{

		}
	}*/

	/// <summary>Callback invoked when the health of the character is depleted.</summary>
	protected override void OnHealthEvent(HealthEvent _event, float _amount = 0.0f)
	{
		base.OnHealthEvent(_event, _amount);

		switch(_event)
		{
			case HealthEvent.FullyDepleted:
			eventsHandler.InvokeEnemyDeactivationEvent(this, DeactivationCause.Destroyed);

			OnObjectDeactivation();
			break;
		}
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
				/*int instanceID = obj.GetInstanceID();
				if(!obstacles.ContainsKey(instanceID))obstacles.Add(instanceID, obj);*/
				break;

				case HitColliderEventTypes.Exit:
				this.RemoveStates(ID_STATE_PLAYERONSIGHT);
				break;
			}

			Debug.Log("[MinionEnemy] Player on Sight: " + _eventType.ToString());
		}
	}

	/// <summary>Callback invoked when new state's flags are added.</summary>
	/// <param name="_state">State's flags that were added.</param>
	public override void OnStatesAdded(int _state)
	{
		if((_state | ID_STATE_PLAYERONSIGHT) == _state)
		{
			vehicle.maxSpeed = evasionSpeed;
			sightSensor.gameObject.SetActive(false);
			this.StartCoroutine(ErraticFlyingBehavior(), ref behaviorCoroutine);
			this.StartCoroutine(AttackBehavior(), ref attackCoroutine);
		}
		if((_state | ID_STATE_IDLE) == _state && (_state | ID_STATE_PLAYERONSIGHT) != _state)
		{
			vehicle.maxSpeed = wanderSpeed;
			sightSensor.gameObject.SetActive(true);
			this.StartCoroutine(WanderBehaviour(), ref behaviorCoroutine);
		}
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
					rigidbody.MoveIn3D(force * Time.fixedDeltaTime);
					transform.rotation = VQuaternion.RightLookRotation(force);
					direction = wanderForce - transform.position;
				}

				yield return VCoroutines.WAIT_PHYSICS_THREAD;
			}

			wait.ChangeDurationAndReset(wanderInterval.Random());
		}
	}

	/// <summary>Performs Erratic Flying's Behavior.</summary>
	private IEnumerator ErraticFlyingBehavior()
	{
		waypoints = new Vector3[5];
		float minDistance = 0.5f * 0.5f;

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
					transform.rotation = VQuaternion.RightLookRotation(seekForce);
					direction = waypoint - transform.position;

					yield return VCoroutines.WAIT_PHYSICS_THREAD;
				}
			}
		}	

		this.AddStates(ID_STATE_IDLE);
	}

	/// <summary>Attack Behavior's Coroutine.</summary>
	private IEnumerator AttackBehavior()
	{
		//Debug.Log("[MoskarBoss] Began shitting myself...");
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

				while(shootWait.MoveNext()) yield return null;

				i++;
				yield return null;
			}
		}
	}
}
}