using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
/// <summary>Event invoked when a Collider passes a ring.</summary>
/// <param name="_collider">Collider that passed the ring.</param>
public delegate void OnRingPassed(Collider2D _collider);

public class Ring : PoolGameObject
{
	public event OnRingPassed onRingPassed; 						/// <summary>OnRingPassed event's delegate.</summary>

	[SerializeField] private EulerRotation _correctionRotation; 	/// <summary>Correction's Rotation.</summary>
	[SerializeField] private bool _deactivateWhenPassedOn; 			/// <summary>Deactivate ring when passed on it?.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _dotProduct; 					/// <summary>Minimium Dot Product's value to be considered passed.</summary>
	[SerializeField] private Renderer _renderer; 					/// <summary>Ring Mesh's Renderer.</summary>
	[SerializeField] private HitCollider2D[] _hitBoxes; 			/// <summary>Ring's HitBoxes.</summary>
	private Dictionary<int, Vector2> _directionsMapping; 			/// <summary>Direction's Mapping for each possible GameObject.</summary>

	/// <summary>Gets correctionRotation property.</summary>
	public EulerRotation correctionRotation { get { return _correctionRotation; } }

	/// <summary>Gets and Sets deactivateWhenPassedOn property.</summary>
	public bool deactivateWhenPassedOn
	{
		get { return _deactivateWhenPassedOn; }
		set { _deactivateWhenPassedOn = value; }
	}

	/// <summary>Gets and Sets dotProduct property.</summary>
	public float dotProduct
	{
		get { return _dotProduct; }
		set { _dotProduct = value; }
	}

	/// <summary>Gets renderer property.</summary>
	public Renderer renderer { get { return _renderer; } }

	/// <summary>Gets hitBoxes property.</summary>
	public HitCollider2D[] hitBoxes { get { return _hitBoxes; } }

	/// <summary>Gets and Sets directionsMapping property.</summary>
	public Dictionary<int, Vector2> directionsMapping
	{
		get { return _directionsMapping; }
		private set { _directionsMapping = value; }
	}

	/// <summary>Draws Gizmos on Editor mode when Ring's instance is selected.</summary>
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, (transform.rotation * correctionRotation) * Vector3.right);
	}

	/// <summary>Ring's instance initialization.</summary>
	private void Awake()
	{
		directionsMapping = new Dictionary<int, Vector2>();

		if(hitBoxes != null)
		{
			int i = 0;
			foreach(HitCollider2D hitBox in hitBoxes)
			{
				hitBox.onTriggerEvent2D += OnTriggerEvent2D;
				hitBox.ID = i;
				i++;
			}
		}
	}

	/// <summary>Callback invoked when Ring's instance is going to be destroyed and passed to the Garbage Collector.</summary>
	private void OnDestroy()
	{
		if(hitBoxes != null) foreach(HitCollider2D hitBox in hitBoxes)
		{
			hitBox.onTriggerEvent2D -= OnTriggerEvent2D;
		}
	}

	/// \TODO REPAIR IT (But it works for the sake of quick example...)
	/// <summary>Callback invoked when this Hit Collider2D intersects with another GameObject.</summary>
	/// <param name="_collider">Collider2D that was involved on the Hit Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_hitColliderID">Optional ID of the HitCollider2D.</param>
	public void OnTriggerEvent2D(Collider2D _collider, HitColliderEventTypes _eventType, int _hitColliderID = 0)
	{
		GameObject obj = _collider.gameObject;
		int instanceID = obj.GetInstanceID();

		switch(_eventType)
		{
			case HitColliderEventTypes.Enter:
			if(!directionsMapping.ContainsKey(instanceID))
			{
				/*Vector3 closestPoint = hitBoxes[_hitColliderID].collider.bounds.ClosestPoint(obj.transform.position);
				Vector2 direction = closestPoint - transform.position;*/
				Vector2 direction = transform.position - obj.transform.position;
				direction = ToRelativeOrientationVector(direction);
				directionsMapping.Add(instanceID, direction);
			}
			break;

			case HitColliderEventTypes.Exit:
			if(directionsMapping.ContainsKey(instanceID))
			{
				Vector2 direction = (obj.transform.position - transform.position);
				direction = ToRelativeOrientationVector(direction);
				if(Vector2.Dot(direction, directionsMapping[instanceID]) >= dotProduct)
				{
					Debug.Log("[Ring] Success, the GameObject passed correctly");
					InvokeRingPassedEvent(_collider);
				}
				else Debug.Log("[Ring] Failure, you are mom gay.");

				/*Debug.DrawRay(transform.position, direction, Color.cyan, 3.0f);
				Debug.DrawRay(transform.position, directionsMapping[instanceID], Color.magenta, 3.0f);*/

				directionsMapping.Remove(instanceID);
			}
			break;
		}
	}

	/// <summary>Transforms local space vector into world space.</summary>
	/// <param name="v">Vector to convert.</param>
	/// <returns>Converted Vector.</returns>
	private Vector2 ToRelativeOrientationVector(Vector2 v)
	{
		Vector2 inverseVector = Quaternion.Inverse(transform.rotation * correctionRotation) * v;
		return inverseVector.x < 0.0f ? Vector2.left : Vector2.right;
	}

	/// <summary>Straightens given Vector.</summary>
	/// <param name="v">Vector to straighten.</param>
	/// <returns>Straightened vector.</returns>
	private Vector2 StraightDirection(Vector2 v)
	{
		float xSign = Mathf.Sign(v.x);
		float ySign = Mathf.Sign(v.y);
		
		v = Vector2.Scale(v, transform.right.Abs());

		return new Vector2(
			v.x > v.y ? (1.0f * xSign) : 0.0f,
			v.y > v.x ? (1.0f * ySign) : 0.0f
		);
	}

	/// <summary>Invokes onRingPassed's Event.</summary>
	/// <param name="_collider">Collider that passed the ring.</param>
	private void InvokeRingPassedEvent(Collider2D _collider)
	{
		if(deactivateWhenPassedOn) OnObjectDeactivation();
		if(onRingPassed != null) onRingPassed(_collider);
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		if(directionsMapping != null) directionsMapping.Clear();
	}
}
}