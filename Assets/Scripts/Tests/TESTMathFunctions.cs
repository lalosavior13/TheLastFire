using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

public class TESTMathFunctions : MonoBehaviour
{
	[SerializeField] private Transform reference; 		/// <summary>Reference's Transform.</summary>
	[SerializeField] private EulerRotation offset; 		/// <summary>Description.</summary>
	[SerializeField] private Vector3 target; 			/// <summary>Target's Position.</summary>
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color color; 				/// <summary>Gizmos' Color.</summary>
	[SerializeField] private float gizmosRadius; 		/// <summary>Gizmos' Radius.</summary>


	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawWireSphere(target, gizmosRadius);

		if(reference == null) return;

		//reference.rotation = LookRotation(target - reference.position);
		reference.RightLookAt(target);

		if(reference != transform)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(reference.position, reference.right);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(reference.position, reference.up);
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(reference.position, reference.forward);
			Gizmos.color = color;
		}
	}

	private Quaternion LookRotation(Vector3 direction)
	{
		return Quaternion.LookRotation(direction) * Quaternion.Inverse(offset);
	}
}