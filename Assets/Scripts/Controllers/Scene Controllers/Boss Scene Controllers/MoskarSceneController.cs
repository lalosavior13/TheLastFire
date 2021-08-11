using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class MoskarSceneController : Singleton<MoskarSceneController>
{
	[SerializeField] private Vector3[] waypoints; 	/// <summary>Moskar's Target Waypoints.</summary>
	private List<MoskarBoss> _moskarReproductions; 		/// <summary>Moskar's Reproductions.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color gizmosColor; 	/// <summary>Gizmos' Color.</summary>
	[SerializeField] private float gizmosRadius; 	/// <summary>Gizmos' Radius.</summary>
#endif

	/// <summary>Gets and Sets moskarReproductions property.</summary>
	public List<MoskarBoss> moskarReproductions
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
		moskarReproductions = new List<MoskarBoss>(16);
	}

	/// <returns>Random Waypoint at given index.</returns>
	public static Vector3 GetRandomWaypoint(int index)
	{
		return Instance.waypoints.Random();
	}
}
}