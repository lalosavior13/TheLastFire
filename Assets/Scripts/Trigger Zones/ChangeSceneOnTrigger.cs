using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class ChangeSceneOnTrigger : MonoBehaviour
{
	[SerializeField] private string scene; 			/// <summary>Scene's Name.</summary>
	[SerializeField] private GameObjectTag[] tags; 	/// <summary>Tags that trigger the Scene Change.</summary>

	/// <summary>Event triggered when this Collider enters another Collider trigger.</summary>
	/// <param name="col">The other Collider involved in this Event.</param>
	private void OnTriggerEnter2D(Collider2D col)
	{
		GameObject obj = col.gameObject;
	
		foreach(GameObjectTag tag in tags)
		{
			if(obj.CompareTag(tag))
			{
				Game.LoadScene(scene);
				return;
			}
		}
	}
}
}