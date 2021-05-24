using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
[CreateAssetMenu]
public class FiniteStateAudioClip : ScriptableObject
{
	[SerializeField] private AudioClip _clip; 				/// <summary>AudioClip's reference.</summary>
	[SerializeField] private FloatWrapper[] _statesRanges; 	/// <summary>States' Time Ranges.</summary>
	private float _time; 									/// <summary>Current's Time.</summary>

	/// <summary>Gets and Sets clip property.</summary>
	public AudioClip clip
	{
		get { return _clip; }
		set { _clip = value; }
	}

	/// <summary>Gets and Sets statesRanges property.</summary>
	public FloatWrapper[] statesRanges
	{
		get { return _statesRanges; }
		set { _statesRanges = value; }
	}

	/// <summary>Gets and Sets time property.</summary>
	public float time
	{
		get { return _time; }
		set { _time = value; }
	}

	/// <summary>Resets FiniteStateAudioClip's instance to its default values.</summary>
	public void Reset()
	{
		if(statesRanges == null)
		{
			statesRanges = new FloatWrapper[1];
			statesRanges[0] = new FloatWrapper(0.0f);
		}
	}

	/// <returns>Current State's ID.</returns>
	public int GetCurrentStateIndex()
	{
		float min = 0.0f;
		int i = 0;

		foreach(FloatWrapper stateRange in statesRanges)
		{
			if(time >= min && time < stateRange) return i;
			
			min = stateRange;
			i++;
		}

		return i;
	}

	/// <returns>Current State's Time.</returns>
	public float GetCurrentStateTime()
	{
		int index = GetCurrentStateIndex();

		return index == 0 ? 0.0f : statesRanges[index - 1];
	}

	/// <summary>Changes AudioClip's state index.</summary>
	/// <param name="_index">New state's index [internally clamped].</param>
	public void ChangeState(int _index)
	{
		_index = Mathf.Clamp(_index, 0, statesRanges.Length - 1);
		time = _index == 0 ? 0.0f : statesRanges[_index - 1];
	}

	/// <summary>Resets state's index and internal time.</summary>
	public void ResetState()
	{
		ChangeState(0);
	}

}
}

/*
	void PlaySoundInterval(float fromSeconds, toSeconds)
	{
	 audioSource.time = fromSeconds;
	 audioSource.Play();
	 audioSource.SetScheduledEndTime(AudioSettings.dspTime+(toSeconds-fromSeconds));
	}
*/