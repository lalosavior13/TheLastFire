using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamingo
{
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DisplacementAccumulator))]
[RequireComponent(typeof(GravityApplier))]
public class RigidbodyMovementAbility : MovementAbility
{
	private DisplacementAccumulator _accumulator; 	/// <summary>DisplacementAccumulator's Component.</summary>
	private Rigidbody2D _rigidbody; 				/// <summary>Rigidbpdy2D's Component.</summary>
	private GravityApplier _gravityApplier; 		/// <summary>GravityApplier's Component.</summary>

	/// <summary>Gets accumulator Component.</summary>
	public DisplacementAccumulator accumulator
	{ 
		get
		{
			if(_accumulator == null) _accumulator = GetComponent<DisplacementAccumulator>();
			return _accumulator;
		}
	}

	/// <summary>Gets rigidbody Component.</summary>
	public Rigidbody2D rigidbody
	{ 
		get
		{
			if(_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
			return _rigidbody;
		}
	}

	/// <summary>Gets gravityApplier Component.</summary>
	public GravityApplier gravityApplier
	{ 
		get
		{
			if(_gravityApplier == null) _gravityApplier = GetComponent<GravityApplier>();
			return _gravityApplier;
		}
	}

	/// <summary>Displaces towards given direction.</summary>
	/// <param name="direction">Movement's Direction.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	/// <param name="space">Space relativity [Space.Self by default].</param>
	public override void Move(Vector2 direction, float scale = 1.0f, Space space = Space.Self)
	{
		float scalar = gravityApplier.grounded ? 1.0f : airScalar;

		Vector2 displacement = CalculateDisplacement(direction, Time.fixedDeltaTime, scale * scalar);
		if(space == Space.Self) direction = rigidbody.rotation * direction;

		accumulator.AddDisplacement(displacement);
	}
}
}