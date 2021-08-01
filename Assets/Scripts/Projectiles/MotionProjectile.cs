using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class MotionProjectile : Projectile
{
	/// <summary>Resets MotionProjectile's instance to its default values.</summary>
	private void Reset()
	{
		axes = Vector3.up;
		space = Space.Self;
	}

	/// <summary>Callback internally invoked inside FixedUpdate.</summary>
	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if(!activated || impactEventHandler.hitBoxes == null) return;

		time = time < (360.0f * motionSpeed) ? time + (Time.fixedDeltaTime * motionSpeed) : 0.0f;

		Vector3 position = (Vector3)rigidbody.position;
		Vector3 childPosition = position;
		Vector3 motionAxis = axes.normalized;
		Vector3 motion = space == Space.Self ? Vector3.Cross(direction, Vector3.forward) : motionAxis;
		
		foreach(HitCollider2D hitBox in impactEventHandler.hitBoxes)
		{
			if(hitBox == null) continue;

			childPosition = position;

			switch(motionType)
			{
				case MotionType.Radial:
				Quaternion rotation = Quaternion.Euler(motion * time);
				childPosition += (rotation * (direction * radius));
				break;

				case MotionType.Sinusoidal:
				float sinusoidalDisplacement = Mathf.Sin(time);
				motion *= sinusoidalDisplacement;
				childPosition += (Vector3)rigidbody.position + (motion * radius);
				break;
			}

			hitBox.transform.position = childPosition;																
		}
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		time = 0.0f;
	}
}
}