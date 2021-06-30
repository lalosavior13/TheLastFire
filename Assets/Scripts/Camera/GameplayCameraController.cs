using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(VCameraDisplacementFollow))]
[RequireComponent(typeof(Boundaries2DDelimiter))]
[RequireComponent(typeof(MiddlePointBetweenTransformsTargetRetriever))]
[RequireComponent(typeof(VCamera2DBoundariesContainer))]
public class GameplayCameraController : VCamera
{
	private VCameraDisplacementFollow _displacementFollow; 								/// <summary>VCameraDisplacementFollow's Component.</summary>
	private Boundaries2DDelimiter _boundariesDelimiter; 								/// <summary>Boundaries2DDelimiter's Component.</summary>
	private MiddlePointBetweenTransformsTargetRetriever _middlePointTargetRetriever; 	/// <summary>MiddlePointBetweenTransformsTargetRetriever's Component.</summary>
	private VCamera2DBoundariesContainer _boundariesContainer; 							/// <summary>VCamera2DBoundariesContainer's Component.</summary>

#region Getters/Setters:
	/// <summary>Gets displacementFollow Component.</summary>
	public VCameraDisplacementFollow displacementFollow
	{ 
		get
		{
			if(_displacementFollow == null) _displacementFollow = GetComponent<VCameraDisplacementFollow>();
			return _displacementFollow;
		}
	}

	/// <summary>Gets boundariesDelimiter Component.</summary>
	public Boundaries2DDelimiter boundariesDelimiter
	{ 
		get
		{
			if(_boundariesDelimiter == null) _boundariesDelimiter = GetComponent<Boundaries2DDelimiter>();
			return _boundariesDelimiter;
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

	/// <summary>Gets boundariesContainer Component.</summary>
	public VCamera2DBoundariesContainer boundariesContainer
	{ 
		get
		{
			if(_boundariesContainer == null) _boundariesContainer = GetComponent<VCamera2DBoundariesContainer>();
			return _boundariesContainer;
		}
	}
#endregion

	/// <summary>Updates Camera.</summary>
	protected override void CameraUpdate()
	{
		Vector3 target = targetRetriever.GetTargetPosition();

		if(delimiters != null) foreach(VCameraDelimiter delimiter in delimiters.Values)
		{
			target = delimiter.Delimited(target);
		}

		Vector3 desiredTarget = displacementFollow.GetDesiredTarget(target);

		transform.position = desiredTarget;
	}

	/// <summary>Updates Camera on Physics' Thread.</summary>
	protected override void CameraFixedUpdate()
	{
		CameraUpdate();
	}
}
}