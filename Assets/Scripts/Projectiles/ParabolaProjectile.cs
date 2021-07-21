using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class ParabolaProjectile : Projectile
{
	/// <summary>Gets type property.</summary>
	public override ProjectileType type { get { return ProjectileType.Parabola; } }

	/// \TODO FIX THIS (Rigidbody2D.MovePosition(Vector2) does not displace the Z's axis...)
	/// <summary>Callback internally invoked inside FixedUpdate.</summary>
	protected override void FixedUpdate()
	{
		if(!activated) return;

		accumulatedVelocity += (Physics.gravity * Time.fixedDeltaTime);
		transform.position += ((direction * speed) + accumulatedVelocity) * Time.fixedDeltaTime;;
	}

	/// <returns>Displacement acoording to the Projectile's Type.</returns>
	protected override Vector2 CalculateDisplacement()
	{
		accumulatedVelocity += (Physics.gravity * Time.fixedDeltaTime);
		Debug.DrawRay(transform.position, direction * speed, Color.magenta, 3.0f);
		return ((direction * speed) + accumulatedVelocity) * Time.fixedDeltaTime;
	}
}
}