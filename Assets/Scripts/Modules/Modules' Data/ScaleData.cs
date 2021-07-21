using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamingo
{
[Serializable]
public struct ScaleData
{
	public Vector3 scale; 	/// <summary>Destiny Scale.</summary>
	public float duration; 	/// <summary>Scale's Duration.</summary>

	/// <summary>ScaleData's Constructor.</summary>
	/// <param name="_scale">Scale.</param>
	/// <param name="_duration">Duration.</param>
	public ScaleData(Vector3 _scale, float _duration)
	{
		scale = _scale;
		duration = _duration;
	}

	/// <returns>String representing this Scale's Data.</returns>
	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();

		builder.Append("Scale's Data : { Scale: ");
		builder.Append(scale.ToString());
		builder.Append(", Duration: ");
		builder.Append(duration.ToString());
		builder.Append(" }");

		return builder.ToString();
	}
}
}