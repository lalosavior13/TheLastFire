using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Voidless;

namespace Flamingo
{
[CustomEditor(typeof(GameData))]
[CanEditMultipleObjects]
public class GameDataInspector : Editor
{
	private GameData gameData; 	/// <summary>Inspector's Target.</summary>

	/// <summary>Sets target property.</summary>
	private void OnEnable()
	{
		gameData = target as GameData;

		if(gameData.surfacesAngleThresholds == null || gameData.surfacesDotProductThresholds == null)
		{
			gameData.surfacesAngleThresholds = new FloatWrapper[4];	
			gameData.surfacesDotProductThresholds = new FloatWrapper[4];	
		}
	}

	/// <summary>OnInspectorGUI override.</summary>
	public override void OnInspectorGUI()
	{	
		DrawDefaultInspector();
		DrawSurfacesAnglesSettings();
	}

	/// <summary>Draws Settings for the Surfaces's Angles' Thresholds.</summary>
	private void DrawSurfacesAnglesSettings()
	{
		bool dotEnabled = gameData.showDotProducts;
		string enabledSetting = dotEnabled ? "Dot Product" : "Angle";
		string disabledSetting = !dotEnabled ? "Dot Product" : "Angle";

		VEditorGUILayout.Spaces(2);

		EditorGUILayout.LabelField(enabledSetting + "s Thresholds' Settings: ");
		dotEnabled = EditorGUILayout.Toggle("Set to " + disabledSetting, dotEnabled);

		gameData.showDotProducts = dotEnabled;
	}
}
}