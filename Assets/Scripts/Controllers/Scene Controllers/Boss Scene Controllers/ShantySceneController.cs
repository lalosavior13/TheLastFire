using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class ShantySceneController : Singleton<ShantySceneController>
{
	[Space(5f)]
	[SerializeField] private CollectionIndex _loopIndex; 	/// <summary>Loop's Index.</summary>

	/// <summary>Gets loopIndex property.</summary>
	public CollectionIndex loopIndex { get { return _loopIndex; } }

#region UnityMethods:
	/// <summary>ShantySceneController's instance initialization.</summary>
	private void Awake()
	{
		AudioController.Play(SourceType.Loop, 0, loopIndex);
	}

	/// <summary>ShantySceneController's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		
	}
	
	/// <summary>ShantySceneController's tick at each frame.</summary>
	private void Update ()
	{
		
	}
#endregion
}
}