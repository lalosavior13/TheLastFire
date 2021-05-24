using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flamingo
{
public class ChangeSceneOnTrigger : MonoBehaviour
{
	[SerializeField] private string _scenePath; 	/// <summary>Scene's Path.</summary>
	[SerializeField] private string _tag; 

	/// <summary>Gets scenePath property.</summary>
	public string scenePath { get { return _scenePath; } }

	/// <summary>Gets tag property.</summary>
	public string tag { get { return _tag; } }

	/// <summary>Event triggered when this Collider enters another Collider trigger.</summary>
	/// <param name="col">The other Collider involved in this Event.</param>
	private void OnTriggerEnter2D(Collider2D col)
	{
		GameObject obj = col.gameObject;
		if(obj.CompareTag(tag))
	
		/*switch(obj.tag)
		{
			case "NULL":
			break;
	
			default:
			break;
		}*/

		Game.LoadScene(scenePath);
	}
}
}