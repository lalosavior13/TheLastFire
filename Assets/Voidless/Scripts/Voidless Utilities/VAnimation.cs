using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public static class VAnimation
{
	/// <summary>Adds AnimationClips into Animation Component.</summary>
	/// <param name="_animation">Animation's Component.</param>
	/// <param name="_clips">AnimationClips to add.</param>
	public static void AddClips(this Animation _animation, params AnimationClip[] _clips)
	{
		if(_clips == null) return;

		foreach(AnimationClip clip in _clips)
		{
			if(clip != null)
			{
				clip.legacy = true;
				_animation.AddClip(clip, clip.name);
			}
		}
	}

	/// <summary>Adds AnimationClip into Animation Component with the same AnimationClip's name.</summary>
	/// <param name="_animation">Animation Component.</param>
	/// <param name="_clip">Animation clip to add into the Animation Component.</param>
	public static void AddClip(this Animation _animation, AnimationClip _clip)
	{
		if(_clip == null) return;

		_clip.legacy = true;
		_animation.AddClip(_clip, _clip.name);
	}

	/// <summary>Gets animation state from AnimationClip.</summary>
	/// <param name="_animation">Animation's Component.</param>
	/// <param name="_clip">AnimationClip.</param>
	/// <returns>AnimationState from given AnimationClip.</returns>
	public static AnimationState GetAnimationState(this Animation _animation, AnimationClip _clip)
	{
		return _clip != null ? _animation[_clip.name] : null;
	}

	/// <summary>Plays an animation without blending.</summary>
	/// <param name="_animation">Animation's Component.</param>
	/// <param name="_clip">AnimationClip to play.</param>
	/// <param name="_mode">The optional PlayMode lets you choose how this animation will affect others already playing.</param>
	public static bool Play(this Animation _animation, AnimationClip _clip, PlayMode _mode = PlayMode.StopSameLayer)
	{
		return _animation.Play(_clip.name, _mode);
	}
}
}