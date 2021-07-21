using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public enum ArrowProjectileState
{
	NotIntersectedWithIncrustable,
	IntersectedWithFirstIncrustable,
	Incrusted
}

/// <summary>Event invoked when the arrow is inverted.</summary>
/// <param name="_projectile">ArrowProjectile that was inverted.</param>
public delegate void OnInverted(ArrowProjectile _projectile);

[RequireComponent(typeof(LineRenderer))]
public class ArrowProjectile : Projectile
{
	public event OnInverted onInverted; 					/// <summary>OnInverted's event delegate.</summary>

	[Space(5f)]
	[Header("Arrow Projectile's Attributes:")]
	[SerializeField] private GameObjectTag[] _incrustTags; 	/// <summary>Tags of GameObjects that can be incrusted by the Arrow Projectile.</summary>
	[SerializeField] private HitCollider2D _tipHitBox; 		/// <summary>Tip's HitBox.</summary>
	[SerializeField] private CollectionIndex _chainIndex; 	/// <summary>Chain's PoolGameObject Collection Index.</summary>
	[SerializeField] private Vector3 _offset; 				/// <summary>Chain's offset from this projectile.</summary>
	private LineRenderer _lineRenderer; 					/// <summary>LineRenderer's Component.</summary>
	private Vector3 _spawnPosition; 						/// <summary>Spawn Position.</summary>
	private PoolGameObject _chain; 							/// <summary>Chain's Pool GameObject.</summary>
	private ArrowProjectileState _state; 					/// <summary>Arrow Projectile's State.</summary>
	private bool inverted;

	/// <summary>Gets and Sets incrustTags property.</summary>
	public GameObjectTag[] incrustTags
	{
		get { return _incrustTags; }
		set { _incrustTags = value; }
	}

	/// <summary>Gets tipHitBox property.</summary>
	public HitCollider2D tipHitBox { get { return _tipHitBox; } }

	/// <summary>Gets chainIndex property.</summary>
	public CollectionIndex chainIndex { get { return _chainIndex; } }

	/// <summary>Gets offset property.</summary>
	public Vector3 offset { get { return _offset; } }

	/// <summary>Gets and Sets chain property.</summary>
	public PoolGameObject chain
	{
		get { return _chain; }
		set { _chain = value; }
	}

	/// <summary>Gets and Sets state property.</summary>
	public ArrowProjectileState state
	{
		get { return _state; }
		set { _state = value; }
	}

	/// <summary>Gets lineRenderer Component.</summary>
	public LineRenderer lineRenderer
	{ 
		get
		{
			if(_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
			return _lineRenderer;
		}
	}

	/// <summary>Gets and Sets spawnPosition property.</summary>
	public Vector3 spawnPosition
	{
		get { return _spawnPosition; }
		set { _spawnPosition = value; }
	}

	/// <summary>Callback internally invoked inside Update.</summary>
	protected override void Update()
	{
		//base.Update(); /// Don't wanna tick its lifespan here...

		switch(state)
		{
			case ArrowProjectileState.NotIntersectedWithIncrustable:
			case ArrowProjectileState.IntersectedWithFirstIncrustable:
			if(chain == null) return;
			chain.transform.rotation = transform.rotation;
			break;

			case ArrowProjectileState.Incrusted:
			TickLifespan();
			break;
		}

		lineRenderer.SetPosition(0, spawnPosition);
		lineRenderer.SetPosition(1, transform.position);
	}

	/// <summary>Callback internally invoked inside FixedUpdate.</summary>
	protected override void FixedUpdate()
	{
		if(state == ArrowProjectileState.Incrusted) return;

		base.FixedUpdate();

		if(chain == null) return;

		chain.transform.position = GetOffsetedPositionForChain();
	}

	/// <summary>Event invoked when a Collision2D intersection is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	public virtual void OnTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0)
	{
#region Debug:
		StringBuilder builder = new StringBuilder();

		builder.Append("OnTriggerEvent invoked to class ");
		builder.AppendLine(name);
		builder.Append("State: ");
		builder.Append(state.ToString());

		Debug.Log(builder.ToString());
#endregion

		if(state == ArrowProjectileState.Incrusted) return;

		Collider2D collider = _info.collider;

		base.OnTriggerEvent(_info, _eventType, _ID);
		
		GameObject obj = collider.gameObject;

		if(chain != null && chain.gameObject == obj) return;

		if(incrustTags != null) foreach(GameObjectTag tag in incrustTags)
		{
			if(obj.CompareTag(tag))
			{
				switch(state)
				{
					case ArrowProjectileState.NotIntersectedWithIncrustable:
					state = ArrowProjectileState.IntersectedWithFirstIncrustable;
					break;

					case ArrowProjectileState.IntersectedWithFirstIncrustable:
					state = ArrowProjectileState.Incrusted;
					tipHitBox.SetTrigger(false);
					break;
				}

				Debug.Log("[ArrowProjectile] Interacted with uncrustable object, new state: " + state.ToString());
			}
		}
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		spawnPosition = transform.position;
		state = ArrowProjectileState.NotIntersectedWithIncrustable;
		tipHitBox.SetTrigger(true);
		lineRenderer.enabled = true;
		chain = PoolManager.RequestPoolGameObject(chainIndex, GetOffsetedPositionForChain(), transform.rotation);
		///BEGINS TEMPORAL:
		chain.gameObject.SetActive(false);
		/// ENDS TEMPORAL.
		inverted = false;
	}

	/// <summary>Callback invoked when the object is deactivated.</summary>
	public override void OnObjectDeactivation()
	{
		if(chain != null)
		{
			chain.OnObjectDeactivation();
			chain = null;
		}

		state = ArrowProjectileState.NotIntersectedWithIncrustable;
		tipHitBox.SetTrigger(true);
		lineRenderer.enabled = false;
		base.OnObjectDeactivation();
	}

	/// <returns>Chain's position relative to this projectile.</returns>
	private Vector3 GetOffsetedPositionForChain()
	{
		return transform.position + (transform.rotation * offset);
	}
}
}