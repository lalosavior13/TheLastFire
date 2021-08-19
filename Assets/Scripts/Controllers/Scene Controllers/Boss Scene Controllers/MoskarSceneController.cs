using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Voidless;

/*
 - Calculate the total of Moskars to destroy:
 
	2 ^ 0 = 1
	2 ^ 1 = 2
	2 ^ 2 = 4
	2 ^ 3 = 8
	2 ^ 4 = 16
	Total = 31
*/

namespace Flamingo
{
[RequireComponent(typeof(Boundaries2DContainer))]
public class MoskarSceneController : Singleton<MoskarSceneController>
{
	[SerializeField] private MoskarBoss _main; 				/// <summary>Initial Moskar's Reference.</summary>
	[SerializeField] private CollectionIndex _moskarIndex; 	/// <summary>Moskar's Index.</summary>
	[SerializeField] private float _reproductionDuration; 	/// <summary>Reproduction Duration. Determines how long it lasts the reproduction's displacement and scaling.</summary>
	[SerializeField] private float _reproductionPushForce; 	/// <summary>Reproduction's Push Force.</summary>
	[SerializeField] private float _reproductionCountdown; 	/// <summary>Duration before creating another round of moskars.</summary>
	private Boundaries2DContainer _moskarBoundaries; 		/// <summary>Moskar's Boundaries.</summary>
	private HashSet<MoskarBoss> _moskarReproductions; 		/// <summary>Moskar's Reproductions.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color gizmosColor; 			/// <summary>Gizmos' Color.</summary>
	[SerializeField] private float gizmosRadius; 			/// <summary>Gizmos' Radius.</summary>
#endif
	private Coroutine moskarReproductionsCountdown; 		/// <summary>MoskarReproductionsCountdown's Coroutine reference.</summary>
	private int _totalMoskars; 								/// <summary>Total of Moskars.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets main property.</summary>
	public MoskarBoss main
	{
		get { return _main; }
		set { _main = value; }
	}

	/// <summary>Gets moskarIndex property.</summary>
	public CollectionIndex moskarIndex { get { return _moskarIndex; } }

	/// <summary>Gets and Sets reproductionDuration property.</summary>
	public float reproductionDuration
	{
		get { return _reproductionDuration; }
		set { _reproductionDuration = value; }
	}

	/// <summary>Gets and Sets reproductionPushForce property.</summary>
	public float reproductionPushForce
	{
		get { return _reproductionPushForce; }
		set { _reproductionPushForce = value; }
	}

	/// <summary>Gets and Sets reproductionCountdown property.</summary>
	public float reproductionCountdown
	{
		get { return _reproductionCountdown; }
		set { _reproductionCountdown = value; }
	}

	/// <summary>Gets moskarBoundaries Component.</summary>
	public Boundaries2DContainer moskarBoundaries
	{ 
		get
		{
			if(_moskarBoundaries == null) _moskarBoundaries = GetComponent<Boundaries2DContainer>();
			return _moskarBoundaries;
		}
	}

	/// <summary>Gets and Sets moskarReproductions property.</summary>
	public HashSet<MoskarBoss> moskarReproductions
	{
		get { return _moskarReproductions; }
		private set { _moskarReproductions = value; }
	}

	/// <summary>Gets and Sets totalMoskars property.</summary>
	public int totalMoskars
	{
		get { return _totalMoskars; }
		set { _totalMoskars = value; }
	}
#endregion

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		
	}
#endif

	/// <summary>Callback internally invoked after Awake.</summary>
	protected override void OnAwake()
	{
		moskarReproductions = new HashSet<MoskarBoss>();

// --- Begins New Implementation: ---
		if(main != null)
		{
			moskarReproductions.Add(main);
			main.eventsHandler.onEnemyDeactivated += OnMoskarDeactivated;

			totalMoskars = 0;

			for(float i = 0; i < main.phases; i++)
			{
				totalMoskars += (int)Mathf.Pow(2.0f, i);
			}
		}
// --- Ends New Implementation: ---

// --- Old Implementation: ---
		//this.StartCoroutine(MoskarReproductionsCountdown(), ref moskarReproductionsCountdown);
	}

	/// <summary>Updates MoskarSceneController's instance at each frame.</summary>
	private void Update()
	{
		Vector3 min = moskarBoundaries.min;
		Vector3 max = moskarBoundaries.max;

		/// Temporal, constraint Moskars on the boundaires.
		foreach(MoskarBoss moskar in moskarReproductions)
		{
			Vector3 position = moskar.transform.position;

			moskar.transform.position = new Vector3
			(
				Mathf.Clamp(position.x, min.x, max.x),
				Mathf.Clamp(position.y, min.y, max.y),
				position.z
			);
		}
	}

	/// <summary>Event invoked when the projectile is deactivated.</summary>
	/// <param name="_enemy">Enemy that invoked the event.</param>
	/// <param name="_cause">Cause of the deactivation.</param>
	/// <param name="_info">Additional Trigger2D's information.</param>
	public void OnMoskarDeactivated(Enemy _enemy, DeactivationCause _cause, Trigger2DInformation _info)
	{
		if(_cause != DeactivationCause.Destroyed) return;

		MoskarBoss moskar = _enemy as MoskarBoss;

		if(moskar == null) return;

		if(!moskarReproductions.Remove(moskar)) return;

		moskar.eventsHandler.onEnemyDeactivated -= OnMoskarDeactivated;

// --- Begins New Implementation: --- 
		if(moskar.currentPhase < moskar.phases)
		{
			MoskarBoss reproduction = null;
			Vector3[] forces = new Vector3[] { Vector3.left * reproductionPushForce, Vector3.right * reproductionPushForce };
			TimeConstrainedForceApplier2D[] reproductionPushes = new TimeConstrainedForceApplier2D[2];
			Vector3 scale = moskar.transform.localScale;
			int phase = moskar.currentPhase;
			float phases = 1.0f * moskar.phases;
			float t = ((1.0f * phase) / phases);
			float it = 1.0f - t;
			float sizeScale = moskar.scaleRange.Lerp(it);
			float sphereColliderSize = moskar.sphereColliderSizeRange.Lerp(it);

			phase++;

			for(int i = 0; i < 2; i++)
			{
				reproduction = PoolManager.RequestPoolGameObject(moskarIndex, moskar.transform.position, moskar.transform.rotation) as MoskarBoss;
				reproduction.eventsHandler.onEnemyDeactivated -= OnMoskarDeactivated;
				reproduction.eventsHandler.onEnemyDeactivated += OnMoskarDeactivated;
				reproduction.state = 0;
				reproduction.AddStates(Enemy.ID_STATE_IDLE);
				reproduction.currentPhase = phase;
				reproduction.health.BeginInvincibilityCooldown();
				reproduction.meshParent.localScale = scale;
				reproduction.phaseProgress = t;
				moskarReproductions.Add(reproduction);

				//reproduction.rigidbody.simulated = false;
				reproductionPushes[i] = new TimeConstrainedForceApplier2D(this, reproduction.rigidbody, forces[i], reproductionDuration, ForceMode.VelocityChange, reproduction.SimulateInteractionsAndResetVelocity);

				this.StartCoroutine(reproduction.meshParent.RegularScale(sizeScale, reproductionDuration));
				reproductionPushes[i].ApplyForce();
			}
		}

		totalMoskars--;
		Debug.Log("[MoskarSceneController] Total Moskars Remaining: " + totalMoskars);
		if(totalMoskars <= 0) Debug.Log("[MoskarSceneController] Finished!!!");

// --- Ends New Implementation ---

// --- Begins Old Implementation: ---
		/*if(moskarReproductions.Count <= 0)
		{
			this.DispatchCoroutine(ref moskarReproductionsCountdown);
			Debug.Log("[MoskarSceneController] Finished!!");
		}*/
// --- Ends Old Implementation: ---
	}

	/// <summary>Moskar Reproduction's Countdown Coroutine.</summary>
	private IEnumerator MoskarReproductionsCountdown()
	{
		SecondsDelayWait wait = new SecondsDelayWait(reproductionCountdown);

		while(true)
		{
			while(wait.MoveNext()) yield return null;
			Debug.Log("[MoskarSceneController] Should Reproduce");
			MoskarBoss firstMoskar = moskarReproductions.First();
			int remainingMoskars = moskarReproductions.Count;
			int moskarsToReproduce = 0;
			int difference = 0;
			float health = 0.0f;

			if(remainingMoskars < 2)
			{
				difference = 2 - remainingMoskars;
				health = 4.0f;

			} else if(remainingMoskars < 4)
			{
				difference = 4 - remainingMoskars;
				health = 3.0f;

			} else if(remainingMoskars < 8)
			{
				difference = 8 - remainingMoskars;
				health = 2.0f;

			} else if(remainingMoskars < 16)
			{
				difference = 1 - remainingMoskars;
				health = 1.0f;
			}

			if(difference > 0)
			{
				for(int i = 0; i < difference; i++)
				{
					MoskarBoss moskar = PoolManager.RequestPoolGameObject(moskarIndex, firstMoskar.transform.position, firstMoskar.transform.rotation) as MoskarBoss;

					if(moskar == null) yield break;

					moskar.eventsHandler.onEnemyDeactivated += OnMoskarDeactivated;
					moskar.state = 0;
					moskar.AddStates(Enemy.ID_STATE_IDLE);
					moskarReproductions.Add(moskar);
				}

				foreach(MoskarBoss moskar in moskarReproductions)
				{
					moskar.health.SetMaxHP(health, true);
				}
			}

			wait.ChangeDurationAndReset(reproductionCountdown);
		}
	}
}
}