using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public enum BombState
{
	WickOff,
	WickOn,
	Exploding
}

[RequireComponent(typeof(BouncingBall))]
public class BombParabolaProjectile : ParabolaProjectile
{
	[Space(5f)]
	[Header("Bomb's Attributes:")]
	[SerializeField] private CollectionIndex _expodableIndex; 	/// <summary>Explodable's Index on the PoolManager.</summary>
	[SerializeField] private LayerMask _flamableMask; 			/// <summary>Mask that contains GameObjects considered Flamable.</summary>
	private BombState _state; 									/// <summary>Current Bomb's State.</summary>
	private BouncingBall _bouncingBall; 						/// <summary>BouncingBall's Component.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets flamableMask property.</summary>
	public LayerMask flamableMask
	{
		get { return _flamableMask; }
		set { _flamableMask = value; }
	}

	/// <summary>Gets and Sets state property.</summary>
	public BombState state
	{
		get { return _state; }
		set { _state = value; }
	}

	/// <summary>Gets bouncingBall Component.</summary>
	public BouncingBall bouncingBall
	{ 
		get
		{
			if(_bouncingBall == null) _bouncingBall = GetComponent<BouncingBall>();
			return _bouncingBall;
		}
	}
#endregion

	/// <summary>Callback invoked when the object is deactivated.</summary>
	public override void OnObjectDeactivation()
	{
		base.OnObjectDeactivation();
		rigidbody.Sleep();
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		state = BombState.WickOff;
	}

	/// <summary>Event invoked when this Hit Collider2D hits a GameObject.</summary>
	/// <param name="_collider">Collider2D that was involved on the Hit Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_hitColliderID">Optional ID of the HitCollider2D.</param>
	public override void OnHitColliderTriggerEvent2D(Collider2D _collider, HitColliderEventTypes _eventType, int _hitColliderID = 0)
	{
		GameObject obj = _collider.gameObject;
		int layer = 1 << obj.layer;

		if((flamableMask | layer) == flamableMask && state == BombState.WickOff)
		{
			state = BombState.WickOn;
		}

		base.OnHitColliderTriggerEvent2D(_collider, _eventType, _hitColliderID);
	
		if((healthAffectableMask | layer) == healthAffectableMask)
		{
			state = BombState.Exploding;
			/*explodable.Explode(
			()=>
			{
				state = BombState.WickOff;
			});*/
		}
	}
}
}