using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[Serializable]
public struct MaterialIPIndexPair
{
	public int materialID;
	public CollectionIndex index;
}

[RequireComponent(typeof(ImpactEventHandler))]
public class EmitParticleEffectOnImpact : ImpactEventListener
{
	[SerializeField] private MaterialIPIndexPair[] _materialIDindexPairs; 	/// <summary>Pairs of MaterialID and index tuples.</summary>
	[SerializeField] private CollectionIndex _particleEffectIndex; 	/// <summary>Particle Effect's Index on the Game's Data.</summary>

	/// <summary>Gets and Sets materialIDindexPairs property.</summary>
	public MaterialIPIndexPair[] materialIDindexPairs
	{
		get { return _materialIDindexPairs; }
		set { _materialIDindexPairs = value; }
	}

	/// <summary>Gets and Sets particleEffectIndex property.</summary>
	public CollectionIndex particleEffectIndex
	{
		get { return _particleEffectIndex; }
		set { _particleEffectIndex = value; }
	}

	/// <summary>Callback invoked when an impact is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	protected override void OnImpactEvent(Trigger2DInformation _info)
	{
		Vector3 point = _info.contactPoint;
		Vector3 direction = transform.position - point;
		int index = particleEffectIndex;
		InteractableMaterial material = _info.collider.GetComponent<InteractableMaterial>();

		if(material != null && (materialIDindexPairs != null && materialIDindexPairs.Length > 0))
		foreach(MaterialIPIndexPair pair in materialIDindexPairs)
		{
			if(material.ID == pair.materialID)
			{
				index = pair.index;
				break;
			}
		}

		ParticleEffect particleEffect = PoolManager.RequestParticleEffect(index, point, VQuaternion.RightLookRotation(direction));
	}
}
}