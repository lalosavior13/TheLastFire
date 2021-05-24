using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public enum RotationEvent
{
	BuildUpBegins,
	BuildingUp,
	BuildUpEnds,
	SwingBegins,
	Swinging,
	SwingEnds
}

/// <summary>Event invojked when a rotation event occurs.</summary>
/// <param name="_event">Type of rotation event.</param>
/// <param name="_ID">Optional Event's ID [0 by default].</param>
public delegate void OnRotationIDEvent(RotationEvent _event, int _ID = 0);

[Serializable]
public class RotationDataSet : IEnumerable<RotationData>
{
	public event OnRotationIDEvent onRotationEvent; 			/// <summary>nRotationIDEvent's delegate.</summary>

	[SerializeField] public RotationData[] _rotationDataSet; 	/// <summary>Set of Rotations' Data.</summary>

	/// <summary>Gets and Sets rotationDataSet property.</summary>
	public RotationData[] rotationDataSet
	{
		get { return _rotationDataSet; }
		set { _rotationDataSet = value; }
	}

	/// <returns>Returns an enumerator that iterates through the Rotations' data.</returns>
	public IEnumerator<RotationData> GetEnumerator()
	{
		foreach(RotationData data in rotationDataSet)
		{
			yield return data;
		}
	}

	/// <returns>Returns an enumerator that iterates through the Rotations' data.</returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		yield return GetEnumerator();
	}

	/// <summary>Invokes Rotation Event.</summary>
	/// <param name="_event">Type of Rotation Event.</param>
	/// <param name="_ID">Optional Event's ID [0 by default].</param>
	public void InvokeRotationEvent(RotationEvent _event, int _ID = 0)
	{
		if(onRotationEvent != null) onRotationEvent(_event, _ID);
	}

	/// <summary>Build-Up and Swings transform from given axis towards direction.</summary>
	/// <param name="_transform">Transform to rotate.</param>
	/// <param name="_rotationAxis">Axis of rotation [relative to the Transform].</param>
	/// <param name="_orientation">Orientation axis [relative to Transform] that must be aligned to the given Rotation Data Direction.</param>
	/// <param name="_dotProductTolerance">Additional Dot Product's Tolerance [0.0f by default].</param>
	/// <param name="onRotationEvent">Optional Callback invoked when a rotation event happens.</param>
	public IEnumerator BuildUpAndSwing(Transform _transform, Vector3 _rotationAxis, Vector3 _orientation, float _dotTolerance = 0.0f, Action<RotationEvent, int> onRotationEvent = null)
	{
		IEnumerator rotate = null;
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);
		float angularSpeed = 0.0f;
		float cooldownDuration = 0.0f;

		foreach(RotationData data in rotationDataSet)
		{
			angularSpeed = data.buildUpAngularSpeed;
			if(onRotationEvent != null) onRotationEvent(RotationEvent.BuildUpBegins, data.buildUpEventID);
			rotate = _transform.RotateOnAxisTowardsDirection(_rotationAxis, _orientation, data.buildUpDirection, angularSpeed, _dotTolerance);
			while(rotate.MoveNext())
			{
				if(onRotationEvent != null) onRotationEvent(RotationEvent.BuildingUp, data.buildUpEventID);
				yield return null;
			}
			if(onRotationEvent != null) onRotationEvent(RotationEvent.BuildUpEnds, data.buildUpEventID);

			cooldownDuration = data.buildUpCooldown;
			if(cooldownDuration > 0.0f)
			{
				wait.ChangeDurationAndReset(cooldownDuration);
				while(wait.MoveNext()) yield return null;
			}

			angularSpeed = data.swingAngularSpeed;
			if(onRotationEvent != null) onRotationEvent(RotationEvent.SwingBegins, data.swingEventID);
			rotate = _transform.RotateOnAxisTowardsDirection(_rotationAxis, _orientation, data.swingDirection, angularSpeed, _dotTolerance);
			while(rotate.MoveNext())
			{
				if(onRotationEvent != null) onRotationEvent(RotationEvent.Swinging, data.buildUpEventID);
				yield return null;
			}
			if(onRotationEvent != null) onRotationEvent(RotationEvent.SwingEnds, data.swingEventID);

			cooldownDuration = data.swingCooldown;
			if(cooldownDuration > 0.0f)
			{
				wait.ChangeDurationAndReset(cooldownDuration);
				while(wait.MoveNext()) yield return null;
			}
		}
	}

	/// <summary>Build-Up and Swings transform from given axis towards direction.</summary>
	/// <param name="_set">Array of RotationDataSets.</param>
	/// <param name="_transform">Transform to rotate.</param>
	/// <param name="_rotationAxis">Axis of rotation [relative to the Transform].</param>
	/// <param name="_orientation">Orientation axis [relative to Transform] that must be aligned to the given Rotation Data Direction.</param>
	/// <param name="_dotProductTolerance">Additional Dot Product's Tolerance [0.0f by default].</param>
	/// <param name="onRotationEvent">Optional Callback invoked when a rotation event happens.</param>
	public static IEnumerator BuildUpAndSwing(RotationDataSet[] _set, Transform _transform, Vector3 _rotationAxis, Vector3 _orientation, float _dotTolerance = 0.0f, Action<RotationEvent, int> onRotationEvent = null)
	{
		IEnumerator rotate = null;
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);
		float angularSpeed = 0.0f;
		float cooldownDuration = 0.0f;

		foreach(RotationDataSet dataSet in _set)
		{
			foreach(RotationData data in dataSet)
			{
				angularSpeed = data.buildUpAngularSpeed;
				if(onRotationEvent != null) onRotationEvent(RotationEvent.BuildUpBegins, data.buildUpEventID);
				rotate = _transform.RotateOnAxisTowardsDirection(_rotationAxis, _orientation, data.buildUpDirection, angularSpeed, _dotTolerance);
				while(rotate.MoveNext())
				{
					if(onRotationEvent != null) onRotationEvent(RotationEvent.BuildingUp, data.buildUpEventID);
					yield return null;
				}
				if(onRotationEvent != null) onRotationEvent(RotationEvent.BuildUpEnds, data.buildUpEventID);

				cooldownDuration = data.buildUpCooldown;
				if(cooldownDuration > 0.0f)
				{
					wait.ChangeDurationAndReset(cooldownDuration);
					while(wait.MoveNext()) yield return null;
				}

				angularSpeed = data.swingAngularSpeed;
				if(onRotationEvent != null) onRotationEvent(RotationEvent.SwingBegins, data.swingEventID);
				rotate = _transform.RotateOnAxisTowardsDirection(_rotationAxis, _orientation, data.swingDirection, angularSpeed, _dotTolerance);
				while(rotate.MoveNext())
				{
					if(onRotationEvent != null) onRotationEvent(RotationEvent.Swinging, data.swingEventID);
					yield return null;
				}
				if(onRotationEvent != null) onRotationEvent(RotationEvent.SwingEnds, data.swingEventID);

				cooldownDuration = data.swingCooldown;
				if(cooldownDuration > 0.0f)
				{
					wait.ChangeDurationAndReset(cooldownDuration);
					while(wait.MoveNext()) yield return null;
				}			
			}
		}	
	}
}
}