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
	private const float MIN_DOT = -1.0f; 		/// <summary>Dot Product's Minimum Value.</summary>
	private const float MAX_DOT = 1.0f; 		/// <summary>Dot Product's Maximum Value.</summary>
	private const float MIN_ANGLE = 0.0f; 		/// <summary>Angle's Minimum Value.</summary>
	private const float MAX_ANGLE = 180.0f; 	/// <summary>Angle's Maximum Value.</summary>

	private GameData gameData; 					/// <summary>Inspector's Target.</summary>

	/// <summary>Sets target property.</summary>
	private void OnEnable()
	{
		gameData = target as GameData;

		if(gameData.ceilingDotProductThreshold !=  null) gameData.ceilingDotProductThreshold = new FloatWrapper(0.0f);
		if(gameData.floorDotProductThreshold !=  null) gameData.floorDotProductThreshold = new FloatWrapper(0.0f);
		if(gameData.ceilingAngleThreshold !=  null) gameData.ceilingAngleThreshold = new FloatWrapper(0.0f);
		if(gameData.floorAngleThreshold !=  null) gameData.floorAngleThreshold = new FloatWrapper(0.0f);
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
		VEditorGUILayout.Spaces(1);

		switch(dotEnabled)
		{
			case true:
			gameData.ceilingDotProductThreshold.value = EditorGUILayout.Slider("Ceiling's Dot Product Threshold", gameData.ceilingDotProductThreshold.value, MIN_DOT, MAX_DOT);
			gameData.floorDotProductThreshold.value = EditorGUILayout.Slider("Floor's Dot Product Threshold", gameData.floorDotProductThreshold.value, MIN_DOT, MAX_DOT);
			gameData.ceilingAngleThreshold.value = VMath.DotProductToAngle(gameData.ceilingDotProductThreshold.value);
			gameData.floorAngleThreshold.value = VMath.DotProductToAngle(gameData.floorDotProductThreshold.value);
			EditorGUILayout.LabelField("Ceiling's Angle Threshold: " + gameData.ceilingAngleThreshold.value);
			EditorGUILayout.LabelField("Floor's Angle Threshold: " + gameData.floorAngleThreshold.value);
			/*VMath.ToDependableNumberSet(
				MIN_DOT,
				MAX_DOT,
				gameData.ceilingDotProductThreshold,
				gameData.floorDotProductThreshold
			);*/
			break;

			case false:
			gameData.ceilingAngleThreshold.value = EditorGUILayout.Slider("Ceiling's Angle Threshold", gameData.ceilingAngleThreshold.value, MIN_ANGLE, MAX_ANGLE);
			gameData.floorAngleThreshold.value = EditorGUILayout.Slider("Floor's Angle Threshold", gameData.floorAngleThreshold.value, MIN_ANGLE, MAX_ANGLE);
			gameData.ceilingDotProductThreshold.value = VMath.AngleToDotProduct(gameData.ceilingAngleThreshold.value);
			gameData.floorDotProductThreshold.value = VMath.AngleToDotProduct(gameData.floorAngleThreshold.value);
			EditorGUILayout.LabelField("Ceiling's Dot Product Threshold: " + gameData.ceilingDotProductThreshold.value);
			EditorGUILayout.LabelField("Floor's Dot Product Threshold: " + gameData.floorDotProductThreshold.value);
			/*VMath.ToDependableNumberSet(
				MIN_ANGLE,
				MAX_ANGLE,
				gameData.ceilingAngleThreshold,
				gameData.floorAngleThreshold
			);*/
			break;
		}

		VEditorGUILayout.Spaces(1);
		EditorGUILayout.LabelField("Ceiling's Angles' Limits: 180 - " + gameData.ceilingAngleThreshold.value);
		EditorGUILayout.LabelField("Wall's Angles' Limits: " + gameData.ceilingAngleThreshold.value + " - " + gameData.floorAngleThreshold.value);
		EditorGUILayout.LabelField("Ceiling's Angles' Limits: " + gameData.floorAngleThreshold.value + " - 0");
	}
}
}