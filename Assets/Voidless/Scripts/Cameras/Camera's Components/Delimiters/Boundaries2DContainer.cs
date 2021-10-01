using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Voidless
{
[Serializable]
public struct Boundaries2D
{
	public Space space; 	/// <summary>Space Relativeness.</summary>
	public Vector3 size; 	/// <summary>Size.</summary>
	public Vector3 center; 	/// <summary>Center.</summary>

	/// <summary>Boundaries 2D's Constructor.</summary>
	public Boundaries2D(Vector3 _size, Vector3 _center = default(Vector3), Space _space = Space.Self)
	{
		space = _space;
		size = _size;
		center = _center;
	}

	/// <summary>Lerps Between Boundares2D A and B.</summary>
	/// <param name="a">Boundaries2D A.</param>
	/// <param name="b">Boundaries2D B.</param>
	/// <param name="t">Normalized Time t [internally clamped].</param>
	/// <returns>Interpolation between A and B.</returns>
	public static Boundaries2D Lerp(Boundaries2D a, Boundaries2D b, float t)
	{
		t = Mathf.Clamp(t, 0.0f, 1.0f);

		return new Boundaries2D(
			Vector3.Lerp(a.size, b.size, t),
			Vector3.Lerp(a.center, b.center, t),
			t == 1.0f ? b.space : a.space
		);
	}
}

public class Boundaries2DContainer : MonoBehaviour
{
	[SerializeField] private Space _space; 					/// <summary>Boundaries' Space Relativeness.</summary>
	[SerializeField] private Vector3 _size; 				/// <summary>Boundaries' Size.</summary>
	[SerializeField] private Vector3 _center; 				/// <summary>Boundaries' Center.</summary>
	[Space(5f)]
	[SerializeField] private float _interpolationDuration; 	/// <summary>Boundaries' Interpolation's Duration.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[SerializeField] private Color color; 					/// <summary>Gizmos' Color.</summary>
#endif
	private Coroutine boundaries2DInterpolation; 			/// <summary>Coroutine's reference.</summary>

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

	/// <summary>Gets and Sets interpolationDuration property.</summary>
	public float interpolationDuration
	{
		get { return _interpolationDuration; }
		set { _interpolationDuration = value; }
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

	/// <returns>Random point inside boundaries.</returns>
	public Vector3 Random()
	{
		Vector3 m = min;
		Vector3 M = max;
		
		return new Vector3
		(
			UnityEngine.Random.Range(m.x, M.x),
			UnityEngine.Random.Range(m.y, M.y),
			UnityEngine.Random.Range(m.z, M.z)
		);
	}

	/// <returns>Data to Boundaries2D.</returns>
	public Boundaries2D ToBoundaries2D()
	{
		return new Boundaries2D(size, center, space);
	}

	/// <summary>Sets Boundaries2D's data.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	public void Set(Boundaries2D b)
	{
		space = b.space;
		size = b.size;
		center = b.center;
	}

	/// <summary>Interpolates towards Boundaries2D.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	/// <param name="d">Duration.</param>
	public void InterpolateTowards(Boundaries2D b, float d)
	{
		this.StartCoroutine(InterpolateTowardsBoundaries(b, d, OnInterpolationEnds), ref boundaries2DInterpolation);
	}

	/// <summary>Interpolates towards Boundaries2D.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	public void InterpolateTowards(Boundaries2D b)
	{
		InterpolateTowards(b, interpolationDuration);
	}

	/// <summary>Callback invoked when the Boundaries2D's interpolation ends.</summary>
	private void OnInterpolationEnds()
	{
		this.DispatchCoroutine(ref boundaries2DInterpolation);
	}

	/// <summary>Interpolation's Coroutine.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	/// <param name="d">Duration.</param>
	/// <param name="onInterpolationEnds">Callback invoked when interpolation ends.</param>
	private IEnumerator InterpolateTowardsBoundaries(Boundaries2D b, float d, Action onInterpolationEnds = null)
	{
		Boundaries2D a = new Boundaries2D(size, center, space);
		float t = 0.0f;
		float iD = 1.0f / d;

		while(t < 1.0f)
		{
			Set(Boundaries2D.Lerp(a, b, t));

			t += (iD * Time.deltaTime);
			yield return null;
		}

		Set(b);
		if(onInterpolationEnds != null) onInterpolationEnds();
	}
}
}