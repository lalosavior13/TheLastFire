using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public delegate void OnDestroyed();

public class DestroyOnImpact : MonoBehaviour
{
	public event OnDestroyed onDestroyed;

	[SerializeField] private LayerMask _mask; 	/// <summary>Impact's Mask.</summary>

	/// <summary>Gets mask property.</summary>
	public LayerMask mask { get { return _mask; } }

	/// <summary>Event triggered when this Collider2D enters another Collider2D trigger.</summary>
	/// <param name="col">The other Collider2D involved in this Event.</param>
	private void OnTriggerEnter2D(Collider2D col)
	{
		GameObject obj = col.gameObject;
		int layer = 1 << obj.layer;

		if((mask | layer) == mask)
		{
			gameObject.SetActive(false);
			if(onDestroyed != null) onDestroyed();
		}
	}
}
}