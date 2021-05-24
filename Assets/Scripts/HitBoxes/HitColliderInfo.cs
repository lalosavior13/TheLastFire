using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[Serializable]
public class HitColliderInfo
{
	[SerializeField] private HitCollider2D _hitCollider; 	/// <summary>Hit Collider.</summary>
	[SerializeField] private float _damageScale; 			/// <summary>Damage Scale that this Hit Collider Applies.</summary>

	/// <summary>Gets and Sets hitCollider property.</summary>
	public HitCollider2D hitCollider
	{
		get { return _hitCollider; }
		set { _hitCollider = value; }
	}

	/// <summary>Gets and Sets damageScale property.</summary>
	public float damageScale
	{
		get { return _damageScale; }
		set { _damageScale = value; }
	}

	/// <summary>HitColliderInfo default constructor.</summary>
	/// <param name="_hitCollider">HitCollider.</param>
	/// <param name="_damageScale">Damage Scale that this HitColldier applies.</param>
	public HitColliderInfo(HitCollider2D _hitCollider, float _damageScale)
	{
		hitCollider = _hitCollider;
		damageScale = _damageScale;
	}
}
}