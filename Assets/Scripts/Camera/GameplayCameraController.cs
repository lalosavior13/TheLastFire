using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(CameraDisplacementFollow))]
[RequireComponent(typeof(CameraTargetRetriever))]
public class GameplayCameraController : VCamera
{
	[SerializeField] private Vector3 _minLimits; 										/// <summary>Camera's Minimum Limits.</summary>
	[SerializeField] private Vector3 _maxLimits; 										/// <summary>Camera's Max Limits.</summary>
	private CameraDisplacementFollow _displacementFollow; 								/// <summary>CameraDisplacementFollow's Component.</summary>
	private CameraViewportPlane _boundariesPlane; 										/// <summary>Camera Boundaries' Plane.</summary>
	private CameraTargetRetriever _targetRetriever; 									/// <summary>CameraTargetRetriever's Component.</summary>
	private MiddlePointBetweenTransformsTargetRetriever _middlePointTargetRetriever; 	/// <summary>MiddlePointBetweenTransformsTargetRetriever's Component.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color gizmosColor; 			/// <summary>Gizmos' Color.</summary>
#endif

	/// <summary>Gets and Sets minLimits property.</summary>
	public Vector3 minLimits
	{
		get { return _minLimits; }
		set { _minLimits = value; }
	}

	/// <summary>Gets and Sets maxLimits property.</summary>
	public Vector3 maxLimits
	{
		get { return _maxLimits; }
		set { _maxLimits = value; }
	}

	/// <summary>Gets displacementFollow Component.</summary>
	public CameraDisplacementFollow displacementFollow
	{ 
		get
		{
			if(_displacementFollow == null) _displacementFollow = GetComponent<CameraDisplacementFollow>();
			return _displacementFollow;
		}
	}

	/// <summary>Gets targetRetriever Component.</summary>
	public CameraTargetRetriever targetRetriever
	{ 
		get
		{
			if(_targetRetriever == null) _targetRetriever = GetComponent<CameraTargetRetriever>();
			return _targetRetriever;
		}
	}

	/// <summary>Gets middlePointTargetRetriever Component.</summary>
	public MiddlePointBetweenTransformsTargetRetriever middlePointTargetRetriever
	{ 
		get
		{
			if(_middlePointTargetRetriever == null) _middlePointTargetRetriever = GetComponent<MiddlePointBetweenTransformsTargetRetriever>();
			return _middlePointTargetRetriever;
		}
	}

	/// <summary>Gets and Sets boundariesPlane property.</summary>
	public CameraViewportPlane boundariesPlane
	{
		get { return _boundariesPlane; }
		private set { _boundariesPlane = value; }
	}

	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
#if UNITY_EDITOR
		Gizmos.color = gizmosColor;

		/// Draw Boundary's Limits:
		Vector3 bottomLeftPoint = minLimits;
		Vector3 bottomRightPoint = new Vector3(maxLimits.x, minLimits.y, minLimits.z);
		Vector3 topLeftPoint = new Vector3(minLimits.x, maxLimits.y, minLimits.z);
		Vector3 topRightPoint = maxLimits;
		
		Gizmos.DrawLine(bottomLeftPoint, bottomRightPoint);
		Gizmos.DrawLine(bottomLeftPoint, topLeftPoint);
		Gizmos.DrawLine(topLeftPoint, topRightPoint);
		Gizmos.DrawLine(bottomRightPoint, topRightPoint);

		if(!Application.isPlaying) return;

		/// Draw Projected's Viewport Plane:
		Gizmos.DrawLine(boundariesPlane.bottomLeftPoint, boundariesPlane.bottomRightPoint);
		Gizmos.DrawLine(boundariesPlane.bottomLeftPoint, boundariesPlane.topLeftPoint);
		Gizmos.DrawLine(boundariesPlane.topLeftPoint, boundariesPlane.topRightPoint);
		Gizmos.DrawLine(boundariesPlane.bottomRightPoint, boundariesPlane.topRightPoint);
#endif
	}

/*#region UnityMethods:
	/// <summary>GameplayCameraController's instance initialization.</summary>
	private void Awake()
	{
		
	}

	/// <summary>GameplayCameraController's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		
	}
	
	/// <summary>GameplayCameraController's tick at each frame.</summary>
	private void Update ()
	{
		
	}
#endregion*/

	/// <summary>Updates Camera.</summary>
	protected override void UpdateCamera()
	{
		CameraViewportHandler.UpdateViewportPlane(camera, distance, ref _boundariesPlane);
		Vector3 target = targetRetriever.GetTarget();
		Vector3 desiredTarget = displacementFollow.GetDesiredTarget(transform.position, target);

		rigidbody.MovePosition(desiredTarget);
	}

	/// <summary>Updates Camera on Physics' Thread.</summary>
	protected override void FixedUpdateCamera()
	{
		displacementFollow.OnFixedUpdate();
	}
}
}