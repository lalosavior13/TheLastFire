using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class ScenarioBoundariesContainer : MonoBehaviour
{
	[SerializeField] private BoxCollider2D _ceiling; 	/// <summary>Ceiling's BoxCollider.</summary>
	[SerializeField] private BoxCollider2D _floor; 		/// <summary>Floor's BoxCollider.</summary>
	[SerializeField] private BoxCollider2D _leftWall; 	/// <summary>Left Wall's BoxCollider.</summary>
	[SerializeField] private BoxCollider2D _rightWall; 	/// <summary>Right Wall's BoxCollider.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Atrributes:")]
	[SerializeField] private Color gizmosColor; 		/// <summary>Gizmos' Color.</summary>
#endif

	/// <summary>Gets and Sets ceiling property.</summary>
	public BoxCollider2D ceiling
	{
		get { return _ceiling; }
		set { _ceiling = value; }
	}

	/// <summary>Gets and Sets floor property.</summary>
	public BoxCollider2D floor
	{
		get { return _floor; }
		set { _floor = value; }
	}

	/// <summary>Gets and Sets leftWall property.</summary>
	public BoxCollider2D leftWall
	{
		get { return _leftWall; }
		set { _leftWall = value; }
	}

	/// <summary>Gets and Sets rightWall property.</summary>
	public BoxCollider2D rightWall
	{
		get { return _rightWall; }
		set { _rightWall = value; }
	}

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = gizmosColor;

		if(ceiling != null)
		Gizmos.DrawCube(ceiling.transform.position + (ceiling.transform.rotation * ceiling.offset), Vector3.Scale(ceiling.transform.localScale, ceiling.size.ToVector3().WithZ(1.0f)));
		
		if(floor != null)
		Gizmos.DrawCube(floor.transform.position + (floor.transform.rotation * floor.offset), Vector3.Scale(floor.transform.localScale, floor.size.ToVector3().WithZ(1.0f)));
		
		if(leftWall != null)
		Gizmos.DrawCube(leftWall.transform.position + (leftWall.transform.rotation * leftWall.offset), Vector3.Scale(leftWall.transform.localScale,  leftWall.size.ToVector3().WithZ(1.0f)));
		
		if(rightWall != null)
		Gizmos.DrawCube(rightWall.transform.position + (rightWall.transform.rotation * rightWall.offset), Vector3.Scale(rightWall.transform.localScale, rightWall.size.ToVector3().WithZ(1.0f)));
	}

	/// <summary>Resets ScenarioBoundariesContainer's instance to its default values.</summary>
	private void Reset()
	{
		gizmosColor = Color.green.WithAlpha(0.5f);
	}
#endif
}
}