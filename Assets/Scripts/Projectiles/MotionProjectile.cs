using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public enum MotionType
{
	Radial,
	Sinusoidal
}

public class MotionProjectile : Projectile
{
	[Space(5f)]
	[Header("Motion Projectile's Attributes:")]
	[SerializeField] private MotionType _type; 				/// <summary>Motion's Type.</summary>
	[SerializeField] private NormalizedVector3 _axes; 		/// <summary>Axes of motion displacement.</summary>
	[SerializeField] private Space _space; 					/// <summary>Space's Relativeness.</summary>
	[SerializeField] private float _radius; 				/// <summary>Motion's Radius.</summary>
	[SerializeField] private float _motionSpeed; 			/// <summary>Motion's Speed.</summary>
	private float _time; 									/// <summary>Motion's Time.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets type property.</summary>
	public MotionType type
	{
		get { return _type; }
		set { _type = value; }
	}

	/// <summary>Gets and Sets axes property.</summary>
	public NormalizedVector3 axes
	{
		get { return _axes; }
		set { _axes = value; }
	}

	/// <summary>Gets and Sets space property.</summary>
	public Space space
	{
		get { return _space; }
		set { _space = value; }
	}

	/// <summary>Gets and Sets radius property.</summary>
	public float radius
	{
		get { return _radius; }
		set { _radius = value; }
	}

	/// <summary>Gets and Sets motionSpeed property.</summary>
	public float motionSpeed
	{
		get { return _motionSpeed; }
		set { _motionSpeed = value; }
	}

	/// <summary>Gets and Sets time property.</summary>
	public float time
	{
		get { return _time; }
		protected set { _time = value; }
	}
#endregion

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

			switch(type)
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