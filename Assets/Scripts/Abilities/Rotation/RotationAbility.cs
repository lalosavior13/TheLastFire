using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class RotationAbility : MonoBehaviour
{
	[SerializeField] private float _speed; 		/// <summary>Rotation's Speed.</summary>

	/// <summary>Gets and Sets speed property.</summary>
	public float speed
	{
		get { return _speed; }
		set { _speed = value; }
	}

#region TowardsTarget:
	/// <summary>Rotates towards given target.</summary>
	/// <param name="_transform">Transform to rotate.</param>
	/// <param name="_target">Target.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public void RotateTowardsTarget(Transform _transform, Vector3 _target, float _scale = 1.0f)
	{
		_transform.rotation = CalculateRotation(_transform.rotation, _target - _transform.position, Time.deltaTime, _scale);
	}

	/// <summary>Rotates towards given target.</summary>
	/// <param name="_animator">Animator to rotate.</param>
	/// <param name="_target">Target.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public void RotateTowardsTarget(Animator _animator, Vector3 _target, float _scale = 1.0f)
	{
		_animator.bodyRotation = CalculateRotation(_animator.bodyRotation, _target - _animator.transform.position, Time.deltaTime, _scale);
	}

	/// <summary>Rotates towards given target.</summary>
	/// <param name="_target">Target.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public void RotateTowardsTarget(Vector3 _target, float _scale = 1.0f)
	{
		RotateTowardsDirection(transform, _target, _scale);
	}
#endregion

#region TowardsDirection:
	/// <summary>Rotates Towards given direction.</summary>
	/// <param name="_transform">Transform to rotate.</param>
	/// <param name="direction">Direction Towards Target.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public virtual void RotateTowardsDirection(Transform _transform, Vector3 _direction, float _scale = 1.0f)
	{
		_transform.rotation = CalculateRotation(_transform.rotation, _direction, Time.deltaTime, _scale);
	}

	/// <summary>Rotates Animator Towards given direction.</summary>
	/// <param name="_animator">Animator to rotate.</param>
	/// <param name="direction">Direction Towards Target.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public virtual void RotateTowardsDirection(Animator _animator, Vector3 _direction, float _scale = 1.0f)
	{
		_animator.bodyRotation = CalculateRotation(_animator.bodyRotation, _direction, Time.deltaTime, _scale);
	}

	/// <summary>Rotates Towards given direction.</summary>
	/// <param name="direction">Direction Towards Target.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public virtual void RotateTowardsDirection(Vector3 _direction, float _scale = 1.0f)
	{
		RotateTowardsDirection(transform, _direction, _scale);
	}
#endregion

#region TowardsRotation:
	/// <summary>Rotates towards given rotation.</summary>
	/// <param name="_transform">Transform to rotate.</param>
	/// <param name="_rotation">Target's rotation.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public virtual void RotateTowards(Transform _transform, Quaternion _rotation, float _scale = 1.0f)
	{
		_transform.rotation = CalculateRotation(_transform.rotation, _rotation, Time.deltaTime, _scale);
	}

	/// <summary>Rotates Animator towards given rotation.</summary>
	/// <param name="_transform">Transform to rotate.</param>
	/// <param name="_rotation">Target's rotation.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public virtual void RotateTowards(Animator _animator, Quaternion _rotation, float _scale = 1.0f)
	{
		_animator.bodyRotation = CalculateRotation(_animator.bodyRotation, _rotation, Time.deltaTime, _scale);
	}

	/// <summary>Rotates towards given rotation.</summary>
	/// <param name="_rotation">Target's rotation.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	public virtual void RotateTowards(Quaternion _rotation, float _scale = 1.0f)
	{
		RotateTowards(transform, _rotation, _scale);
	}
#endregion

	/// <summary>Calculates ideal rotation.</summary>
	/// <param name="_rotation">Transform's Rotation.</param>
	/// <param name="direction">Rotation's Direction.</param>
	/// <param name="deltaTime">Delta Time's Value.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	/// <returns>Rotation Vector.</returns>
	protected virtual Quaternion CalculateRotation(Quaternion _rotation, Vector3 direction, float deltaTime, float scale = 1.0f)
	{
		direction.Normalize();

		Quaternion rotation = Quaternion.LookRotation(direction);

		return CalculateRotation(_rotation, rotation, deltaTime, scale);
	}

	/// <summary>Calculates ideal rotation.</summary>
	/// <param name="_transformRotation">Transform's Rotation.</param>
	/// <param name="rotation">Target's Rotation.</param>
	/// <param name="deltaTime">Delta Time's Value.</param>
	/// <param name="scale">Additional scalar [1.0f by default].</param>
	/// <returns>Rotation Vector.</returns>
	protected virtual Quaternion CalculateRotation(Quaternion _transformRotation, Quaternion rotation, float deltaTime, float scale = 1.0f)
	{
		return Quaternion.RotateTowards(_transformRotation, rotation, deltaTime * speed * scale);
	}
}
}