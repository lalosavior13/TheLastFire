using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Voidless;

namespace Flamingo
{
/*public enum AudioSourceType
{
	Loop,
	Scenario,
	...
}*/

[RequireComponent(typeof(AudioSource))]
public class AudioController : Singleton<AudioController>
{
	[Header("Audio's Settings:")]
	[SerializeField] private string _exposedVolumeParameterName; 	/// <summary>Exposed Volume Parameter's Name.</summary>
	[SerializeField] private float _fadeOutDuration; 				/// <summary>Fade-Out's Duration.</summary>
	[SerializeField] private float _fadeInDuration; 				/// <summary>Fade-In's Duration.</summary>
	[Space(5f)]
	[Header("Audio Sources:")]
	[SerializeField] private AudioSource[] _loopSources; 			/// <summary>Loops' AudioSources.</summary>
	[SerializeField] private AudioSource[] _scenarioSources; 		/// <summary>Scenario's AudioSources.</summary>
	[SerializeField] private AudioSource[] _soundEffectSources; 	/// <summary>Mateo's Audiosources.</summary>
	private AudioSource _audioSource; 								/// <summary>AudioSource's Component.</summary>
	private int _lastLoopIndex; 									/// <summary>Last Loop's Index.</summary>
	private Coroutine volumeFading; 								/// <summary>Volume Fading's Coroutine Reference.</summary>

	/// <summary>Gets exposedVolumeParameterName property.</summary>
	public string exposedVolumeParameterName { get { return _exposedVolumeParameterName; } }

	/// <summary>Gets fadeOutDuration property.</summary>
	public float fadeOutDuration { get { return _fadeOutDuration; } }

	/// <summary>Gets fadeInDuration property.</summary>
	public float fadeInDuration { get { return _fadeInDuration; } }

	/// <summary>Gets loopSources property.</summary>
	public static AudioSource[] loopSources { get { return Instance._loopSources; } }

	/// <summary>Gets scenarioSources property.</summary>
	public static AudioSource[] scenarioSources { get { return Instance._scenarioSources; } }

	/// <summary>Gets soundEffectSources property.</summary>
	public static AudioSource[] soundEffectSources { get { return Instance._soundEffectSources; } }

	/// <summary>Gets audioSource Component.</summary>
	public AudioSource audioSource
	{ 
		get
		{
			if(_audioSource == null) _audioSource = GetComponent<AudioSource>();
			return _audioSource;
		}
	}

	/// <summary>Gets and Sets lastLoopIndex property.</summary>
	public static int lastLoopIndex
	{
		get { return Instance._lastLoopIndex; }
		set { Instance._lastLoopIndex = value; }
	}

	/// <summary>Callback called on Awake if this Object is the Singleton's Instance.</summary>
   	protected override void OnAwake()
	{
		lastLoopIndex = -1;
	}

	/// <summary>Gets Loop's AudioSource on the given index.</summary>
	/// <param name="index">Index [0 by default].</param>
	/// <returns>Loop's AudioSource.</returns>
	public static AudioSource GetLoopSource(int index = 0)
	{
		AudioSource[] sources = Instance._loopSources;
		return sources != null ? sources[Mathf.Clamp(index, 0, sources.Length)] : null;
	}

	/// <summary>Gets Scenario's AudioSource on the given index.</summary>
	/// <param name="index">Index [0 by default].</param>
	/// <returns>Scenario's AudioSource.</returns>
	public static AudioSource GetScenarioSource(int index = 0)
	{
		AudioSource[] sources = Instance._scenarioSources;
		return sources != null ? sources[Mathf.Clamp(index, 0, sources.Length)] : null;
	}

	/// <summary>Gets Mateo's AudioSource on the given index.</summary>
	/// <param name="index">Index [0 by default].</param>
	/// <returns>Mateo's AudioSource.</returns>
	public static AudioSource GetSoundEffectSource(int index = 0)
	{
		AudioSource[] sources = Instance._soundEffectSources;
		return sources != null ? sources[Mathf.Clamp(index, 0, sources.Length)] : null;
	}

	/// <summary>Stops AudioSource, then assigns and plays AudioClip.</summary>
	/// <param name="_audioSource">AudioSource to play sound.</param>
	/// <param name="_aucioClip">AudioClip to play.</param>
	/// <param name="_loop">Loop AudioClip? false as default.</param>
	/// <returns>Playing Audioclip.</returns>
	public static AudioClip Play(AudioSource _source, int _index, bool _loop = false)
	{
		if(_index == lastLoopIndex) return null;

		AudioClip clip = Game.data.loops[_index];
		AudioMixer mixer = _source.outputAudioMixerGroup.audioMixer;

		if(mixer != null && lastLoopIndex > -1)
		{
			/// Fade-Out last piece -> Set new piece -> Fade-In new piece.
			Instance.StartCoroutine(mixer.FadeVolume(Instance.exposedVolumeParameterName, Instance.fadeOutDuration, 0.0f, 
			()=>
			{
				_source.PlaySound(clip, _loop);
				Instance.StartCoroutine(mixer.FadeVolume(Instance.exposedVolumeParameterName, Instance.fadeOutDuration, 1.0f, 
				()=>
				{
					Instance.DispatchCoroutine(ref Instance.volumeFading);
				}));

			}), ref Instance.volumeFading);
		}
		else _source.PlaySound(clip, _loop);

		lastLoopIndex = _index;

		return clip;
	}

	/// <summary>Stops default AudioSource, then assigns and plays AudioClip.</summary>
	/// <param name="_index">Index of AudioClip to play.</param>
	/// <param name="_loop">Loop AudioClip? false as default.</param>
	/// <returns>Playing Audioclip.</returns>
	public static AudioClip Play(int _index, bool _loop = false)
	{
		return Play(Instance.audioSource, _index, _loop);
	}

	/// <summary>Stops AudioSource, Fades-Out if there is an AudioMixer.</summary>
	/// <param name="_source">AudioSource to Stop.</param>
	/// <param name="onStopEnds">Optional callback invoked when the stop process reaches itrs end [null by default].</param>
	public static void Stop(AudioSource _source, Action onStopEnds = null)
	{
		if(_source.clip == null || !_source.isPlaying) return;

		AudioMixer mixer = _source.outputAudioMixerGroup.audioMixer;

		if(mixer != null)
		{
			Instance.StartCoroutine(mixer.FadeVolume(Instance.exposedVolumeParameterName, Instance.fadeOutDuration, 0.0f,
			()=>
			{
				_source.Stop();
				if(onStopEnds != null) onStopEnds();
			}));
		}
		else
		{
			_source.Stop();
			if(onStopEnds != null) onStopEnds();
		}
	}

	/// <summary>Stacks and plays AudioClip on the given AudioSource.</summary>
	/// <param name="_source">Source to use.</param>
	/// <param name="_indeex">AudioClip's index on the Game's Data to play.</param>
	/// <param name="_volumeScale">Normalized Volume's Scale [1.0f by default].</param>
	/// <returns>Playing Audioclip.</returns>
	public static AudioClip PlayOneShot(AudioSource _source, int _index, float _volumeScale = 1.0f)
	{
		AudioClip clip = Game.data.soundEffects[_index];
		_source.PlayOneShot(clip, _volumeScale);

		return clip;
	}

	/// <summary>Stacks and plays AudioClip on the default AudioSource.</summary>
	/// <param name="_indeex">AudioClip's index on the Game's Data to play.</param>
	/// <param name="_volumeScale">Normalized Volume's Scale [1.0f by default].</param>
	/// <returns>Playing Audioclip.</returns>
	public static AudioClip PlayOneShot(int _index, float _volumeScale = 1.0f)
	{
		return PlayOneShot(Instance.audioSource, _index, _volumeScale);
	}
}
}

/*

	Create timestamps, each timeStamp defines a state and the beginning of each state

	if(currentTime > timeStamp[i] && currentTime timeStamp[i + 1]) state = i;

	OnnRebeginState() {
		
		ContinueFromState(state);

	}

	//Play Audio on Editor:
	//https://answers.unity.com/questions/36388/how-to-play-audioclip-in-edit-mode.html#:~:text=The%20audio%20preview%20mode%20must,and%20your%20audio%20should%20work.

*/