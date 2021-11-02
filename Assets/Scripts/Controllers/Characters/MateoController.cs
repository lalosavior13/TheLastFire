using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class MateoController : Singleton<MateoController>
{
	[SerializeField] private Mateo _mateo; 	/// <summary>Mateo's Reference.</summary>

	/// <summary>Gets and Sets mateo property.</summary>
	public Mateo mateo
	{
		get { return _mateo; }
		set { _mateo = value; }
	}

#region UnityMethods:
	/// <summary>MateoController's instance initialization.</summary>
	private void Awake()
	{
		
	}

	/// <summary>MateoController's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		
	}
	
	/// <summary>MateoController's tick at each frame.</summary>
	private void Update ()
	{
		
	}
#endregion
}
}