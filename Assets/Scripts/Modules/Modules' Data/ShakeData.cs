using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamingo
{
[Serializable]
public struct ShakeData
{
	public float duration; 		/// <summary>Shake's Duration.</summary>
	public float speed; 		/// <summary>Shake's Speed.</summary>
	public float magnitude; 	/// <summary>Shake's Magnitude.</summary>

	/// <summary>ShakeData's Constructor.</summary>
	/// <param name="_duration">Duration.</param>
	/// <param name="_speed">Speed.</param>
	/// <param name="_magnitude">Magnitude.</param>
	public ShakeData(float _duration, float _speed, float _magnitude)
	{
		duration = _duration;
		speed = _speed;
		magnitude = _magnitude;
	}

	/// <returns>String representing this Shake's Data.</returns>
	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();

		builder.Append("Shake's Data : { Duration: ");
		builder.Append(duration.ToString());
		builder.Append(", Speed: ");
		builder.Append(speed.ToString());
		builder.Append(", Magnitude: ");
		builder.Append(magnitude.ToString());
		builder.Append(" }");

		return builder.ToString();
	}
}
}