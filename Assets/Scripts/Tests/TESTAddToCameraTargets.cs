using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flamingo;
using Voidless;

[RequireComponent(typeof(VCameraTarget))]
public class TESTAddToCameraTargets : MonoBehaviour
{
	private VCameraTarget _cameraTarget; 	/// <summary>VCameraTarget's Component.</summary>

	/// <summary>Gets cameraTarget Component.</summary>
	public VCameraTarget cameraTarget
	{ 
		get
		{
			if(_cameraTarget == null) _cameraTarget = GetComponent<VCameraTarget>();
			return _cameraTarget;
		}
	}

	/// <summary>Callback invoked when TESTAddToCameraTargets's instance is enabled.</summary>
	private void OnEnable()
	{
		Game.AddTargetToCamera(cameraTarget);
	}

	/// <summary>Callback invoked when TESTAddToCameraTargets's instance is disabled.</summary>
	private void OnDisable()
	{
		if(Application.isPlaying)
		Game.RemoveTargetToCamera(cameraTarget);
	}

	/// <summary>TESTAddToCameraTargets's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		Game.AddTargetToCamera(cameraTarget);
	}
}