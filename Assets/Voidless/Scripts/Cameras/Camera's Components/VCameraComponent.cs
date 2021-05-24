using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
[RequireComponent(typeof(VCamera))]
public abstract class VCameraComponent : MonoBehaviour
{
	private VCamera _vCamera; 	/// <summary>vCamera's Component.</summary>
	
	/// <summary>Gets and Sets vCamera Component.</summary>
	public VCamera vCamera
	{ 
		get
		{
			if(_vCamera == null) _vCamera = GetComponent<VCamera>();
			return _vCamera;
		}
	}
}
}