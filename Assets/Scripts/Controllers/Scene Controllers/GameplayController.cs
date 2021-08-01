using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class GameplayController : Singleton<GameplayController>
{
	[SerializeField] private Vector2 _center; 	/// <summary>Scene's Center.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] protected Color color; 	/// <summary>Gizmos' Color.</summary>
#endif

	/// <summary>Gets center property.</summary>
	public static Vector2 center { get { return Instance._center; } }

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawWireSphere(center, 0.25f);
	}
#endif

	/// <summary>PoolManager's instance initialization.</summary>
	protected override void OnAwake()
	{

	}
}
}