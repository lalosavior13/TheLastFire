using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{

public enum MotionType
{
	Default,
	Radial,
	Sinusoidal
}

public class SelfMotionPerformer : MonoBehaviour
{
	[SerializeField] private Transform[] _children; 	/// <summary>Self-Contained children to move.</summary>
	[Space(5f)]
	[SerializeField] private MotionType _motionType; 	/// <summary>Motion's Type.</summary>
	[SerializeField] private NormalizedVector3 _axes; 	/// <summary>Axes of motion displacement.</summary>
	[SerializeField] private Space _space; 				/// <summary>Space's Relativeness.</summary>
	[SerializeField] private float _radius; 			/// <summary>Motion's Radius.</summary>
	[SerializeField] private float _motionSpeed; 		/// <summary>Motion's Speed.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[SerializeField] private Color gizmosColor; 		/// <summary>Gizmos' Color.</summary>
#endif
	private Vector3 _direction; 						/// <summary>Motion's Direction.</summary>
	private float _time; 								/// <summary>Motion's Time.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets children property.</summary>
	public Transform[] children
	{
		get { return _children; }
		set { _children = value; }
	}

	/// <summary>Gets and Sets motionType property.</summary>
	public MotionType motionType
	{
		get { return _motionType; }
		set { _motionType = value; }
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

	/// <summary>Gets and Sets direction property.</summary>
	public Vector3 direction
	{
		get { return _direction; }
		set { _direction = value; }
	}
#endregion

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = gizmosColor;

		if(motionType != MotionType.Default) Gizmos.DrawWireSphere(transform.position, radius);
	}
#endif

	/// <summary>Resets MotionProjectile's instance to its default values.</summary>
	private void Reset()
	{
		axes = Vector3.up;
		space = Space.Self;
	}

	/// <summary>SelfMotionPerformer's tick at each frame.</summary>
	private void Update ()
	{
		if(motionType == MotionType.Default) return;

		if(children == null) return;

		time = time < (360.0f * motionSpeed) ? time + (Time.fixedDeltaTime * motionSpeed) : 0.0f;

		Vector3 position = transform.position;
		Vector3 childPosition = position;
		Vector3 motionAxis = axes.normalized;
		Vector3 motion = /*space == Space.Self ? Vector3.Cross(direction, Vector3.forward) : */motionAxis;
		if(direction.sqrMagnitude == 0.0f) direction = Vector3.right;
		
		foreach(Transform child in children)
		{
			if(child == null) continue;

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
				childPosition += transform.position + (motion * radius);
				break;
			}

			child.position = childPosition;																
		}
	}
}
}