using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[Serializable]
public struct SelfMotionData
{
	public MotionType type; 		/// <summary>Motion's Type.</summary>
	public NormalizedVector3 axes; 	/// <summary>Displacement's Axes.</summary>
	public float radius; 			/// <summary>Motion's Radius.</summary>
	public float speed; 			/// <summary>Motion's Speed.</summary>
}
}