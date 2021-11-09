using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public class VirtualAnchorContainer : MonoBehaviour
{
	[SerializeField] private Vector3[] _anchors; 	/// <summary>Virtual anchors.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color gizmosColor; 	/// <summary>Gizmos' Color.</summary>
	[SerializeField] private float gizmosRadius; 	/// <summary>Gizmos' Radius.</summary>
#endif

	/// <summary>Gets and Sets anchors property.</summary>
	public Vector3[] anchors
	{
		get { return _anchors; }
		set { _anchors = value; }
	}

	/// <summary>Draws Gizmos on Editor mode when VirtualAnchorContainer's instance is selected.</summary>
	private void OnDrawGizmosSelected()
	{
		if(anchors == null) return;

#if UNITY_EDITOR
		Gizmos.color = gizmosColor;

		for(int i = 0; i < anchors.Length; i++)
		{
			Gizmos.DrawSphere(GetAnchoredPosition(i), gizmosRadius);
		}
#endif
	}

	/// <summary>Resets VirtualAnchorContainer's instance to its default values.</summary>
	private void Reset()
	{
#if UNITY_EDITOR
		gizmosColor = Color.magenta.WithAlpha(0.5f);
		gizmosRadius = 0.1f;
#endif
	}

	/// <summary>Get Virtual Anchor.</summary>
	/// <param name="index">Anchor's Index [0 by default].</param>
	/// <returns>Virtual Anchor.</returns>
	public Vector3 GetAnchoredPosition(int index = 0)
	{
		return (anchors != null) ? transform.TransformPoint(anchors[index]) : transform.position;
	}

	/// <summary>Gets anchor position relative to another position.</summary>
	/// <param name="position">Relative position.</param>
	/// <param name="index">Anchor's Index [0 by default].</param>
	/// <returns>Anchored position relative to another position.</returns>
	public Vector3 GetAnchoredPosition(Vector3 position, int index = 0)
	{
		return position - (transform.rotation * anchors[index]);
	}

	/// <summary>Sets position relative to virtual anchor.</summary>
	/// <param name="position">Position to set to the Transform.</param>
	/// <param name="index">Anchor's Index [0 by default].</param>
	public void SetAnchoredPosition(Vector3 position, int index = 0)
	{
		transform.position = GetAnchoredPosition(position, index);
	}
}
}