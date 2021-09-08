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

public class BombParabolaProjectile : Projectile
{
	[Space(5f)]
	[Header("Bomb's Attributes:")]
	[SerializeField] private CollectionIndex _expodableIndex; 	/// <summary>Explodable's Index on the PoolManager.</summary>
	[SerializeField] private GameObjectTag[] _flamableTags; 	/// <summary>Tags of GameObjects that are considered flamable.</summary>
	[SerializeField] private LineRenderer _fuse; 				/// <summary>Bomb's Fuse.</summary>
	private BombState _state; 									/// <summary>Current Bomb's State.</summary>

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

	/// <summary>Gets and Sets fuse property.</summary>
	public LineRenderer fuse
	{
		get { return _fuse; }
		set { _fuse = value; }
	}
#endregion

	/// <summary>Updates BombParabolaProjectile's instance at each frame.</summary>
	protected override void Update()
	{
		base.Update();
		UpdateFuse();
	}

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

	/// <summary>Updates Fuse.</summary>
	private void UpdateFuse()
	{
		if(fuse == null) return;

		Vector3 origin = fuse.transform.position;

		fuse.SetPosition(0, origin);
		fuse.SetPosition(1, origin + fuse.transform.forward);
	}
}
}