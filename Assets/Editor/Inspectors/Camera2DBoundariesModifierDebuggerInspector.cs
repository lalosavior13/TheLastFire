using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Voidless;

namespace Flamingo
{
[CustomEditor(typeof(Camera2DBoundariesModifierDebugger))]
public class Camera2DBoundariesModifierDebuggerInspector : Editor
{
	private Camera2DBoundariesModifierDebugger camera2DBoundariesModifierDebugger; 	/// <summary>Inspector's Target.</summary>

	/// <summary>Sets target property.</summary>
	private void OnEnable()
	{
		camera2DBoundariesModifierDebugger = target as Camera2DBoundariesModifierDebugger;
	}

	/// <summary>OnInspectorGUI override.</summary>
	public override void OnInspectorGUI()
	{	
		DrawDefaultInspector();
		if(GUILayout.Button("Fetch Camera 2D Boundaries Modifiers")) camera2DBoundariesModifierDebugger.FetchCamera2DBoundariesModifiers();
		if(GUILayout.Button("Create/Update Line Renderers")) camera2DBoundariesModifierDebugger.CreateLineRenderers();
	}
}
}