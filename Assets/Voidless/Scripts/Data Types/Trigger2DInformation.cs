using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public struct Trigger2DInformation
{
	private Collider2D _collider; 	/// <summary>Collider that has been intersected with.</summary>
	private Vector3 _contactPoint;	/// <summary>The point of intersection.</summary>

	/// <summary>Gets and Sets collider property.</summary>
	public Collider2D collider
	{
		get { return _collider; }
		private set { _collider = value; }
	}

	/// <summary>Gets and Sets contactPoint property.</summary>
	public Vector3 contactPoint
	{
		get { return _contactPoint; }
		private set { _contactPoint = value; }
	}

	/// <summary>Trigger2DInformation's Constructor.</summary>
	/// <param name="_collider">Collider2D's reference.</param>
	/// <param name="_contactPoint">Point of contact.</param>
	public Trigger2DInformation(Collider2D _collider, Vector3 _contactPoint) : this()
	{
		collider = _collider;
		contactPoint = _contactPoint;
	}

	/// <summary>Creates a Trigger2DInformation structore from 2 Collider2Ds.</summary>
	/// <param name="a">Collider2D A.</param>
	/// <param name="b">Collider2D B.</param>
	/// <returns>Trigger2DInformation given two Collider2Ds.</returns>
	public static Trigger2DInformation CreateTriggerInformation(Collider2D a, Collider2D b)
	{
		Vector3 point = a.bounds.ClosestPoint(b.transform.position);
		return new Trigger2DInformation(b, point);
	}
}
}