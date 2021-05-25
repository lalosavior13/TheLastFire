using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;
using UnityEngine.SceneManagement;

namespace Flamingo
{
public enum Faction
{
	Ally,
	Enemy
}

/// <summary>Event invoked when an ID event occurs.</summary>
/// <param name="_ID">Event's ID.</param>
public delegate void OnIDEvent(int _ID);

public class Game : Singleton<Game>
{
	[SerializeField] private GameData _data; 								/// <summary>Game's Data.</summary>
	[Space(5f)]
	[SerializeField] private PlayerController _mateoController; 			/// <summary>Mateo's Controller.</summary>
	[SerializeField] private Mateo _mateo; 									/// <summary>Mateo's Reference.</summary>
	[SerializeField] private GameplayCameraController _cameraController; 	/// <summary>Gameplay's Camera Controller.</summary>

	/// <summary>Gets and Sets data property.</summary>
	public static GameData data
	{
		get { return Instance._data; }
		set { Instance._data = value; }
	}

	/// <summary>Gets and Sets mateoController property.</summary>
	public static PlayerController mateoController
	{
		get { return Instance._mateoController; }
		set { Instance._mateoController = value; }
	}

	/// <summary>Gets and Sets mateo property.</summary>
	public static Mateo mateo
	{
		get { return Instance._mateo; }
		set { Instance._mateo = value; }
	}

	/// <summary>Gets and Sets cameraController property.</summary>
	public static GameplayCameraController cameraController
	{
		get { return Instance._cameraController; }
		set { Instance._cameraController = value; }
	}

	/// <summary>Callback internally called immediately after Awake.</summary>
	protected override void OnAwake()
	{
		data.Initialize();
	}

	private void Start()
	{
		if(mateo != null) AddTargetTransformToCamera(mateo.transform);
	}

#region TEMPORAL
	/// <summary>Updates Game's instance at each frame.</summary>
	private void Update()
	{
		if(InputController.InputBegin(6))
		{
			enabled = false;
			ResetScene();
		}
	}
#endregion

	/// <summary>Resets FSM Loop's States.</summary>
	public static void ResetFSMLoopStates()
	{
		if(data != null) data.ResetFSMLoopStates();
	}

	/// <returns>Mateo's Position.</returns>
	public static Vector2 GetMateoPosition()
	{
		return mateo != null ? mateo.transform.position : Vector3.zero;
	}

	/// <summary>Calculates Mateo's projected position [heuristics] on given time.</summary>
	/// <param name="t">Time's projection.</param>
	/// <returns>Mateo's Projected Position.</returns>
	public static Vector2 ProjectMateoPosition(float t)
	{
		return mateo != null ? mateo.transform.position + (mateo.deltaCalculator.velocity * t) : Vector3.zero;
	}

	/// <summary>Adds Target's Transform into the Camera.</summary>
	/// <param name="_transform">Transform to Add.</param>
	public static void AddTargetTransformToCamera(Transform _transform)
	{
		if(cameraController != null) cameraController.middlePointTargetRetriever.AddTargetTransform(_transform);
	}

	/// <summary>Removes Target's Transform into the Camera.</summary>
	/// <param name="_transform">Transform to Remove.</param>
	public static void RemoveTargetTransformToCamera(Transform _transform)
	{
		if(cameraController != null) cameraController.middlePointTargetRetriever.RemoveTargetTransform(_transform);
	}

	/// <summary>Loads Scene.</summary>
	/// <param name="_scene">Scene's Name.</param>
	public static void LoadScene(string _scene)
	{
		PlayerPrefs.SetString(GameData.PATH_SCENE_TOLOAD, _scene);
		SceneManager.LoadScene(GameData.PATH_SCENE_LOADING);
	}

	/// <summary>Resets current Scene.</summary>
	public static void ResetScene()
	{
		Scene scene = SceneManager.GetActiveScene();
		LoadScene(scene.name);
	}
}
}