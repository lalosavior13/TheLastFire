using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class SurfacesDebugger : MonoBehaviour
{
	[SerializeField] private LayerMask _surfacesMask; 		/// <summary>Surface's LayerMask.</summary>
	private List<CircleCollider2D> _circleColliders; 		/// <summary>CircleColliders.</summary>
	private List<BoxCollider2D> _boxColliders; 				/// <summary>BoxColliders.</summary>
	private List<PolygonCollider2D> _polygonColliders; 		/// <summary>PolygonColliders.</summary>
	private List<EdgeCollider2D> _edgeColliders; 			/// <summary>EdgeColliders.</summary>
	private List<CapsuleCollider2D> _capsuleColliders; 		/// <summary>CapsuleColliders.</summary>
	private List<CompositeCollider2D> _compositeColliders; 	/// <summary>CompositeColliders.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets surfacesMask property.</summary>
	public LayerMask surfacesMask
	{
		get { return _surfacesMask; }
		set { _surfacesMask = value; }
	}

	/// <summary>Gets and Sets circleColliders property.</summary>
	public List<CircleCollider2D> circleColliders
	{
		get { return _circleColliders; }
		private set { _circleColliders = value; }
	}

	/// <summary>Gets and Sets boxColliders property.</summary>
	public List<BoxCollider2D> boxColliders
	{
		get { return _boxColliders; }
		private set { _boxColliders = value; }
	}

	/// <summary>Gets and Sets polygonColliders property.</summary>
	public List<PolygonCollider2D> polygonColliders
	{
		get { return _polygonColliders; }
		private set { _polygonColliders = value; }
	}

	/// <summary>Gets and Sets edgeColliders property.</summary>
	public List<EdgeCollider2D> edgeColliders
	{
		get { return _edgeColliders; }
		private set { _edgeColliders = value; }
	}

	/// <summary>Gets and Sets capsuleColliders property.</summary>
	public List<CapsuleCollider2D> capsuleColliders
	{
		get { return _capsuleColliders; }
		private set { _capsuleColliders = value; }
	}

	/// <summary>Gets and Sets compositeColliders property.</summary>
	public List<CompositeCollider2D> compositeColliders
	{
		get { return _compositeColliders; }
		private set { _compositeColliders = value; }
	}
#endregion

	/// <summary>SurfacesDebugger's instance initialization.</summary>
	private void Awake()
	{
		PopulateLists();
	}
	
	/// <summary>SurfacesDebugger's tick at each frame.</summary>
	private void Update ()
	{
		UpdateBoxColliders();
	}

	/// <summary>Populates Lists.</summary>
	public void PopulateLists()
	{
		GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
		int layer = 0;
		Collider2D[] colliders = null;
		CircleCollider2D circleCollider = null;
		BoxCollider2D boxCollider = null;
		PolygonCollider2D polygonCollider = null;
		EdgeCollider2D edgeCollider = null;
		CapsuleCollider2D capsuleCollider = null;
		CompositeCollider2D compositeCollider = null;

		circleColliders = new List<CircleCollider2D>();
		boxColliders = new List<BoxCollider2D>();
		polygonColliders = new List<PolygonCollider2D>();
		edgeColliders = new List<EdgeCollider2D>();
		capsuleColliders = new List<CapsuleCollider2D>();
		compositeColliders = new List<CompositeCollider2D>();

		foreach(GameObject obj in objects)
		{
			layer = 1 << obj.layer;

			if((surfacesMask | layer) != surfacesMask) continue;

			colliders = obj.GetComponents<Collider2D>();

			if(colliders == null || colliders.Length == 0) continue;

			foreach(Collider2D collider  in colliders)
			{
				circleCollider = collider as CircleCollider2D;
				if(circleCollider != null)
				{
					circleColliders.Add(circleCollider);
					continue;
				}

				boxCollider = collider as BoxCollider2D;
				if(boxCollider != null)
				{
					boxColliders.Add(boxCollider);
					continue;
				}

				polygonCollider = collider as PolygonCollider2D;
				if(polygonCollider != null)
				{
					polygonColliders.Add(polygonCollider);
					continue;
				}

				edgeCollider = collider as EdgeCollider2D;
				if(edgeCollider != null)
				{
					edgeColliders.Add(edgeCollider);
					continue;
				}

				capsuleCollider = collider as CapsuleCollider2D;
				if(capsuleCollider != null)
				{
					capsuleColliders.Add(capsuleCollider);
					continue;
				}

				compositeCollider = collider as CompositeCollider2D;
				if(compositeCollider != null)
				{
					compositeColliders.Add(compositeCollider);
					continue;
				}
			}
		}
	}

	/// <summary>Updates BoxColliders.</summary>
	private void UpdateBoxColliders()
	{
		if(boxColliders == null) return;

		Vector2 a = Vector2.zero;
		Vector2 b = Vector2.zero;
		Vector2 c = Vector2.zero;
		Vector2 d = Vector2.zero;
		Vector2 min = Vector2.zero;
		Vector2 max = Vector2.zero;
		Vector2 position = Vector2.zero;
		Vector2 size = Vector2.zero;
		Vector2 extents = Vector2.zero;
		Vector2[] normals1 = null;
		Vector2[] normals2 = null;
		Quaternion rotation = Quaternion.identity;

		foreach(BoxCollider2D boxCollider in boxColliders)
		{
			position = boxCollider.transform.position;
			rotation = boxCollider.transform.rotation;
			size = boxCollider.size;
			extents = size * 0.5f;
			min = rotation * ((position + boxCollider.offset) - extents);
			min = rotation * ((position + boxCollider.offset) + extents);
			a = new Vector2(min.x, min.y);
			b = new Vector2(min.x, max.y);
			c = new Vector2(max.x, min.y);
			d = new Vector2(max.x, max.y);
			normals1 = new Vector2[4];
			normals2 = new Vector2[4];
		}
	}
}
}