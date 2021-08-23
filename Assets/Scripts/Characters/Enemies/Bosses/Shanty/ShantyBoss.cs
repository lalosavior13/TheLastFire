using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(Skeleton))]
public class ShantyBoss : Boss
{
	[SerializeField] private CollectionIndex _bombIndex; 	/// <summary>Bomb's Index.</summary>
	[SerializeField] private float _projectionTime; 		/// <summary>Bomb's Projection Time.</summary>
	[SerializeField] private ContactWeapon _sword; 			/// <summary>Shanty's Sword.</summary>
	private Skeleton _skeleton; 							/// <summary>Skeleton's Component.</summary>

	/// <summary>Gets bombIndex property.</summary>
	public CollectionIndex bombIndex { get { return _bombIndex; } }

	/// <summary>Gets projectionTime property.</summary>
	public float projectionTime { get { return _projectionTime; } }

	/// <summary>Gets sword property.</summary>
	public ContactWeapon sword { get { return _sword; } }

	/// <summary>Gets skeleton Component.</summary>
	public Skeleton skeleton
	{ 
		get
		{
			if(_skeleton == null) _skeleton = GetComponent<Skeleton>();
			return _skeleton;
		}
	}

	/// <summary>ShantyBoss's instance initialization.</summary>
	protected override void Awake()
	{
		base.Awake();
	}

	/// <summary>ShantyBoss's starting actions before 1st Update frame.</summary>
	protected override void Start ()
	{
		base.Start();
		ThrowBomb();
	}

	private void ThrowBomb()
	{
		Projectile bomb = PoolManager.RequestParabolaProjectile(Faction.Enemy, bombIndex, skeleton.rightHand.position, Game.mateo.transform.position, projectionTime);
		bomb.projectileEventsHandler.onProjectileEvent -= OnBombEvent;
		bomb.projectileEventsHandler.onProjectileEvent += OnBombEvent;
	}

	private void OnBombEvent(Projectile _projectile, int _eventID, Trigger2DInformation _info)
	{
		switch(_eventID)
		{
			case Projectile.ID_EVENT_REPELLED:
			ContactWeapon weapon = _info.collider.GetComponentInParent<ContactWeapon>();
			
			if(weapon == null) return;

			Vector3 velocity = Vector3.zero;
			float speed = 0.0f;

			/// With this branch, decide where to repel the Bomb...
			if(Game.mateo.sword == weapon)
			{ /// If Mateo hit the Bomb:
				velocity = VPhysics.ProjectileDesiredVelocity(1.0f, _projectile.transform.position, transform.position, Physics.gravity);
				speed = velocity.magnitude;
			} else if(sword == weapon)
			{ /// If Shanty hit the Bomb:
				velocity = VPhysics.ProjectileDesiredVelocity(1.0f, _projectile.transform.position, Game.mateo.transform.position, Physics.gravity);
				speed = velocity.magnitude;
			}

			_projectile.direction = velocity;
			_projectile.speed = speed;
			break;
		}
	}
}
}