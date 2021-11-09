using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flamingo;

public class TEST_GoToScene : MonoBehaviour
{
	[SerializeField] private string sceneName; 	/// <summary>Scene's Name.</summary>

	/// <summary>TEST_GoToScene's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		Game.LoadScene(sceneName);	
	}
}