using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public enum DashState
{
	Unactive,
	Dashing,
	OnCooldown
}

/// <summary>Event invoked whn a Dash State changes.</summary>
/// <param name="_state">New Entered State.</param>
public delegate void OnDashStateChange(DashState _state);

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DisplacementAccumulator2D))]
[RequireComponent(typeof(GravityApplier))]
public class DashAbility : MonoBehaviour, IFiniteStateMachine<DashState>
{
	public event OnDashStateChange onDashStateChange; 			/// <summary>OnDashStateChange's event delegate.</summary>

	[SerializeField] private ForceInformation2D _forceInfo; 	/// <summary>Force's Information.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _percentageLimit; 		/// <summary>Percentage's Limit.</summary>
	[SerializeField] private float _cooldownDuration; 			/// <summary>Cooldown's Duration.</summary>
	[SerializeField] private float _gravityScale; 				/// <summary>Gravity Scale change when the dashing begins.</summary>
	[SerializeField] private int _scaleChangePriority; 			/// <summary>Gravity Scale's Change Priority.</summary>
	private FloatWrapper _scaleWrapper; 							/// <summary>Gravity's Scale FloatWrapper.</summary>
	private TimeConstrainedForceApplier2D _forceApplier; 		/// <summary>Forces' Appliers.</summary>
	private DashState _state; 									/// <summary>Current State.</summary>
	private DashState _previousState; 							/// <summary>Previous State.</summary>
	private Rigidbody2D _rigidbody; 							/// <summary>Rigidbody's Component.</summary>
	private DisplacementAccumulator2D _accumulator; 			/// <summary>displacementAccumulator's Component.</summary>
	private GravityApplier _gravityApplier; 					/// <summary>GravityApplier's Comopnent.</summary>
	private Cooldown _cooldown; 								/// <summary>Cooldown's Reference.</summary>
	private Vector2 _accumulatedDisplacement; 					/// <summary>Accumulated's Displacement.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets forceInfo property.</summary>
	public ForceInformation2D forceInfo
	{
		get { return _forceInfo; }
		private set { _forceInfo = value; }
	}

	/// <summary>Gets and Sets percentageLimit property.</summary>
	public float percentageLimit
	{
		get { return _percentageLimit; }
		set { _percentageLimit = Mathf.Clamp(value, 0.0f, 1.0f); }
	}

	/// <summary>Gets and Sets cooldownDuration property.</summary>
	public float cooldownDuration
	{
		get { return _cooldownDuration; }
		set { _cooldownDuration = value; }
	}

	/// <summary>Gets and Sets gravityScale property.</summary>
	public float gravityScale
	{
		get { return _gravityScale; }
		set { _gravityScale = value; }
	}

	/// <summary>Gets and Sets scaleChangePriority property.</summary>
	public int scaleChangePriority
	{
		get { return _scaleChangePriority; }
		set { _scaleChangePriority = value; }
	}

	/// <summary>Gets and Sets scaleWrapper property.</summary>
	public FloatWrapper scaleWrapper
	{
		get { return _scaleWrapper; }
		set { _scaleWrapper = value; }
	}

	/// <summary>Gets and Sets forceApplier property.</summary>
	public TimeConstrainedForceApplier2D forceApplier
	{
		get { return _forceApplier; }
		set { _forceApplier = value; }
	}

	/// <summary>Gets and Sets state property.</summary>
	public DashState state
	{
		get { return _state; }
		set { _state = value; }
	}

	/// <summary>Gets and Sets accumulatedDisplacement property.</summary>
	public Vector2 accumulatedDisplacement
	{
		get { return _accumulatedDisplacement; }
		set { _accumulatedDisplacement = value; }
	}

	/// <summary>Gets and Sets previousState property.</summary>
	public DashState previousState
	{
		get { return _previousState; }
		set { _previousState = value; }
	}

	/// <summary>Gets rigidbody Component.</summary>
	public new Rigidbody2D rigidbody
	{ 
		get
		{
			if(_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
			return _rigidbody;
		}
	}

	/// <summary>Gets gravityApplier Component.</summary>
	public GravityApplier gravityApplier
	{ 
		get
		{
			if(_gravityApplier == null) _gravityApplier = GetComponent<GravityApplier>();
			return _gravityApplier;
		}
	}

	/// <summary>Gets accumulator Component.</summary>
	public DisplacementAccumulator2D accumulator
	{ 
		get
		{
			if(_accumulator == null) _accumulator = GetComponent<DisplacementAccumulator2D>();
			return _accumulator;
		}
	}

	/// <summary>Gets and Sets cooldown property.</summary>
	public Cooldown cooldown
	{
		get { return _cooldown; }
		set { _cooldown = value; }
	}

	/// <summary>Gets reachedMinMagnitude property.</summary>
	public bool reachedMinMagnitude { get { return state == DashState.Dashing ? accumulatedDisplacement.magnitude >= (forceApplier.ProjectedForceAtTime(forceInfo.duration).magnitude * percentageLimit) : false; } }
#endregion

	/// <summary>Callback invoked when DashAbility's instance is enabled.</summary>
	private void OnEnable()
	{
		//Dash();
	}

	/// <summary>DashAbility's instance initialization.</summary>
	private void Awake()
	{
		forceApplier = new TimeConstrainedForceApplier2D(
			accumulator,
			forceInfo.force,
			forceInfo.duration,
			forceInfo.forceMode,
			OnDashEnds
		);
		scaleWrapper = new FloatWrapper(gravityScale);
		cooldown = new Cooldown(this, cooldownDuration, OnCooldownEnds);
		this.ChangeState(DashState.Unactive);
	}

	/// <summary>Updates DashAbility's instance at each Physics Thread's frame.</summary>
	private void FixedUpdate()
	{
		if(state == DashState.Dashing)
		{ /// Measure the accumulated Dash's displacement if the current state is Dashing:
			accumulatedDisplacement = (forceApplier.velocity * forceApplier.timeScale * Time.fixedDeltaTime);
		}

	}

	/// <summary>Enters DashState State.</summary>
	/// <param name="_state">DashState State that will be entered.</param>
	public void OnEnterState(DashState _state)
	{
		switch(_state)
		{
			case DashState.Unactive:
			gravityApplier.RejectScaleChange(GetInstanceID());
			//cooldown.End();
			break;

			case DashState.Dashing:
			forceApplier.ApplyForce();
			gravityApplier.RequestScaleChange(GetInstanceID(), scaleWrapper, scaleChangePriority);
			break;

			case DashState.OnCooldown:
			gravityApplier.RejectScaleChange(GetInstanceID());
			cooldown.Begin();
			break;
	
			default:
			break;
		}

		if(onDashStateChange != null) onDashStateChange(_state);
	}
	
	/// <summary>Leaves DashState State.</summary>
	/// <param name="_state">DashState State that will be left.</param>
	public void OnExitState(DashState _state)
	{
		switch(_state)
		{
			case DashState.OnCooldown:
			//cooldown.End();
			break;
		}
	}

	/// <summary>Callback invoked when the jump ends.</summary>
	private void OnDashEnds()
	{
		this.ChangeState(DashState.OnCooldown);
	}

	/// <summary>Callback internally invoked when the cooldown ends.</summary>
	private void OnCooldownEnds()
	{
		this.ChangeState(DashState.Unactive);
	}

	/// <summary>Performs Dash's Ability.</summary>
	public void Dash()
	{
		if(state == DashState.Unactive)
		{
			accumulatedDisplacement = Vector3.zero;
			forceApplier.force = transform.right * forceInfo.force.x;
			this.ChangeState(DashState.Dashing);
		}
	}

	/// <summary>Performs Dash's Ability on the given direction.</summary>
	/// <param name="_direction">Dash's Direction.</param>
	public void Dash(Vector3 _direction)
	{
		if(state == DashState.Unactive)
		{
			forceApplier.force = _direction * forceInfo.force.x;
			this.ChangeState(DashState.Dashing);
		}
	}

	/// <summary>Cancels Dash's Ability.</summary>
	public void CancelDash()
	{
		if(state != DashState.Dashing) return;

		forceApplier.CancelForce();
		OnDashEnds();
	}
}
}