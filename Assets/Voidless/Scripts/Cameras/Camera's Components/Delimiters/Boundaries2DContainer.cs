using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public class Boundaries2DContainer : MonoBehaviour
{
	[SerializeField] private Space _space; 		/// <summary>Boundaries' Space Relativeness.</summary>
	[SerializeField] private Vector3 _size; 	/// <summary>Boundaries' Size.</summary>
	[SerializeField] private Vector3 _center; 	/// <summary>Boundaries' Center.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[SerializeField] private Color color; 		/// <summary>Gizmos' Color.</summary>
#endif

	/// <summary>Gets and Sets space property.</summary>
	public Space space
	{
		get { return _space; }
		set { _space = value; }
	}

	/// <summary>Gets and Sets size property.</summary>
	public Vector3 size
	{
		get { return _size; }
		set { _size = value; }
	}

	/// <summary>Gets and Sets center property.</summary>
	public Vector3 center
	{
		get { return space == Space.World ? _center : transform.position + _center; }
		set { _center = value; }
	}

	/// <summary>Gets min property.</summary>
	public Vector3 min { get { return center - (size * 0.5f); } }

	/// <summary>Gets max property.</summary>
	public Vector3 max { get { return center + (size * 0.5f); } }

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = color;

		/// Draw Boundary's Limits:
		Vector3 bottomLeftPoint = min;
		Vector3 bottomRightPoint = new Vector3(max.x, min.y, min.z);
		Vector3 topLeftPoint = new Vector3(min.x, max.y, min.z);
		Vector3 topRightPoint = max;
		
		Gizmos.DrawLine(bottomLeftPoint, bottomRightPoint);
		Gizmos.DrawLine(bottomLeftPoint, topLeftPoint);
		Gizmos.DrawLine(topLeftPoint, topRightPoint);
		Gizmos.DrawLine(bottomRightPoint, topRightPoint);
	}

	/// <summary>Resets VCamera2DBoundariesContainer's instance to its default values.</summary>
	private void Reset()
	{
		color = Color.white;
	}
#endif
}
}