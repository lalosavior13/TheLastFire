using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
/// \TODO Think it through....
[RequireComponent(typeof(Health))]
public class LinkHurtBoxesToHealth : MonoBehaviour
{
	[SerializeField] private HitCollider2D[] _hurtBoxes; 	/// <summary>HurtBoxes.</summary>

	/// <summary>Gets and Sets hurtBoxes property.</summary>
	public HitCollider2D[] hurtBoxes
	{
		get { return _hurtBoxes; }
		set { _hurtBoxes = value; }
	}

#region UnityMethods:
	/// <summary>LinkHurtBoxesToHealth's instance initialization.</summary>
	private void Awake()
	{
			
	}

	/// <summary>LinkHurtBoxesToHealth's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		
	}
	
	/// <summary>LinkHurtBoxesToHealth's tick at each frame.</summary>
	private void Update ()
	{
		
	}
#endregion
}
}