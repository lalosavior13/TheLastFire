using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

using Random = UnityEngine.Random;

namespace Flamingo
{
public class TESTMateoGameBreaker : MonoBehaviour
{
	[SerializeField] private Mateo mateo; 							/// <summary>Mateo's Reference.</summary>
	[Space(5f)]
	[SerializeField] private FloatRange attackInterval; 			/// <summary>Attack Durations' Interval.</summary>
	[SerializeField] private FloatRange jumpInterval; 				/// <summary>Jump Duration's Interval.</summary>
	[SerializeField] private FloatRange movementInterval; 			/// <summary>Movement Duration's Interval.</summary>
	[SerializeField] private FloatRange rightAxesChangeInterval; 	/// <summary>Right-Axes Chabge's Interval.</summary>
	private float attackDuration; 									/// <summary>Current Attack's Duration.</summary>
	private float jumpDuration; 									/// <summary>Current Jump's Duration.</summary>
	private float movementDuration; 								/// <summary>Current Movement's Duration.</summary>
	private float rightAxesChangeDuration; 							/// <summary>Right-Axes Change Duration.</summary>
	private float attackTime; 										/// <summary>Current Attack's Time.</summary>
	private float jumpTime; 										/// <summary>Current Jump's Time.</summary>
	private float movementTime; 									/// <summary>Current Movement's Time.</summary>
	private float rightAxesChangeTime; 								/// <summary>Current Right-Axes' Time.</summary>
	private Vector2 leftAxes; 										/// <summary>Current Desired Displacement.</summary>
	private Vector2 rightAxes; 										/// <summary>Right Axes.</summary>

	/// <summary>TESTMateoGameBreaker's instance initialization.</summary>
	private void Awake()
	{
		attackDuration = attackInterval.Random();
		jumpDuration = jumpInterval.Random();
		movementDuration = movementInterval.Random();
		rightAxesChangeDuration = rightAxesChangeInterval.Random();
		CalculateNewLeftAxes();
		CalculateNewRightAxes();
	}
	
	/// <summary>TESTMateoGameBreaker's tick at each frame.</summary>
	private void Update ()
	{
		if(mateo == null) return;

		float t = Time.deltaTime;
		
		if(attackTime >= attackDuration)
		{
			attackDuration = attackInterval.Random();
			attackTime = 0.0f;
			mateo.SwordAttack(leftAxes);
		}

		if(jumpTime >= jumpDuration)
		{
			jumpDuration = jumpInterval.Random();
			jumpTime = 0.0f;
			mateo.Jump(leftAxes);
		}

		if(movementTime >= movementDuration)
		{
			movementDuration = movementInterval.Random();
			movementTime = 0.0f;
			CalculateNewLeftAxes();
		}

		if(rightAxesChangeTime >= rightAxesChangeDuration)
		{
			rightAxesChangeDuration = rightAxesChangeInterval.Random();
			rightAxesChangeTime = 0.0f;
			CalculateNewRightAxes();
		}

		mateo.OnLeftAxesChange(leftAxes);
		mateo.OnRightAxesChange(rightAxes);

		attackTime += t;
		jumpTime += t;
		movementTime += t;
		rightAxesChangeTime += t;
	}

	/// <summary>Updates TESTMateoGameBreaker's instance at each Physics Thread's frame.</summary>
	private void FixedUpdate()
	{
		if(mateo == null) return;

		if(leftAxes.x != 0.0f) mateo.Move(leftAxes.WithY(0.0f));
	}

	/// <summary>Calculates new Desired Displacement.</summary>
	private void CalculateNewLeftAxes()
	{
		int random = Random.Range(0, 3);

		switch(random)
		{
			case 0:
			leftAxes = Vector2.zero;
			break;

			case 1:
			leftAxes = Vector2.right;
			break;

			case 2:
			leftAxes = Vector2.left;
			break;
		}
	}

	/// <summary>Calculates new Desired Right-Axes.</summary>
	private void CalculateNewRightAxes()
	{
		rightAxes = Random.Range(0, 2) == 1 ? Vector2.zero : VVector2.Random(new FloatRange(0.0f, 1.0f)).normalized;
	}
}
}