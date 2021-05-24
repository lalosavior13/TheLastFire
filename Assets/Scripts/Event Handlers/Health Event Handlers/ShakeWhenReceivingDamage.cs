using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamingo
{
public class ShakeWhenReceivingDamage : MonoBehaviour
{
	[SerializeField] private float _shakeDuration; 		/// <summary>Shake's Duration.</summary>
	[SerializeField] private float _shakeSpeed; 		/// <summary>Shake's Speed.</summary>
	[SerializeField] private float _shakeMagnitude; 	/// <summary>Shake's Magnitude.</summary>

	/// <summary>Gets and Sets shakeDuration property.</summary>
	public float shakeDuration
	{
		get { return _shakeDuration; }
		set { _shakeDuration = value; }
	}

	/// <summary>Gets and Sets shakeSpeed property.</summary>
	public float shakeSpeed
	{
		get { return _shakeSpeed; }
		set { _shakeSpeed = value; }
	}

	/// <summary>Gets and Sets shakeMagnitude property.</summary>
	public float shakeMagnitude
	{
		get { return _shakeMagnitude; }
		set { _shakeMagnitude = value; }
	}

	//public void OnHealthEvent


}
}