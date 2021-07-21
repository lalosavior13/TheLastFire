using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class HomingProjectile : Projectile
{
	[Space(5f)]
	[Header("Steering Attributes:")]
	[SerializeField] private float _maxSteeringForce; 			/// <summary>Maximum's Steering Force.</summary>
	[SerializeField] private float _distance; 					/// <summary>Distance from Parent's Projectile.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[SerializeField] private Color color; 						/// <summary>Gizmos' Color.</summary>
#endif
	private HomingProjectile _parentProjectile; 				/// <summary>Parent Projectile.</summary>
	private Func<Vector2> _target; 								/// <summary>Target's Position function.</summary>
	private Vector3 _lastPosition; 								/// <summary>Last Position reference [for the Steering Snake].</summary>
	private Vector2 velocity; 									/// <summary>Velocity's Vector.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets maxSteeringForce property.</summary>
	public float maxSteeringForce
	{
		get { return _maxSteeringForce; }
		set { _maxSteeringForce = value; }
	}

	/// <summary>Gets and Sets distance property.</summary>
	public float distance
	{
		get { return _distance; }
		set { _distance = value; }
	}

	/// <summary>Gets and Sets parentProjectile property.</summary>
	public HomingProjectile parentProjectile
	{
		get { return _parentProjectile; }
		set { _parentProjectile = value; }
	}

	/// <summary>Gets and Sets target property.</summary>
	public Func<Vector2> target
	{
		get { return _target; }
		set { _target = value; }
	}

	/// <summary>Gets and Sets lastPosition property.</summary>
	public Vector3 lastPosition
	{
		get { return _lastPosition; }
		set { _lastPosition = value; }
	}
#endregion

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawWireSphere(transform.position, distance);
	}
#endif

	/// <summary>Callback internally invoked inside FixedUpdate.</summary>
	protected override void FixedUpdate()
	{
		if(!activated) return;
		
		if(parentProjectile != null)
		{
			Vector2 direction = parentProjectile.rigidbody.position - rigidbody.position;

			if(direction.sqrMagnitude > (distance * distance))
			{
				//rigidbody.position = parentProjectile.rigidbody.position + (direction.normalized * distance);
				rigidbody.MovePosition(rigidbody.position + (direction.normalized * speed * Time.fixedDeltaTime));
			}
		}
		else
		{
			rigidbody.MovePosition(rigidbody.position + CalculateDisplacement());
		}
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		lastPosition = transform.position;
		velocity = Vector2.zero;
		target = null;
	}

	/// <returns>Displacement acoording to the Projectile's Type.</returns>
	protected override Vector2 CalculateDisplacement()
	{
		return target != null ?
			SteeringVehicle2D.GetSeekForce(rigidbody.position, target(), ref velocity, speed, maxSteeringForce) * Time.fixedDeltaTime : Vector2.zero;

		if(target != null) return Vector3.zero;

		Vector3 displacement = Vector3.zero;
		Vector3 steeringForce = SteeringVehicle2D.GetSeekForce(rigidbody.position, target(), ref velocity, speed, maxSteeringForce);

		switch(speedMode)
		{
			case SpeedMode.Lineal:
			displacement = steeringForce;
			break;

			case SpeedMode.Accelerating:
			accumulatedVelocity += (steeringForce * Time.fixedDeltaTime);
			displacement = accumulatedVelocity;
			break;
		}

		if(rotateTowardsDirection) transform.rotation = VQuaternion.RightLookRotation(displacement);

		return displacement * Time.fixedDeltaTime;
	}

	/// <returns>Projectile's Position.</returns>
	public Vector2 GetPosition()
	{
		return rigidbody.position;
	}
}
}