using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public class MiddlePointBetweenTransformsTargetRetriever : CameraTargetRetriever
{
	private Dictionary<int, Transform> _targetTransforms; 	/// <summary>Targets' Transforms.</summary>

	/// <summary>Gets and Sets targetTransforms property.</summary>
	public Dictionary<int, Transform> targetTransforms
	{
		get { return _targetTransforms; }
		private set { _targetTransforms = value; }
	}

	/// <summary>MiddlePointBetweenTransformsTargetRetriever's instance initialization.</summary>
	private void Awake()
	{
		if(targetTransforms == null) targetTransforms = new Dictionary<int, Transform>();
	}

	/// <summary>Adds Target's Transform into the internal dictionary.</summary>
	/// <param name="_targetTransform">Target Transform to add.</param>
	public void AddTargetTransform(Transform _targetTransform)
	{
		int instanceID = _targetTransform.GetInstanceID();

		if(!targetTransforms.ContainsKey(instanceID))
		targetTransforms.Add(instanceID, _targetTransform);
	}

	/// <summary>Removes Target's Transform into the internal dictionary.</summary>
	/// <param name="_targetTransform">Target Transform to add.</param>
	public void RemoveTargetTransform(Transform _targetTransform)
	{
		int instanceID = _targetTransform.GetInstanceID();

		if(targetTransforms.ContainsKey(instanceID))
		targetTransforms.Remove(instanceID);
	}

	/// <returns>Camera's Target.</returns>
	public override Vector3 GetTarget()
	{
		Vector3 positions = Vector3.zero;
		float count = 0.0f;

		foreach(Transform targetTransform in targetTransforms.Values)
		{
			if(targetTransform == null || !targetTransform.gameObject.activeSelf) continue;

			positions += targetTransform.position;
			count++;
		}

		if(count == 0.0f) positions = transform.position;

		return count > 1.0f ? positions / count : positions;
	}
}
}