using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Voidless;

namespace Flamingo
{
[CustomEditor(typeof(DeathBehavior))]
public class DeathBehaviorInspector : Editor
{
	private DeathBehavior deathBehavior; 	/// <summary>Inspector's Target.</summary>

	/// <summary>Sets target property.</summary>
	private void OnEnable()
	{
		deathBehavior = target as DeathBehavior;
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}

	/// <summary>Callback invoked when DeathBehaviorInspector's instance is disabled.</summary>
	private void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

	/// <summary>OnInspectorGUI override.</summary>
	public override void OnInspectorGUI()
	{	
		DrawDefaultInspector();
		EditorGUILayout.Space();
		if(deathBehavior.rotationsDataSet != null)
		deathBehavior.debugIndex = VEditorGUILayout.IntSlider("Debug Index ", deathBehavior.debugIndex, 0, deathBehavior.rotationsDataSet.Length - 1);
		deathBehavior.debugSubIndex = VEditorGUILayout.IntSlider("Debug Sub-Index ", deathBehavior.debugSubIndex, 0, deathBehavior.rotationsDataSet[deathBehavior.debugIndex].rotationDataSet.Length - 1);
		serializedObject.ApplyModifiedProperties();
	}

	/// <summary>Enables the Editor to handle an event in the Scene view.</summary>
	public void OnSceneGUI(SceneView _view)
	{
		/*RotationDataSet[] sets = deathBehavior.rotationsDataSet;

		if(sets == null || sets.Length <= 0) return;
		
		RotationData data = sets[deathBehavior.debugIndex].rotationDataSet[deathBehavior.debugSubIndex];
		Vector3 axis = deathBehavior.scytheRotationAxis;
		Vector3 orientation = deathBehavior.snathOrientationVector;
		float halfBuildUpRotation = 0.0f;
		float halfSlashRotation = 0.0f;
		float buildUpSign = 0.0f;
		float swingSign = 0.0f;*/

		//foreach(RotationDataSet dataSet in sets)
		//{
			//foreach(RotationData data in /*dataSet*/sets[deathBehavior.debugIndex])
			//{
				/*buildUpSign = Mathf.Sign(data.buildUpAngularSpeed);
				swingSign = Mathf.Sign(data.swingAngularSpeed);
				halfBuildUpRotation = Vector3.SignedAngle(data.buildUpDirection, orientation, axis) * buildUpSign;
				halfSlashRotation = Vector3.SignedAngle(data.swingDirection, orientation, axis) * swingSign;

				Handles.color = deathBehavior.buildUpColor;
				Handles.DrawSolidArc(deathBehavior.pivot, -axis, Vector3.down, halfBuildUpRotation, deathBehavior.gizmosRadius);
				Handles.color = deathBehavior.swingColor;
				Handles.DrawSolidArc(deathBehavior.pivot, -axis, Vector3.down, halfSlashRotation, deathBehavior.gizmosRadius);*/
			//}
		//}
	}
}
}