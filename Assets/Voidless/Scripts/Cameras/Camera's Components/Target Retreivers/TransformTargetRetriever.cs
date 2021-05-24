using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public class TransformTargetRetriever : CameraTargetRetriever
{
	[SerializeField] private Transform _targetTransform; 	/// <summary>Target's Transform.</summary>

	/// <summary>Gets and Sets targetTransform property.</summary>
	public Transform targetTransform
	{
		get { return _targetTransform; }
		set { _targetTransform = value; }
	}

	/// <returns>Camera's Target.</returns>
	public override Vector3 GetTarget()
	{
		return targetTransform != null ? targetTransform.position : transform.position;
	}
}
}