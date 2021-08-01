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
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color gizmosColor; 	/// <summary>Gizmos' Color.</summary>
	[SerializeField] private float gizmosRadius; 	/// <summary>Gizmos' Radius.</summary>
#endif

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = gizmosColor;

		foreach(Vector3 waypoint in waypoints)
		{
			Gizmos.DrawWireSphere(waypoint, gizmosRadius);
		}
	}
#endif

	/// <returns>Random Waypoint at given index.</returns>
	public static Vector3 GetRandomWaypoint(int index)
	{
		return Instance.waypoints.Random();
	}
}
}