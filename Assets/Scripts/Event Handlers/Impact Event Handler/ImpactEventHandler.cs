using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
/// <summary>Event invoked when an impact is received.</summary>
/// <param name="_info">Trigger2D's Information.</param>
public delegate void OnImpactEvent(Trigger2DInformation _info);

public class ImpactEventHandler : MonoBehaviour
{
	public event OnImpactEvent onImpactEvent; 	/// <summary>OnImpactEvent's delegate.</summary>

	/// <summary>Invokes Impact's Event.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	public void InvokeImpactEvent(Trigger2DInformation _info)
	{
		if(onImpactEvent != null) onImpactEvent(_info);
	}
}
}