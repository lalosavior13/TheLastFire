using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Voidless;

namespace Flamingo
{
[CustomEditor(typeof(JumpAbility))]
public class JumpAbilityInspector : Editor
{
	private JumpAbility jumpAbility; 	/// <summary>Inspector's Target.</summary>

	/// <summary>Sets target property.</summary>
	private void OnEnable()
	{
		jumpAbility = target as JumpAbility;
	}

	/// <summary>OnInspectorGUI override.</summary>
	public override void OnInspectorGUI()
	{	
		DrawDefaultInspector();

		if(!Application.isPlaying) return;
		
		GUILayout.Space(20.0f);
		if(GUILayout.Button("Update Forces' Appliers")) jumpAbility.UpdateForcesAppliers();
	}
}
}