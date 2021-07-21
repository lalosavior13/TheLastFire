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
	[SerializeField] private GameObjectTag[] _flamableTags; 	/// <summary>Tags of GameObjects that are considered flamable.</summary>
	private BombState _state; 									/// <summary>Current Bomb's State.</summary>
	private BouncingBall _bouncingBall; 						/// <summary>BouncingBall's Component.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets flamableTags property.</summary>
	public GameObjectTag[] flamableTags
	{
		get { return _flamableTags; }
		set { _flamableTags = value; }
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

	/// <summary>Event invoked when a Collision2D intersection is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	public virtual void OnTriggerEvent(Trigger2DInformation _info, HitColliderEventTypes _eventType, int _ID = 0)
	{
		Collider2D collider = _info.collider;
		GameObject obj = collider.gameObject;

		base.OnTriggerEvent(_info, _eventType, _ID);
	
		/*if((healthAffectableMask | layer) == healthAffectableMask)
		{
			state = BombState.Exploding;
			/*explodable.Explode(
			()=>
			{
				state = BombState.WickOff;
			});*/
		//}*/

		foreach(GameObjectTag tag in flamableTags)
		{
			if(obj.CompareTag(tag))
			{
				state = BombState.WickOn;
				break;
			}
		}
	}
}
}