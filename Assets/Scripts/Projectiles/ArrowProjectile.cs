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

[RequireComponent(typeof(LineRenderer))]
public class ArrowProjectile : Projectile
{
	[Space(5f)]
	[Header("Arrow Projectile's Attributes:")]
	[SerializeField] private HitCollider2D _tipHitBox; 		/// <summary>Tip's HitBox.</summary>
	[SerializeField] private LayerMask _incrustMask; 		/// <summary>Incrust's Layer Mask.</summary>
	[SerializeField] private LayerMask _repelMask; 			/// <summary>Repel's LayerMask.</summary>
	[SerializeField] private CollectionIndex _chainIndex; 	/// <summary>Chain's PoolGameObject Collection Index.</summary>
	[SerializeField] private Vector3 _offset; 				/// <summary>Chain's offset from this projectile.</summary>
	private LineRenderer _lineRenderer; 					/// <summary>LineRenderer's Component.</summary>
	private Vector3 _spawnPosition; 						/// <summary>Spawn Position.</summary>
	private PoolGameObject _chain; 							/// <summary>Chain's Pool GameObject.</summary>
	private ArrowProjectileState _state; 					/// <summary>Arrow Projectile's State.</summary>
	private bool inverted;

	/// <summary>Gets tipHitBox property.</summary>
	public HitCollider2D tipHitBox { get { return _tipHitBox; } }

	/// <summary>Gets incrustMask property.</summary>
	public LayerMask incrustMask { get { return _incrustMask; } }

	/// <summary>Gets repelMask property.</summary>
	public LayerMask repelMask { get { return _repelMask; } }

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

	/// <summary>Updates ArrowProjectile's instance at each frame.</summary>
	private void Update()
	{
		OnUpdate();
	}

	/// <summary>Callback internally invoked insided Update.</summary>
	protected override void OnUpdate()
	{
		base.OnUpdate();

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

	/// <summary>Callback internally invoked insided FixedUpdate.</summary>
	protected override void OnFixedUpdate()
	{
		if(state == ArrowProjectileState.Incrusted) return;

		base.OnFixedUpdate();

		if(chain == null) return;

		chain.transform.position = GetOffsetedPositionForChain();
	}

	/// <summary>Event invoked when an impact is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	public override void OnImpactEvent(Trigger2DInformation _info)
	{
		int layer = (1 << _info.collider.gameObject.layer);

		if((repelMask | layer) == repelMask)
		{
			if(inverted)
			{
				//Debug.Log("[ArrowProjectile] It was already inverted");
				return;
			}

			//Debug.Log("[ArrowProjectile] I ought to repel..." + GetInstanceID());
			direction *= -1.0f;
			accumulatedVelocity *= - 1.0f;
			tipHitBox.Activate(false);
			state = ArrowProjectileState.NotIntersectedWithIncrustable;
			inverted = true;
		}
		else
		{
			//Debug.Log("[ArrowProjectile] Not on Repel Layer...");
			base.OnImpactEvent(_info);
		}
	}

	/// <summary>Event invoked when this Hit Collider2D hits a GameObject.</summary>
	/// <param name="_collider">Collider2D that was involved on the Hit Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_hitColliderID">Optional ID of the HitCollider2D.</param>
	public override void OnHitColliderTriggerEvent2D(Collider2D _collider, HitColliderEventTypes _eventType, int _hitColliderID = 0)
	{
#region Debug:
		StringBuilder builder = new StringBuilder();

		builder.Append("OnHitColliderTriggerEvent2D invoked to class ");
		builder.AppendLine(name);
		builder.Append("State: ");
		builder.Append(state.ToString());

		Debug.Log(builder.ToString());
#endregion

		if(state == ArrowProjectileState.Incrusted) return;

		base.OnHitColliderTriggerEvent2D(_collider, _eventType, _hitColliderID);
		
		GameObject obj = _collider.gameObject;

		if(chain != null && chain.gameObject == obj) return;

		int layerMask = 1 << obj.layer;

		if((incrustMask | layerMask) == incrustMask)
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

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		spawnPosition = transform.position;
		state = ArrowProjectileState.NotIntersectedWithIncrustable;
		tipHitBox.SetTrigger(true);
		chain = PoolManager.RequestPoolGameObject(chainIndex, GetOffsetedPositionForChain(), transform.rotation);
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
		base.OnObjectDeactivation();
	}

	/// <returns>Chain's position relative to this projectile.</returns>
	private Vector3 GetOffsetedPositionForChain()
	{
		return transform.position + (transform.rotation * offset);
	}
}
}