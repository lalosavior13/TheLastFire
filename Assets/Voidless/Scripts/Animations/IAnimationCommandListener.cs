using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public interface IAnimationCommandListener : IAnimationStateListener
{
	/// <summary>Callback invoked when a command setup begins.</summary>
    /// <param name="_animator">Animator's reference.</param>
    /// <param name="_stateInfo">AnimationState's Information.</param>
    /// <param name="_layerID">Layer's Index on the State Machine.</param>
    void OnStartup(Animator _animator, AnimatorStateInfo _stateInfo, int _layerID);

    /// <summary>Callback invoked whan a command activation begins.</summary>
    /// <param name="_animator">Animator's reference.</param>
    /// <param name="_stateInfo">AnimationState's Information.</param>
    /// <param name="_layerID">Layer's Index on the State Machine.</param>
    void OnActive(Animator _animator, AnimatorStateInfo _stateInfo, int _layerID);

    /// <summary>Callback invoked when a command recovery begins.</summary>
    /// <param name="_animator">Animator's reference.</param>
    /// <param name="_stateInfo">AnimationState's Information.</param>
    /// <param name="_layerID">Layer's Index on the State Machine.</param>
    void OnRecovery(Animator _animator, AnimatorStateInfo _stateInfo, int _layerID);

    /// <summary>Callback invoked when [and if] the activation window begins.</summary>
    /// <param name="_duration">Window's Duration.</param>
    void OnAdditionalWindow(float _duration);

    /// <summary>Callback invoked when an animation begins.</summary>
    /// <param name="_animator">Animator's reference.</param>
    /// <param name="_startUpPercentage">Startup's Percentage.</param>
    /// <param name="_activePercentage">Active's Percentage.</param>
    /// <param name="_recoveryPercentage">Recovery's Percentage.</param>
    /// <param name="_additionalWindow">Additional Window's duration [0.0f by default].</param>
    /// <param name="_layerID">Layer's Index on the State Machine [0 by default].</param>
    void OnAnimationCommandEnter(Animator _animator, float _startUpPercentage, float _activePercentage, float _recoveryPercentage, float _additionalWindow = 0.0f, int _layerID = 0);
}
}