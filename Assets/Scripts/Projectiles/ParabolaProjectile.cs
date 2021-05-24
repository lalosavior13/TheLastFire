using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class ParabolaProjectile : Projectile
{
	/// \TODO FIX THIS (Rigidbody2D.MovePosition(Vector2) does not displace the Z's axis...)
	/// <summary>Callback internally invoked insided FixedUpdate.</summary>
	protected override void OnFixedUpdate()
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