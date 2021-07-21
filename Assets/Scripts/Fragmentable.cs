using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class Fragmentable : MonoBehaviour
{
	[SerializeField] private HitCollider2D[] _pieces; 		/// <summary>Fragmentable's Pieces.</summary>
	[SerializeField] private float _fragmentationDuration; 	/// <summary>Fragmentation's Duration.</summary>
	private TransformData[] _piecesData; 					/// <summary>Fragmentable Pieces' Bake data.</summary>
	private bool _fragmented; 								/// <summary>Is the object fragmented?.</summary>
	private Coroutine defragmentation; 						/// <summary>Defragmentation's Coroutine reference.</summary>

	/// <summary>Gets and Sets pieces property.</summary>
	public HitCollider2D[] pieces
	{
		get { return _pieces; }
		set { _pieces = value; }
	}

	/// <summary>Gets and Sets piecesData property.</summary>
	public TransformData[] piecesData
	{
		get { return _piecesData; }
		set { _piecesData = value; }
	}

	/// <summary>Gets and Sets fragmentationDuration property.</summary>
	public float fragmentationDuration
	{
		get { return _fragmentationDuration; }
		set { _fragmentationDuration = value; }
	}

	/// <summary>Gets and Sets fragmented property.</summary>
	public bool fragmented
	{
		get { return _fragmented; }
		set { _fragmented = value; }
	}

	/// <summary>Callback invoked when Fragmentable's instance is disabled.</summary>
	private void OnDisable()
	{
		//Defragmentate();
	}

	/// <summary>Fragmentable's instance initialization.</summary>
	private void Awake()
	{
		/*BakePiecesData();

		if(pieces != null)
		foreach(HitCollider2D piece in pieces)
		{
			piece.onTriggerEvent2D += OnTriggerEvent2D;
		}*/
	}

	/// <summary>Bakes Pieces' Data.</summary>
	public void BakePiecesData()
	{
		if(pieces == null) return;

		int length = pieces.Length;

		if(piecesData == null || piecesData.Length != length) piecesData = new TransformData[length];

		for(int i = 0; i < length; i++)
		{
			piecesData[i] = pieces[i].transform;
			pieces[i].transform.parent = transform;
			pieces[i].ID = i;
		}

		fragmented = true;
	}

	/// <summary>Fragmentates Pieces.</summary>
	public void Fragmentate(Vector3 _force, Action onFragmentationEnds = null)
	{
		if(!fragmented) return;

		this.StartCoroutine(Fragmentation(_force, onFragmentationEnds), ref defragmentation);
		fragmented = true;
	}

	/// <summary>Gathers pieces into their baked data.</summary>
	public void Defragmentate()
	{
		if(!fragmented) return;

		TransformData pieceData = default(TransformData);
		int i = 0;

		foreach(HitCollider2D piece in pieces)
		{
			if(piece != null)
			{
				pieceData = piecesData[i];
				piece.transform.position = pieceData.position;
				piece.transform.rotation = pieceData.rotation;
				piece.transform.localScale = pieceData.scale;
				piece.transform.parent = transform;
				piece.rigidbody.SetForTrigger();
			}

			i++;
		}
	}

	/// <summary>Gathers pieces into their baked data at given duration.</summary>
	/// <param name="_duration">Defragmentation's Duration.</param>
	/// <param name="onDefragmentationEnds">Optional callback invoked when the defragmentation ends [null by default].</param>
	public void Defragmentate(float _duration, Action onDefragmentationEnds = null)
	{
		/*if(fragmented)
		this.StartCoroutine(Defragmentation(_duration, onDefragmentationEnds), ref defragmentation);*/
	}

	/// <summary>Callback invoked when this Hit Collider2D intersects with another GameObject.</summary>
	/// <param name="_collider">Collider2D that was involved on the Hit Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_hitColliderID">Optional ID of the HitCollider2D.</param>
	private void OnTriggerEvent2D(Collider2D _collider, HitColliderEventTypes _eventType, int _hitColliderID = 0)
	{
		Trigger2DInformation info = new Trigger2DInformation(pieces[_hitColliderID].collider, _collider);
		//Fragmentate(pieces[_hitColliderID].transform.position - info.contactPoint);
	}	

	/// <summary>Fragments pieces for some time.</summary>
	private IEnumerator Fragmentation(Vector3 f, Action onFragmentationEnds = null)
	{
		float t = 0.0f;
		float inverseDuration = 1.0f / fragmentationDuration;
		Rigidbody2D body = null;
		SecondsDelayWait wait = new SecondsDelayWait(fragmentationDuration);

		foreach(HitCollider2D piece in pieces)
		{
			body = piece.rigidbody;
			body.transform.parent = null;
			body.SetForDynamicBody();
			body.AddForce(f);

		}

		while(wait.MoveNext()) yield return null;

		foreach(HitCollider2D piece in pieces)
		{
			piece.gameObject.SetActive(false);
		}

		gameObject.SetActive(false);

		if(onFragmentationEnds != null) onFragmentationEnds();
	}

	/// <summary>Gathers pieces into their baked data at given duration.</summary>
	/// <param name="_duration">Defragmentation's Duration.</param>
	/// <param name="onDefragmentationEnds">Optional callback invoked when the defragmentation ends [null by default].</param>
	private IEnumerator Defragmentation(float _duration, Action onDefragmentationEnds = null)
	{
		int length = pieces.Length;
		int i = 0;
		TransformData[] currentPiecesData = new TransformData[length];
		TransformData pieceData = default(TransformData);
		TransformData currentData = default(TransformData);
		float t = 0.0f;
		float inverseDuration = 1.0f / _duration;

		for(i = 0; i < length; i++)
		{
			if(pieces[i] != null) currentPiecesData[i] = pieces[i].transform;
		}

		i = 0;

		while(t < 1.0f)
		{
			foreach(HitCollider2D piece in pieces)
			{
				if(piece != null)
				{
					pieceData = piecesData[i];
					currentData = currentPiecesData[i];

					piece.transform.position = Vector3.Lerp(currentData.position, pieceData.position, t);
					piece.transform.rotation = Quaternion.Lerp(currentData.rotation, pieceData.rotation, t);
					piece.transform.localScale = Vector3.Lerp(currentData.localScale, pieceData.scale, t);
				}

				i++;
			}
			t += (Time.deltaTime * inverseDuration);
			yield return null;
		}

		Defragmentate();
		if(onDefragmentationEnds != null) onDefragmentationEnds();
	}
}
}