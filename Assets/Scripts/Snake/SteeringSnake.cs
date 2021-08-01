using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

/// \TODO CLEAN THIS FUCKING MESS!!!!!
namespace Flamingo
{
public class SteeringSnake : MonoBehaviour
{
	[SerializeField] private float _duration; 				/// <summary>Duration.</summary>
	private float _time; 									/// <summary>Current Time.</summary>
	private LinkedList<Projectile> _projectilesList; 	/// <summary>Projectiles' LinkedList.</summary>
	private List<Projectile> _projectiles; 			/// <summary>Projectiles' List.</summary>
	private Func<Vector2> _targetFunction; 					/// <summary>Target's Function.</summary>

	/// <summary>Gets and Sets duration property.</summary>
	public float duration
	{
		get { return _duration; }
		set { _duration = value; }
	}

	/// <summary>Gets and Sets time property.</summary>
	public float time
	{
		get { return _time; }
		protected set { _time = value; }
	}

	/// <summary>Gets and Sets projectilesList property.</summary>
	public LinkedList<Projectile> projectilesList
	{
		get { return _projectilesList; }
		protected set { _projectilesList = value; }
	}

	/// <summary>Gets and Sets projectiles property.</summary>
	public List<Projectile> projectiles
	{
		get { return _projectiles; }
		set { _projectiles = value; }
	}

	/// <summary>Gets and Sets targetFunction property.</summary>
	public Func<Vector2> targetFunction
	{
		get { return _targetFunction; }
		set { _targetFunction = value; }
	}

	/// <summary>SteeringSnake's instance initialization when loaded [Before scene loads].</summary>
	private void Awake()
	{
		projectilesList = new LinkedList<Projectile>();
	}
	
	/// <summary>Snake's tick at each frame.</summary>
	private void Update ()
	{
		if(projectilesList == null || projectilesList.Count == 0) return;

		time += Time.deltaTime;
		if(time >= duration) SetLastPositions();
	}

	/// <summary>Initializes Steering Snake's Behavior.</summary>
	/// <param name="_projectiles">New set of projectiles that will compose the Steering Snake.</param>
	public void InitializeLinkedList(Func<Vector2> function, params Projectile[] _projectiles)
	{
		if(_projectiles == null) return;

		projectiles = _projectiles.ToList();
		projectilesList = new LinkedList<Projectile>(_projectiles);
		targetFunction = function;
		Projectile p = null;

		foreach(Projectile projectile in projectilesList)
		{
			if(projectile != projectilesList.First.Value)
			{
				projectile.target = projectilesList.First.Value.GetPosition;
				projectile.parentProjectile = p;
			}
			else
			{
				projectile.target = function;
				projectile.parentProjectile = null;
			}
			
			projectile.onPoolObjectDeactivation += OnProjectileDeactivated;
			projectile.projectileType = ProjectileType.Homing;
			p = projectile;
		}

		SetLastPositions();
	}

	/// <summary>Sets LinkedList's Projectiles' Last Positions.</summary>
	private void SetLastPositions(float _time = 0.0f)
	{
		time = _time;

		foreach(Projectile projectile in projectilesList)
		{
			projectile.lastPosition = projectile.transform.position;
		}
	}

	/// <summary>Callback invoked when a projectile [as IPoolObject] gets deactivated.</summary>
	/// <param name="_object">IPoolObject .</param>
	private void OnProjectileDeactivated(IPoolObject _object)
	{
		Projectile projectile = _object as Projectile;
		projectile.onPoolObjectDeactivation -= OnProjectileDeactivated;

		if(projectile == null) return;

		projectiles.Remove(projectile);
		projectilesList = new LinkedList<Projectile>(projectiles);
		SetLastPositions(time);

		Projectile p = null;

		foreach(Projectile proj in projectilesList)
		{
			if(proj == projectilesList.First.Value)
			{
				proj.target = targetFunction;
				proj.parentProjectile = null;
			}
			else
			{
				proj.target = p.GetPosition;
				proj.parentProjectile = p;
			}

			p = proj;
		}
	}
}
}