using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(ImpactEventHandler))]
public abstract class ImpactEventListener : MonoBehaviour
{
	private ImpactEventHandler _eventHandler; 	/// <summary>ImpactEventHandler's Component.</summary>

	/// <summary>Gets eventHandler Component.</summary>
	public ImpactEventHandler eventHandler
	{ 
		get
		{
			if(_eventHandler == null) _eventHandler = GetComponent<ImpactEventHandler>();
			return _eventHandler;
		}
	}

	/// <summary>ImpactEventListener's instance initialization.</summary>
	private void Awake()
	{
		eventHandler.onImpactEvent += OnImpactEvent;		
	}

	/// <summary>Callback invoked when ImpactEventListener's instance is destroyed, before being passed to the Garbage Collector.</summary>
	private void OnDestroy()
	{
		eventHandler.onImpactEvent -= OnImpactEvent;
	}

	/// <summary>Callback invoked when an impact is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	protected abstract void OnImpactEvent(Trigger2DInformation _info);
}
}