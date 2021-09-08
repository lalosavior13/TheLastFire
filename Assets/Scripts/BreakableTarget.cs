using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(Projectile))]
[RequireComponent(typeof(SelfMotionPerformer))]
public class BreakableTarget : PoolGameObject
{
	private Projectile _projectile; 					/// <summary>Projectile's Component.</summary>
	private SelfMotionPerformer _selfMotionPerformer; 	/// <summary>SelfMotionPerformer's Component.</summary>

	/// <summary>Gets projectile Component.</summary>
	public Projectile projectile
	{ 
		get
		{
			if(_projectile == null) _projectile = GetComponent<Projectile>();
			return _projectile;
		}
	}

	/// <summary>Gets selfMotionPerformer Component.</summary>
	public SelfMotionPerformer selfMotionPerformer
	{ 
		get
		{
			if(_selfMotionPerformer == null) _selfMotionPerformer = GetComponent<SelfMotionPerformer>();
			return _selfMotionPerformer;
		}
	}
}
}