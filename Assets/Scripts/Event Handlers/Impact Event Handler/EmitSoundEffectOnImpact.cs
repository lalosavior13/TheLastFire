using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(ImpactEventHandler))]
public class EmitSoundEffectOnImpact : ImpactEventListener
{
	[SerializeField] private MaterialIPIndexPair[] _materialIDindexPairs; 	/// <summary>Pairs of MaterialID and index tuples.</summary>
	[SerializeField] private CollectionIndex _soundEffectIndex; 	/// <summary>Particle Effect's Index on the Game's Data.</summary>

	/// <summary>Gets and Sets materialIDindexPairs property.</summary>
	public MaterialIPIndexPair[] materialIDindexPairs
	{
		get { return _materialIDindexPairs; }
		set { _materialIDindexPairs = value; }
	}

	/// <summary>Gets and Sets soundEffectIndex property.</summary>
	public CollectionIndex soundEffectIndex
	{
		get { return _soundEffectIndex; }
		set { _soundEffectIndex = value; }
	}

	/// <summary>Callback invoked when an impact is received.</summary>
	/// <param name="_info">Trigger2D's Information.</param>
	protected override void OnImpactEvent(Trigger2DInformation _info)
	{
		int index = soundEffectIndex;
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

		AudioController.PlayOneShot(index);
	}
}
}