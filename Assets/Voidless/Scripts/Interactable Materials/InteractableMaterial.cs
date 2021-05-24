using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public class InteractableMaterial : MonoBehaviour
{
	[SerializeField] private int _ID; 	/// <summary>Material's ID.</summary>

	/// <summary>Gets and Sets ID property.</summary>
	public int ID
	{
		get { return _ID; }
		set { _ID = value; }
	}
}
}