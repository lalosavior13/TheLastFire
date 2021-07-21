using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public struct Trigger2DInformation
{
	private Collider2D _collider; 	/// <summary>Collider that has been intersected with.</summary>
	private Vector3 _contactPoint;	/// <summary>The point of intersection.</summary>
	private Vector3 _direction; 	/// <summary>Direction of the Collider2D A towards Collider2D B.</summary>

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

	/// <summary>Gets and Sets direction property.</summary>
	public Vector3 direction
	{
		get { return _direction; }
		set { _direction = value; }
	}

	/// <summary>Trigger2DInformation's Constructor.</summary>
	/// <param name="_collider">Collider2D's reference.</param>
	/// <param name="_contactPoint">Point of contact.</param>
	public Trigger2DInformation(Collider2D a, Collider2D b) : this()
	{
		if(a == null || b == null) return;

		Vector3 point = a.bounds.ClosestPoint(b.transform.position);

		collider = b;
		contactPoint = point;
		direction = point - a.transform.position;
	}

	/// <summary>Creates a Trigger2DInformation structore from 2 Collider2Ds.</summary>
	/// <param name="a">Collider2D A.</param>
	/// <param name="b">Collider2D B.</param>
	/// <returns>Trigger2DInformation given two Collider2Ds.</returns>
	public static Trigger2DInformation Create(Collider2D a, Collider2D b)
	{
		return new Trigger2DInformation(a, b);
	}

	/// <returns>String representing this Trigger2D's Information.</returns>
	public override string ToString()
	{
		if(collider == null)
		return "No Trigger 2D information could be retreived since there was no Collider2D data...";

		StringBuilder builder = new StringBuilder();

		builder.AppendLine("Trigger2D's Information: ");
		builder.Append("Collider2D of type ");
		builder.Append(collider.name);
		builder.Append(" from GameObject ");
		builder.AppendLine(collider.gameObject.name);
		builder.Append("Contact Point: ");
		builder.AppendLine(contactPoint.ToString());
		builder.Append("Direction: ");
		builder.Append(direction.ToString());

		return builder.ToString();
	}
}
}