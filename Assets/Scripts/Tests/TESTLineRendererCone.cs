using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

[RequireComponent(typeof(LineRenderer))]
public class TESTLineRendererCone : MonoBehaviour
{
	[SerializeField] private float angle; 	/// <summary>Cone's Angle.</summary>
	[SerializeField] private float radius; 	/// <summary>Cone's Radius.</summary>
	private LineRenderer _lineRenderer; 	/// <summary>LineRenderer's Component.</summary>

	/// <summary>Gets lineRenderer Component.</summary>
	public LineRenderer lineRenderer
	{ 
		get
		{
			if(_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
			return _lineRenderer;
		}
	}
	
	/// <summary>TESTLineRendererCone's tick at each frame.</summary>
	private void Update ()
	{
		lineRenderer.DrawCone(angle, radius);	
	}
}