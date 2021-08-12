using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(Boundaries2DContainer))]
public class MoskarSceneController : Singleton<MoskarSceneController>
{
	[SerializeField] private Vector3[] waypoints; 			/// <summary>Moskar's Target Waypoints.</summary>
	[SerializeField] private CollectionIndex _moskarIndex; 	/// <summary>Moskar's Index.</summary>
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

	/// <summary>Gets moskarIndex property.</summary>
	public CollectionIndex moskarIndex { get { return _moskarIndex; } }

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

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		/*Gizmos.color = gizmosColor;

		foreach(Vector3 waypoint in waypoints)
		{
			Gizmos.DrawWireSphere(waypoint, gizmosRadius);
		}*/
	}
#endif

	/// <summary>Callback internally invoked after Awake.</summary>
	protected override void OnAwake()
	{
		moskarReproductions = new HashSet<MoskarBoss>();

		MoskarBoss moskar = GameObject.FindObjectOfType<MoskarBoss>();
		
		if(moskar != null)
		{
			moskarReproductions.Add(moskar);
			moskar.eventsHandler.onEnemyDeactivated += OnMoskarDeactivated;
		}

		this.StartCoroutine(MoskarReproductionsCountdown(), ref moskarReproductionsCountdown);
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

	/// <returns>Random Waypoint at given index.</returns>
	public static Vector3 GetRandomWaypoint(int index)
	{
		return Instance.waypoints.Random();
	}

	/// <summary>Event invoked when the projectile is deactivated.</summary>
	/// <param name="_enemy">Enemy that invoked the event.</param>
	/// <param name="_cause">Cause of the deactivation.</param>
	/// <param name="_info">Additional Trigger2D's information.</param>
	public void OnMoskarDeactivated(Enemy _enemy, DeactivationCause _cause, Trigger2DInformation _info)
	{
		MoskarBoss moskar = _enemy as MoskarBoss;

		if(moskar == null) return;

		if(!moskarReproductions.Remove(moskar)) return;

		moskar.eventsHandler.onEnemyDeactivated -= OnMoskarDeactivated;

		if(moskarReproductions.Count <= 0)
		{
			this.DispatchCoroutine(ref moskarReproductionsCountdown);
			Debug.Log("[MoskarSceneController] Finished!!");
		}
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