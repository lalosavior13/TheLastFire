using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class TESTAnimatorControllerTransitionsNoFSM : MonoBehaviour
{
	[SerializeField] private AnimatorCredential[] stateCredentials; 	/// <summary>Animation States' Credentials.</summary>
	[SerializeField] private string[] states; 							/// <summary>Animation States.</summary>
	[SerializeField] private Animator _animator; 						/// <summary>Animator's Component.</summary>
	private int index; 													/// <summary>Current State's Index.</summary>

	/// <summary>Gets animator Component.</summary>
	public Animator animator
	{ 
		get
		{
			if(_animator == null) _animator = GetComponent<Animator>();
			return _animator;
		}
	}

	/// <summary>TESTAnimatorControllerTransitionsNoFSM's instance initialization.</summary>
	private void Awake()
	{
		index = 0;
		animator.Play(stateCredentials[index]);
	}
	
	/// <summary>TESTAnimatorControllerTransitionsNoFSM's tick at each frame.</summary>
	private void Update ()
	{
		if(stateCredentials == null) return;

		if(Input.GetKeyUp(KeyCode.Space))
		{
			index = index + 1 < stateCredentials.Length ? index + 1 : 0;
			animator.Play(stateCredentials[index]);
		}	
	}
}
}