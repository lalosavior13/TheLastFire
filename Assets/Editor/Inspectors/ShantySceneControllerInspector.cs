using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Voidless;

namespace Flamingo
{
[CustomEditor(typeof(ShantySceneController))]
public class ShantySceneControllerInspector : Editor
{
	private ShantySceneController shantysceneController; 	/// <summary>Inspector's Target.</summary>

	/// <summary>Sets target property.</summary>
	private void OnEnable()
	{
		shantysceneController = target as ShantySceneController;
	}

	/// <summary>OnInspectorGUI override.</summary>
	public override void OnInspectorGUI()
	{	
		DrawDefaultInspector();
		VEditorGUILayout.Spaces(2);
		if(GUILayout.Button("Test Stage 1")) shantysceneController.OnStageChanged(1);
		if(GUILayout.Button("Test Stage 2")) shantysceneController.OnStageChanged(2);
		if(GUILayout.Button("Test Stage 3")) shantysceneController.OnStageChanged(3);
	}
}
}