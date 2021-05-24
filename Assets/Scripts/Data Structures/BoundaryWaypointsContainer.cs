using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

using Random = UnityEngine.Random;

namespace Flamingo
{
[Serializable]
public class BoundaryWaypointsContainer
{
	[SerializeField] private Vector3[] _ceilingWaypoints; 				/// <summary>Ceiling's Waypoints.</summary>
	[SerializeField] private Vector3[] _floorWaypoints; 				/// <summary>Floor's Waypoints.</summary>
	[SerializeField] private Vector3[] _leftWallWaypoints; 				/// <summary>Left Wall's Waypoints.</summary>
	[SerializeField] private Vector3[] _rightWallWaypoints; 			/// <summary>Right Wall's Waypoints.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[SerializeField] private Color color; 								/// <summary>Gizmos' Color.</summary>
	[SerializeField] private float radius; 								/// <summary>Waypoints' Radius.</summary>
#endif
	
	/// <summary>Gets ceilingWaypoints property.</summary>
	public Vector3[] ceilingWaypoints { get { return _ceilingWaypoints; } }

	/// <summary>Gets floorWaypoints property.</summary>
	public Vector3[] floorWaypoints { get { return _floorWaypoints; } }

	/// <summary>Gets leftWallWaypoints property.</summary>
	public Vector3[] leftWallWaypoints { get { return _leftWallWaypoints; } }

	/// <summary>Gets rightWallWaypoints property.</summary>
	public Vector3[] rightWallWaypoints { get { return _rightWallWaypoints; } }

	/// <summary>Draws Gizmos on Editor mode.</summary>
	public void OnDrawGizmos()
	{
#if UNITY_EDITOR
		Gizmos.color = color;

		if(ceilingWaypoints != null) foreach(Vector3 waypoint in ceilingWaypoints)
		{
			Gizmos.DrawWireSphere(waypoint, radius);
		}
		if(floorWaypoints != null) foreach(Vector3 waypoint in floorWaypoints)
		{
			Gizmos.DrawWireSphere(waypoint, radius);
		}
		if(leftWallWaypoints != null) foreach(Vector3 waypoint in leftWallWaypoints)
		{
			Gizmos.DrawWireSphere(waypoint, radius);
		}
		if(rightWallWaypoints != null) foreach(Vector3 waypoint in rightWallWaypoints)
		{
			Gizmos.DrawWireSphere(waypoint, radius);
		}
#endif
	}

	/// <returns>Ray that contans an origin and direction of an upcoming target.</returns>
	public Ray GetTargetOriginAndDirection()
	{
		DisplacementType orientation = (DisplacementType)UnityEngine.Random.Range(1, 4);

		switch(orientation)
		{
			case DisplacementType.Horizontal:
			return (Random.Range(0, 2) == 0) ? new Ray(leftWallWaypoints.Random(), Vector3.right) : new Ray(rightWallWaypoints.Random(), Vector3.left);
			
			case DisplacementType.Vertical:
			return (Random.Range(0, 2) == 0) ? new Ray(ceilingWaypoints.Random(), Vector3.down) : new Ray(floorWaypoints.Random(), Vector3.up);
			
			case DisplacementType.Diagonal: return GetArrowOriginAndDirection();

			default: return default(Ray);
		}
	}

	/// <returns>Ray tat contains an origin and direction of an upcoming arrow.</returns>
	public Ray GetArrowOriginAndDirection()
	{
		List<Vector3[]> waypointSets = new List<Vector3[]>(4);
		Vector3[] originWaypointSet = null;
		Vector3[] destinyWaypointSet = null;

		if(ceilingWaypoints != null && ceilingWaypoints.Length > 0)waypointSets.Add(ceilingWaypoints);
		if(floorWaypoints != null && floorWaypoints.Length > 0)waypointSets.Add(floorWaypoints);
		if(leftWallWaypoints != null && leftWallWaypoints.Length > 0)waypointSets.Add(leftWallWaypoints);
		if(rightWallWaypoints != null && rightWallWaypoints.Length > 0)waypointSets.Add(rightWallWaypoints);

		int index = Random.Range(0, waypointSets.Count);
		originWaypointSet = waypointSets[index];
		waypointSets.RemoveAt(index);

		Vector3 origin = originWaypointSet.Random();
		destinyWaypointSet = waypointSets[Random.Range(0, waypointSets.Count)];
		Vector3 destiny = destinyWaypointSet.Random();

		return new Ray(origin, destiny - origin);
	}
}
}