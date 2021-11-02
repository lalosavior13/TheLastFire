using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Voidless;

namespace Flamingo
{
[CustomEditor(typeof(FlashWhenReceivingDamage))]
public class FlashWhenReceivingDamageInspector : Editor
{
	private FlashWhenReceivingDamage flashWhenReceivingDamage; 	/// <summary>Inspector's Target.</summary>

	/// <summary>Sets target property.</summary>
	private void OnEnable()
	{
		flashWhenReceivingDamage = target as FlashWhenReceivingDamage;
	}

	/// <summary>OnInspectorGUI override.</summary>
	public override void OnInspectorGUI()
	{	
		DrawDefaultInspector();
		VEditorGUILayout.Spaces(2);
		if(GUILayout.Button("Get Renderers")) flashWhenReceivingDamage.GetRenderers();	
	}
}
}