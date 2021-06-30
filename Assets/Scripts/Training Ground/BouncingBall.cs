using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(Rigidbody2D))]
public class BouncingBall : MonoBehaviour
{
	public float upForce;
	public float force;
	public HitCollider2D hitBox;
	private Rigidbody2D rigidbody;

#region UnityMethods:
	/// <summary>BouncingBall's instance initialization.</summary>
	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		hitBox.onTriggerEvent2D += OnTriggerEvent2D;
	}

	/// <summary>BouncingBall's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		//Game.AddTargetToCamera(transform);
	}
#endregion

	private void OnTriggerEvent2D(Collider2D _collider, HitColliderEventTypes _eventType, int _hitColliderID = 0)
	{
		Trigger2DInformation info = Trigger2DInformation.CreateTriggerInformation(hitBox.collider, _collider);
		Vector2 direction =  transform.position - info.contactPoint;

		rigidbody.AddForce((Vector2.up * upForce) + (direction.normalized * force), ForceMode2D.Impulse);
	}
}
}