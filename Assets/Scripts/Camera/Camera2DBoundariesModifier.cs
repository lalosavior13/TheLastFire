using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(Boundaries2DContainer))]
[RequireComponent(typeof(BoxCollider2D))]
//[ExecuteInEditMode]
public class Camera2DBoundariesModifier : MonoBehaviour
{
	protected static HashSet<Camera2DBoundariesModifier> boundariesModifiers; 	/// <summary>Boundaries' Modifiers.</summary>

	[SerializeField] private GameObjectTag _playerTag; 							/// <summary>Player's Tag.</summary>
	private Boundaries2DContainer _boundariesContainer; 						/// <summary>Boundaries2DContainer's Component.</summary>
	private BoxCollider2D _boxCollider; 										/// <summary>BoxCollider2D's Component.</summary>

	/// <summary>Gets and Sets playerTag property.</summary>
	public GameObjectTag playerTag
	{
		get { return _playerTag; }
		private set { _playerTag = value; }
	}

	/// <summary>Gets boundariesContainer Component.</summary>
	public Boundaries2DContainer boundariesContainer
	{ 
		get
		{
			if(_boundariesContainer == null) _boundariesContainer = GetComponent<Boundaries2DContainer>();
			return _boundariesContainer;
		}
	}

	/// <summary>Gets boxCollider Component.</summary>
	public BoxCollider2D boxCollider
	{ 
		get
		{
			if(_boxCollider == null) _boxCollider = GetComponent<BoxCollider2D>();
			return _boxCollider;
		}
	}

	/// <summary>Camera2DBoundariesModifier's instance initialization when loaded [Before scene loads].</summary>
	private void Awake()
	{
		if(boundariesModifiers == null) boundariesModifiers = new HashSet<Camera2DBoundariesModifier>();
	}

	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		if(!Application.isPlaying)
		UpdateBoxCollider();
	}

	/// <summary>Resets Camera2DBoundariesModifier's instance to its default values.</summary>
	private void Reset()
	{
		boundariesContainer.space = Space.Self;
		boxCollider.isTrigger = true;
		playerTag = Game.data.playerTag;
	}

	/// <summary>Updates BoxCollider2D.</summary>
	private void UpdateBoxCollider()
	{
		boxCollider.size = boundariesContainer.size;
	}

	/// <summary>Event triggered when this Collider2D enters another Collider2D trigger.</summary>
	/// <param name="col">The other Collider2D involved in this Event.</param>
	private void OnTriggerEnter2D(Collider2D col)
	{
		GameObject obj = col.gameObject;
	
		if(obj.CompareTag(playerTag))
		{
			Boundaries2DContainer cameraBoundariesContainer = Game.cameraController.boundariesContainer;

			if(cameraBoundariesContainer == null) return;

			cameraBoundariesContainer.center = boundariesContainer.center;
			cameraBoundariesContainer.size = boundariesContainer.size;
			boundariesModifiers.Add(this);
		}
	}

	/// <summary>Event triggered when this Collider2D exits another Collider2D trigger.</summary>
	/// <param name="col">The other Collider2D involved in this Event.</param>
	private void OnTriggerExit2D(Collider2D col)
	{
		GameObject obj = col.gameObject;
	
		if(obj.CompareTag(playerTag))
		{
			Boundaries2DContainer cameraBoundariesContainer = Game.cameraController.boundariesContainer;

			if(cameraBoundariesContainer == null) return;

			if(boundariesModifiers.Contains(this)) boundariesModifiers.Remove(this);

			if(boundariesModifiers.Count == 0)
			{
				Game.SetDefaultCameraBoundaries2D();
				return;
			}

			Boundaries2DContainer boundaries = boundariesModifiers.First().boundariesContainer;

			cameraBoundariesContainer.InterpolateTowards(boundaries.ToBoundaries2D());
		}
	}
}
}