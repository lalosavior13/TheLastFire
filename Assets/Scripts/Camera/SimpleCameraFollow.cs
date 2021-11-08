using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
	[SerializeField] private Transform target; 	/// <summary>Camera's Target.</summary>
	[SerializeField] private Vector3 offset; 	/// <summary>Offset from target.</summary>
	
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		if(Application.isPlaying || target == null) return;
		transform.position = (target.position + offset);
	}

	/// <summary>SimpleCameraFollow's tick at each frame.</summary>
	private void Update ()
	{
		if(target == null) return;

		transform.position = (target.position + offset);	
	}
}