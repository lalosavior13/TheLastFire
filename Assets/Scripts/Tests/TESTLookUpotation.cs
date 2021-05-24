using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

public class TESTLookUpotation : MonoBehaviour
{
	[SerializeField] private Transform t; 		/// <summary>Transform to rotate.</summary>
	[SerializeField] private Vector3 up; 		/// <summary>Up Vector.</summary>
	[SerializeField] private Vector3 target; 	/// <summary>Target.</summary>

	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		if(t == null) return;

		float length = 5.0f;
		Vector3 forward = (target - t.position).normalized;
		Vector3 cross = Vector3.Cross(up, forward);
		Vector3 invertedCross = Vector3.Cross(forward, up);

		Gizmos.DrawSphere(target, 0.25f);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(t.position, t.right * length);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(t.position, t.up * length);
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(t.position, t.forward * length);
		Gizmos.color = Color.cyan;
		Gizmos.DrawRay(t.position, cross * length);
		Gizmos.color = Color.magenta;
		Gizmos.DrawRay(t.position, invertedCross * length);
	}

	/// <summary>Resets TESTLookUpotation's instance to its default values.</summary>
	private void Reset()
	{
		up = Vector3.up;
	}

	/// <summary>TESTLookUpotation's tick at each frame.</summary>
	private void Update ()
	{
		if(t == null) return;

		t.rotation = VQuaternion.UpLookRotation(target - t.position, up);		
	}
}