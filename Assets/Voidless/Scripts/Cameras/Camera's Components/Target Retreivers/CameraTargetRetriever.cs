using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public abstract class CameraTargetRetriever : VCameraComponent
{
	/// <returns>Camera's Target.</returns>
	public abstract Vector3 GetTarget();


	/*public virtual ValueVTuple<Vector3, Vector3> GetLimits()
	{

	}*/
}
}