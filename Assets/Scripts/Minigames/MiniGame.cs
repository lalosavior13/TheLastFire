using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
/// <summary>Event invoked when a Mini-Game Event occurs.</summary>
/// <param name="_miniGame">Mini-Game that invoked the event.</param>
/// <param name="_eventID">Event's ID.</param>
public delegate void OnMiniGameEvent(MiniGame _miniGame, int _eventID);

[Serializable]
public abstract class MiniGame
{
	public const int ID_EVENT_MINIGAME_ENDED = 0; 	/// <summary>Mini-Game's Ended Event ID.</summary>

	public event OnMiniGameEvent onMiniGameEvent; 	/// <summary>OnMiniGameEvent's Delegate.</summary>

	protected Coroutine coroutine; 					/// <summary>Coroutine's Reference.</summary>

	/// <summary>Initializes Mini-Game.</summary>
	/// <param name="_monoBehaviour">MonoBehaviour that will start the coroutine.</param>
	/// <param name="onMiniGameEvent">Optional callback invoked then the Mini-Game invokes Events.</param>
	public virtual void Initialize(MonoBehaviour _monoBehaviour, OnMiniGameEvent onMiniGameEvent = null)
	{
		if(_monoBehaviour == null) return;

		if(onMiniGameEvent != null) this.onMiniGameEvent += onMiniGameEvent; 
		_monoBehaviour.StartCoroutine(MiniGameCoroutine(), ref coroutine);
	}

	/// <summary>Terminates Mini-Game.</summary>
	/// <param name="_monoBehaviour">MonoBehaviour that will request the Coroutine's dispatchment.</param>
	public virtual void Terminate(MonoBehaviour _monoBehaviour)
	{
		_monoBehaviour.DispatchCoroutine(ref coroutine);
		onMiniGameEvent = null;
	}

	/// <summary>Mini-Game's Coroutine.</summary>
	protected virtual IEnumerator MiniGameCoroutine() { yield return null; }
}
}