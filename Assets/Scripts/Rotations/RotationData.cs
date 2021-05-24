using System;
using Voidless;

namespace Flamingo
{
[Serializable]
public struct RotationData
{
	public NormalizedVector3 buildUpDirection; 	/// <summary>Build-Up's Direction.</summary>
	public NormalizedVector3 swingDirection; 	/// <summary>Slash's Direction.</summary>
	public float buildUpAngularSpeed; 			/// <summary>Nuild-Up's Angular Speed.</summary>
	public float swingAngularSpeed; 			/// <summary>Slash's Angular Speed.</summary>
	public float buildUpCooldown; 				/// <summary>[Optional] Cooldown Duration after the build-up.</summary>
	public float swingCooldown; 				/// <summary>[Optional] Cooldown Duration after the swing.</summary>
	public int buildUpEventID; 					/// <summary>Event's ID invoked when the Build-Up rotation is done.</summary>
	public int swingEventID; 					/// <summary>Event's ID invoked when the Swing rotation is done.</summary>
}
}