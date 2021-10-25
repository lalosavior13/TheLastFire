using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class OxfordTheFoxBoss : Boss
{
	[Range(0.0f, 3.0f)] public float speed;
	[Range(0.0f, 3.0f)] public float fadeDuration;
	public AnimationClip[] animations;

#region UnityMethods:
	/// <summary>OxfordTheFoxBoss's instance initialization.</summary>
	private void Awake()
	{
		animation.AddClips(animations);
	}

	/// <summary>OxfordTheFoxBoss's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		StartCoroutine(AnimationTest());

		//animation.Blend(clip, weight, fadeDuration)
	}

	private IEnumerator AnimationTest()
	{
		AnimationState state = null;
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);

		foreach(AnimationClip clip in animations)
		{
			//animation.Play(clip);
			animation.CrossFade(clip, fadeDuration);
			state = animation.GetAnimationState(clip);
			wait.ChangeDurationAndReset(state.length);
			
			while(wait.MoveNext())
			{
				state.speed = speed;
				yield return null;
			}
		}
	}
#endregion
}
}
