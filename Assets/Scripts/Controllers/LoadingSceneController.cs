using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voidless;
using UnityEngine.SceneManagement;

namespace Flamingo
{
public class LoadingSceneController : MonoBehaviour
{
	[SerializeField] private RectTransform _loadingBar; 	/// <summary>Loading Bar's UI.</summary>
	[SerializeField] private float _additionalWait; 		/// <summary>Additional Wait.</summary>

	/// <summary>Gets loadingBar property.</summary>
	public RectTransform loadingBar { get { return _loadingBar; } }

	/// <summary>Gets additionalWait property.</summary>
	public float additionalWait { get { return _additionalWait; } }

	/// <summary>LoadingSceneController's instance initialization.</summary>
	private void Awake()
	{
		if(loadingBar != null) loadingBar.localScale = new Vector3(0.0f, 1.0f, 1.0f);
	}

	/// <summary>LoadingSceneController's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		string scenePath = PlayerPrefs.GetString(GameData.PATH_SCENE_TOLOAD, GameData.PATH_SCENE_DEFAULT);

		//AsynchronousSceneLoader.LoadScene(scenePath);		
		this.StartCoroutine(AsynchronousSceneLoader.LoadSceneAndDoWhileWaiting(
			scenePath,
			OnSceneLoading,
			null,
			LoadSceneMode.Single,
			additionalWait
		));
	}
	
	/// <summary>LoadingSceneController's tick at each frame.</summary>
	/// <param name="progress">Normalized progress of the loading.</param>
	private void OnSceneLoading(float progress)
	{
		if(loadingBar == null) return;

		Vector3 scale = loadingBar.localScale;
		scale.x = progress;
		loadingBar.localScale = scale;
	}
}
}