using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(BoxCollider2D))]
public abstract class TriggerZone<T> : MonoBehaviour where T : TriggerZone<T>
{
	protected static HashSet<T> triggerZones; 			/// <summary>TriggerZone's static registry.</summary>

	[SerializeField] private GameObjectTag[] _tags; 	/// <summary>GameObject Tags that invoke the trigger.</summary>
	private BoxCollider2D _boxCollider; 				/// <summary>BoxCollider2D's Component.</summary>

	/// <summary>Gets tags property.</summary>
	public GameObjectTag[] tags { get { return _tags; } }

	/// <summary>Gets boxCollider Component.</summary>
	public BoxCollider2D boxCollider
	{ 
		get
		{
			if(_boxCollider == null) _boxCollider = GetComponent<BoxCollider2D>();
			return _boxCollider;
		}
	}

	/// <summary>Draws Gizmos on Editor mode.</summary>
	protected virtual void OnDrawGizmos()
	{
		if(!Application.isPlaying)
		UpdateBoxCollider();
	}

	/// <summary>TriggerZone's instance initialization.</summary>
	protected virtual void Awake()
	{
		if(triggerZones == null) triggerZones = new HashSet<T>();
	}

	/// <summary>Event triggered when this Collider2D enters another Collider2D trigger.</summary>
	/// <param name="col">The other Collider2D involved in this Event.</param>
	private void OnTriggerEnter2D(Collider2D col)
	{
		if(tags == null) return;

		GameObject obj = col.gameObject;
		
		foreach(GameObjectTag tag in tags)
		{
			if(obj.CompareTag(tag))
			{
				//triggerZones.Add(this);
				OnEnter(col);
				return;
			}
		}
	}

	/// <summary>Event triggered when this Collider2D exits another Collider2D trigger.</summary>
	/// <param name="col">The other Collider2D involved in this Event.</param>
	private void OnTriggerExit2D(Collider2D col)
	{
		if(tags == null) return;

		GameObject obj = col.gameObject;
		
		foreach(GameObjectTag tag in tags)
		{
			if(obj.CompareTag(tag))
			{
				//triggerZones.Remove(this);
				OnExit(col, triggerZones.Count > 0 ? triggerZones.First() : null);
				return;
			}
		}
	}

	/// <summary>Updates BoxCollider2D.</summary>
	protected virtual void UpdateBoxCollider() { /*...*/ }

	/// <summary>Callback internally invoked when a GameObject's Collider2D enters the TriggerZone.</summary>
	/// <param name="_collider">Collider2D that Enters.</param>
	protected abstract void OnEnter(Collider2D _collider);

	/// <summary>Callback internally invoked when a GameObject's Collider2D exits the TriggerZone.</summary>
	/// <param name="_collider">Collider2D that Exits.</param>
	/// <param name="_trigger">Next Trigger that ought to be attended.</param>
	protected abstract void OnExit(Collider2D _collider, T _trigger);
}
}