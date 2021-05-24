using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(ImpactEventHandler))]
public class ImpactEventListener : MonoBehaviour
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

	/// <summary>Callback invoked when ImpactEventListener's instance is disabled.</summary>
	private void OnDisable()
	{
		eventHandler.onImpactEvent -= OnImpactEvent;
	}

	/// <summary>ImpactEventListener's instance initialization.</summary>
	private void Awake()
	{
		eventHandler.onImpactEvent += OnImpactEvent;		
	}

	/// <summary>Callback invoked when an impact is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	protected virtual void OnImpactEvent(Trigger2DInformation _info) { /*...*/ }
}
}