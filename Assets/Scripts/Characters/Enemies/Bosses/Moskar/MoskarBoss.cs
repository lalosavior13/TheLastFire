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
public class MoskarBoss : Boss
{
	[SerializeField] private FOVSight2D _sightSensor; 		/// <summary>FOVSight2D's Component.</summary>
	[SerializeField] private Transform _tail; 				/// <summary>Moskar's Tail's Transform.</summary>
	[Space(5f)]
	[Header("Wander Attributes: ")]
	[SerializeField] private FloatRange _wanderInterval; 	/// <summary>Wander interval between each angle change [as a range].</summary>
	private SteeringVehicle2D _vehicle; 					/// <summary>SteeringVehicle2D's Component.</summary>
	private Rigidbody2D _rigidbody; 						/// <summary>Rigidbody2D's Component.</summary>
	private VCameraTarget _cameraTarget; 					/// <summary>VCameraTarget's Component.</summary>
	Vector3[] waypoints;

#region Getters/Setters:
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

	/// <summary>Gets and Sets wanderInterval property.</summary>
	public FloatRange wanderInterval
	{
		get { return _wanderInterval; }
		set { _wanderInterval = value; }
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

		Game.AddTargetToCamera(cameraTarget);
		sightSensor.onSightEvent += OnSightEvent;

		this.ChangeState(ID_STATE_IDLE);
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
				Debug.Log("[MoskarBoss] Player On-Sight");
				this.AddStates(ID_STATE_PLAYERONSIGHT);
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
		if((_state | ID_STATE_IDLE) == _state)
		{
			sightSensor.gameObject.SetActive(true);
			this.StartCoroutine(WanderBehaviour(), ref behaviorCoroutine);
		}
		if((state | ID_STATE_PLAYERONSIGHT) == _state)
		{
			sightSensor.gameObject.SetActive(false);
			this.StartCoroutine(ErraticFlyingBehavior(), ref behaviorCoroutine);
		}
	}

	/// <summary>Wander's Steering Beahviour Coroutine.</summary>
	private IEnumerator WanderBehaviour()
	{
		Debug.Log("[MoskarBoss] Beginning Wandering State...");
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
		FloatRange radiusRange = new FloatRange(-15.0f, 15.0f);
		waypoints = new Vector3[5];
		float minDistance = 0.5f * 0.5f;

		for(int i = 0; i < waypoints.Length; i++)
		{
			waypoints[i] = VVector2.Random(radiusRange);
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

		//this.StartCoroutine(ErraticFlyingBehavior());
		this.AddStates(ID_STATE_IDLE);
	}
}
}