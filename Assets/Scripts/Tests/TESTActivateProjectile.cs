using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;
using Flamingo;

public class TESTActivateProjectile : MonoBehaviour
{
	[SerializeField] private Projectile projectile; 	/// <summary>Projectile's reference.</summary>

	/// <summary>TESTActivateProjectile's instance initialization.</summary>
	private void Awake()
	{
		if(projectile != null) projectile.activated = true;
	}
}