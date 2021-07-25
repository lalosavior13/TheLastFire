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

[Flags]
public enum SurfaceType
{
	Undefined = 0,
	Floor  = 1,
	Wall = 2,
	Ceiling = 4,

	FloorAndWall = Floor | Wall,
	FloorAndCeiling = Floor | Ceiling,
	WallAndCeiling = Wall | Ceiling,
	All = Floor | Wall | Ceiling
}

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
		if(mateo != null) AddTargetToCamera(mateo.cameraTarget);
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

	/// <summary>Gets Mateo's Maximum Jump Force.</summary>
	/// <param name="_allJumps">Predict the force of all jumps? If false, it will only project the force of the first jump.</param>
	/// <returns>Force projection of Mateo's jumps.</returns>
	public static Vector2 GetMateoMaxJumpingHeight(bool _allJumps = true)
	{
		return _allJumps ? mateo.jumpAbility.PredictForces() : mateo.jumpAbility.PredictForce(0);
	}

	/// <summary>Adds Target's VCameraTarget into the Camera.</summary>
	/// <param name="_target">VCameraTarget to Add.</param>
	public static void AddTargetToCamera(VCameraTarget _target)
	{
		if(cameraController != null) cameraController.middlePointTargetRetriever.AddTarget(_target);
	}

	/// <summary>Removes Target's VCameraTarget into the Camera.</summary>
	/// <param name="_target">VCameraTarget to Remove.</param>
	public static void RemoveTargetToCamera(VCameraTarget _target)
	{
		if(cameraController != null) cameraController.middlePointTargetRetriever.RemoveTarget(_target);
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

	/// <summary>Evaluates Surface Type.</summary>
	/// <param name="u">Up's Normal.</param>
	/// <param name="n">Face's Normal.</param>
	public static SurfaceType EvaluateSurfaceType(Vector2 n)
	{
		if(n.sqrMagnitude != 1.0f) n.Normalize();

		Vector2 g = -Physics2D.gravity.normalized;
		float d = Vector2.Dot(n, g);
		float c = data.ceilingDotProductThreshold;
		float f = data.floorDotProductThreshold;

		if(d >= -1.0f && d < c) return SurfaceType.Ceiling;
		if(d >= c && d < f) return SurfaceType.Wall;
		if(d  >= f && d <= 1.0f) return SurfaceType.Floor; 

		return SurfaceType.Undefined;
	}
}
}