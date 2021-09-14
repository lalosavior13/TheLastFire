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
	public const int ID_EVENT_MINIGAME_ENDED = 0; 					/// <summary>Mini-Game's Ended Event ID.</summary>
	public const int ID_EVENT_MINIGAME_SCOREUPDATE_LOCAL = 1;       /// <summary>Local Score's Update Event ID.</summary>
    public const int ID_EVENT_MINIGAME_SCOREUPDATE_VISITOR = 2;     /// <summary>Visitor Score's Update Event ID.</summary>

	public event OnMiniGameEvent onMiniGameEvent; 					/// <summary>OnMiniGameEvent's Delegate.</summary>

	private bool _running; 											/// <summary>Is the Mini-Game Running?.</summary>
	protected Coroutine coroutine; 									/// <summary>Coroutine's Reference.</summary>

	/// <summary>Gets and Sets running property.</summary>
	public bool running
	{
		get { return _running; }
		protected set { _running = value; }
	}

	/// <summary>Initializes Mini-Game.</summary>
	/// <param name="_monoBehaviour">MonoBehaviour that will start the coroutine.</param>
	/// <param name="onMiniGameEvent">Optional callback invoked then the Mini-Game invokes Events.</param>
	public virtual void Initialize(MonoBehaviour _monoBehaviour, OnMiniGameEvent onMiniGameEvent = null)
	{
		if(_monoBehaviour == null) return;

		if(onMiniGameEvent != null) this.onMiniGameEvent += onMiniGameEvent; 
		_monoBehaviour.StartCoroutine(MiniGameCoroutine(), ref coroutine);
		running = true;
	}

	/// <summary>Terminates Mini-Game.</summary>
	/// <param name="_monoBehaviour">MonoBehaviour that will request the Coroutine's dispatchment.</param>
	public virtual void Terminate(MonoBehaviour _monoBehaviour)
	{
		_monoBehaviour.DispatchCoroutine(ref coroutine);
		onMiniGameEvent = null;
		running = false;
	}

	/// <summary>Invokes Event.</summary>
	/// <param name="_ID">Event's ID.</param>
	protected void InvokeEvent(int _ID)
	{
		if(onMiniGameEvent != null) onMiniGameEvent(this, _ID);
	}

	/// <summary>Mini-Game's Coroutine.</summary>
	protected virtual IEnumerator MiniGameCoroutine() { yield return null; }
}
}