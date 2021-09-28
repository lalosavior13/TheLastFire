using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimationEventInvoker))]
public class ShantyShip : MonoBehaviour
{
	public const int ID_ANIMATIONEVENT_SHOOTCANNONS = 0; 				/// <summary>Shoot Cannons' AnimatorEvent ID.</summary>
	public const int ID_STATE_DOCKED = 0; 								/// <summary>Docked's State ID.</summary>
	public const int ID_STATE_SAIL_0 = 1; 								/// <summary>Sail 0's State ID.</summary>
	public const int ID_STATE_CANNON = 2; 								/// <summary>Cannon's State ID.</summary>
	public const int ID_STATE_SAIL_1 = 3; 								/// <summary>Sail 1's State ID.</summary>

	[SerializeField] private AnimatorCredential _stateIDCredential; 	/// <summary>State ID's Animator Credential.</summary>
	[SerializeField] private HitCollider2D _ropeHitBox; 				/// <summary>Rope's HitBox.</summary>
	[Space(5f)]
	[SerializeField] private CollectionIndex _projectileIndex; 			/// <summary>Projectile's Index.</summary>
	[SerializeField] private Transform[] _cannons; 						/// <summary>Cannons.</summary>
	private Animator _animator; 										/// <summary>Animator's Component.</summary>
	private AnimationEventInvoker _animationEventInvoker; 				/// <summary>AnimatorEventInvoker's Component.</summary>

	/// <summary>Gets stateIDCredential property.</summary>
	public AnimatorCredential stateIDCredential { get { return _stateIDCredential; } }

	/// <summary>Gets ropeHitBox property.</summary>
	public HitCollider2D ropeHitBox { get { return _ropeHitBox; } }

	/// <summary>Gets projectileIndex property.</summary>
	public CollectionIndex projectileIndex { get { return _projectileIndex; } }

	/// <summary>Gets cannons property.</summary>
	public Transform[] cannons { get { return _cannons; } }

	/// <summary>Gets animator Component.</summary>
	public Animator animator
	{ 
		get
		{
			if(_animator == null) _animator = GetComponent<Animator>();
			return _animator;
		}
	}

	/// <summary>Gets animationEventInvoker Component.</summary>
	public AnimationEventInvoker animationEventInvoker
	{ 
		get
		{
			if(_animationEventInvoker == null) _animationEventInvoker = GetComponent<AnimationEventInvoker>();
			return _animationEventInvoker;
		}
	}

	/// <summary>Goes to given [Animator's] state.</summary>
	/// <param name="_ID">State's ID.</param>
	public void GoToState(int _ID)
	{
		animationEventInvoker.AddIntActionListener(OnAnimationIntEvent);
		animator.SetInteger(stateIDCredential, _ID);
	}

	/// <summary>Shoots Cannons.</summary>
	private void ShootCannons()
	{
		if(cannons == null) return;

		foreach(Transform cannon in cannons)
		{
			PoolManager.RequestProjectile(Faction.Enemy, projectileIndex, cannon.position, cannon.forward);
		}
	}

	/// <summary>Callback invoked when an Animation Event is invoked.</summary>
	/// <param name="_ID">Int argument.</param>
	private void OnAnimationIntEvent(int _ID)
	{
		switch(_ID)
		{
			case ID_ANIMATIONEVENT_SHOOTCANNONS:
			ShootCannons();
			break;
		}
	}
}
}